using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using log4net;
using log4net.Config;
using System.IO;
using System.Linq;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;
using static IntroSE.Kanban.Backend.BusinessLayer.Constants;

namespace IntroSE.Kanban.Backend.BusinessLayer;

public class Board
{
    public string Name { get; }
    private Column[] _columns = new Column[3];
    private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    public int BoardId { get; }
    public string Owner { get; set; }
    private int _taskCounter;
    internal BoardDTO BoardDTO;
    

    private Board(int boardId,string email, string name) //look create
    {
        BoardId = boardId;
        Name = name;
        _columns[Backlog] = Column.Create(NoLimit, boardId, Backlog);
        _columns[InProgress] = Column.Create(NoLimit, boardId, InProgress);
        _columns[Done] = Column.Create(NoLimit, boardId, Done);
        Owner = email;
        _taskCounter = 0;
        BoardDTO = new BoardDTO(boardId, name, email, 0);
        
        // Load configuration
        //Right click on log4net.config file and choose Properties. 
        //Then change option under Copy to Output Directory build action into Copy if newer or Copy always.
        var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
        XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
    }

    public Board(BoardDTO boardDto)
    {
        BoardId = boardDto.BoardId;
        Name = boardDto.Name;
        Owner = boardDto.OwnerEmail;
        _taskCounter = boardDto.TaskIncrement;
        BoardDTO = boardDto;

        List <ColumnDTO> columns = boardDto.GetColumnDtos();
        _columns[Backlog] = new Column(columns[Backlog]);
        _columns[InProgress] = new Column(columns[InProgress]);
        _columns[Done] = new Column(columns[Done]);
        
        //loading all the data into columns
        _columns[Backlog].LoadTasks();
        _columns[InProgress].LoadTasks();
        _columns[Done].LoadTasks();

        
        
        // Load configuration
        //Right click on log4net.config file and choose Properties. 
        //Then change option under Copy to Output Directory build action into Copy if newer or Copy always.
        var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
        XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
    }

    public static Board Create(int boardId,string email, string name)
    {
        Board b = new Board(boardId, email, name);
        b.BoardDTO.Persist(); //call to DTO to add in database
        return b;
    }
    
    public bool ExistsTask(int taskId, int coulmnId) // returns true if success, false if fail.
    {
        if (coulmnId < 0 | coulmnId > 2)
            return false;
        bool exists = _columns[coulmnId].ExistsTask(taskId);
        if (exists)
            log.Info($"task (id: {taskId}) exists in column: '{coulmnId}' in board: '{Name}'");
        else
            log.Warn($"task (id: {taskId}) does not exist in column: '{coulmnId}' in board: '{Name}'");
        return exists;
    }

    public Task GetTask(int taskId, int columnId)
    {
        return _columns[columnId].GetTask(taskId);
    }

    public void AddTask(string title, string description, DateTime dueDate,string assignee) // returns true if success, false if fail.
    {
        _columns[Backlog].ValidateLimit(); //check that task can be added
        Task t = Task.Create(_taskCounter, dueDate, title, description,assignee, BoardId); //error handling is in the constructor called by create
        //note: assignee is already validated in the SL
        log.Info($"Task (id: {t.Id}) has been added successfully to board: '{Name}'");
       _columns[Backlog].AddTask(t);
        _taskCounter++; //to add the id counter
        BoardDTO.AppendTaskCounter(); //taskCounter+1
    }

    public void AdvanceTask(string email, int columnOrdinal, int taskId) // returns true if success, false if fail.
    {
        ValidateAdvanceOrdinal(columnOrdinal);
        Task t = _columns[columnOrdinal].GetTask(taskId); // validation of TaskId in "GetTask"
        if (!email.Equals(t.Assignee)) // only assignee has premission
        {
            throw new ArgumentException($"user:{email} tried to advance task (id: {taskId}) while not its assignee");
        }
        _columns[columnOrdinal + 1].ValidateLimit(); //check that next column has enough space
        t.TaskDTO.AdvanceTask(); // first db and THEN in RAM.
        _columns[columnOrdinal + 1].AddTask(t); // advance to next column
        _columns[columnOrdinal].RemoveTask(taskId);
        log.Info($"advanced task (id: {taskId}) successfully at board: '{Name}'");
        

    }

    public void LimitColumn(int columnOrdinal, int limit) // returns true if success, false if fail.
    {
        ValidateOrdinal(columnOrdinal); //check this ordinal is valid
        _columns[columnOrdinal].SetLimit(limit);
        log.Info($"limited column (ordinal: {columnOrdinal}) to the limit: {limit} at board: '{Name}'");
        //the Column is responsible to handle DB in this case
    }

    public int GetColumnLimit(int columnOrdinal) // on RAM
    {
       ValidateOrdinal(columnOrdinal); //if 0 , 1 or 2 without magic number
       return _columns[columnOrdinal].GetLimit();
    }

    
    public string GetColumnName(int columnOrdinal) // on RAM
    {
        ValidateOrdinal(columnOrdinal);
        if (columnOrdinal == Backlog)
            return "backlog";
        if (columnOrdinal == InProgress)
            return "in progress";
        return "done";
    }

    public List<Task> GetColumn(int columnOrdinal)
    {
        ValidateOrdinal(columnOrdinal); //validate column 
        return _columns[columnOrdinal].GetTasks(); //gets tasks from coulumn
    }

    public List<Task> InProgressTasks(string email)
    {
        return _columns[InProgress].GetAssigneeTasks(email); //gets all the task
    }
    

    public void AssignTask(string email, int columnOrdinal, int taskId, string emailAssignee)
    {
        ValidateOrdinal(columnOrdinal);
        Task toAssign = GetTask(taskId, columnOrdinal); //getting the task that we assign to
        toAssign.AssignTask(email,emailAssignee); // checks are made there

    }

    public void TransferOwnership(string currentOwnerEmail, string newOwnerEmail)
    {
        if (!Owner.Equals(currentOwnerEmail)) // user is not the actual owner
            throw new ArgumentException($"user {currentOwnerEmail} is not the owner,{Owner} is.");
        BoardDTO.TransferOwnership(newOwnerEmail); //transfer owner ship in DB
        Owner = newOwnerEmail;
        
        log.Info($"user: {currentOwnerEmail} has transfered ownership of board {Name} id: {BoardId} to user {newOwnerEmail} ");

    }

    internal List<Task> GetUserTasksThatNotDone(string email) // helps us to unAssign tasks when user leaves a board
    {
        List<Task> lst = new List<Task>();
        foreach (Task t in _columns[Backlog].GetTasks())
        {
            if(t.Assignee.Equals(email)) //all tasks in backlog that belong to user
                lst.Add(t);
        }
        foreach (Task t in _columns[InProgress].GetTasks())
        {
            if(t.Assignee.Equals(email)) //all tasks in inprogress that belong to user
                lst.Add(t);
        }

        return lst;
    }
    
    public void UpdateTaskDueDate(string email, int columnOrdinal, int taskId, DateTime dueDate)
    {
        ValidateUpdateTaskOrdinal(columnOrdinal); //validate oridnal
        Task t = GetTask(taskId, columnOrdinal);
        t.UpdateTaskDueDate(email,dueDate); //checks are made there
    }
    public void UpdateTaskTitle(string email, int columnOrdinal, int taskId, String title)
    {
        ValidateUpdateTaskOrdinal(columnOrdinal); //validate ordinal
        Task t = GetTask(taskId, columnOrdinal);
        t.UpdateTaskTitle(email,title); //checks are made there
    }
    public void UpdateTaskDescription(string email, int columnOrdinal, int taskId, String desc)
    {
        ValidateUpdateTaskOrdinal(columnOrdinal); //validate ordinal
        Task t = GetTask(taskId, columnOrdinal);
        t.UpdateTaskDescription(email,desc); //checks are made there
    }
    
    
    private void ValidateOrdinal(int ordinal)
    {
        if (!(ordinal >= Backlog & ordinal <= Done))
            throw new ArgumentException($"column ordinal {ordinal} is not valid");
    }
    
    private void ValidateUpdateTaskOrdinal(int ordinal) //may not be done
    {
        if (ordinal != InProgress && ordinal != Backlog)
            throw new ArgumentException($"column ordianl {ordinal} is not valid for task advancement");
    }

    private void ValidateAdvanceOrdinal(int ordinal)
    {
        if (ordinal != InProgress && ordinal != Backlog)
            throw new ArgumentException($"column ordianl {ordinal} is not valid for task advancement");
    }
    
}