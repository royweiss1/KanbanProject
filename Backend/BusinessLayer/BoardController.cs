using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using log4net;
using log4net.Config;
using System.IO;
using IntroSE.Kanban.Backend.DataAccessLayer;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;

namespace IntroSE.Kanban.Backend.BusinessLayer;

internal class BoardController
{
    private Dictionary<string, Dictionary<string, Board>> _boardsPerUser;
    private Dictionary<int, Board> _boards; // added, <ID,BOARD> dict.

    private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private static int BoardsCounter;
    public BoardController()
    {
        _boardsPerUser = new();
        _boards = new();
        // Load configuration
        var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
        XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
    }

    public bool ExistsBoard(int boardId) //exists in the system
        =>  _boards.ContainsKey(boardId);

    public List<int> GetUserBoards(string email)  // returns list of boards id's
    {
        List<int> l = new List<int>();
        if (_boardsPerUser.ContainsKey(email))
        {
            List<Board> boards = _boardsPerUser[email].Values.ToList(); //gets all the boards in list
            foreach (Board b in boards)
            {
                l.Add(b.BoardId); //adds id of boards
            }
        }

        return l;
    }


    private Board ValidateBoardForUser(string email, string boardName)
    {
        if (email == null || boardName == null)
            throw new ArgumentException("parameters cant be null");
        if (!_boardsPerUser.ContainsKey(email)) // if there are no boards for user
        {
            log.Warn($"user: {email} has no boards, therefor board: {boardName} does not exists");
            throw new ArgumentException($"user: {email} has no boards, therefor board: {boardName} does not exists");
        }
        Dictionary<string, Board> boards = _boardsPerUser[email];

        bool exists= boards.ContainsKey(boardName); // check for board by name
        if (exists)
            log.Info($"board name: {boardName} exists for user: '{email}'");
        else
        {
            log.Warn($"board name: {boardName} does not exists for user: '{email}'");
            throw new ArgumentException($"board name: {boardName} does not exists for user: '{email}'");
        }

        return GetBoard(email, boardName); //valid so we call GetBoard
    }

    private bool ExistsBoardForUser(string email, string boardName) =>
        _boardsPerUser.ContainsKey(email) && _boardsPerUser[email].ContainsKey(boardName);

    
    // when one wants to get board should use "ValidateBoardForUser"
    private Board GetBoard(string email, string boardName)=> _boardsPerUser[email][boardName];

    public string GetBoardNameById(int id)
    {
        return _boards[id].Name;
    }
    
    public void AddBoard(string email, string name)// returns true if success, false if fail.
    {
        if (!_boardsPerUser.ContainsKey(email)){ // if it is the first board we are adding, need to initialize a new dictionary for the user
            _boardsPerUser.Add(email,new Dictionary<string, Board>());
            log.Info($"user's {email} list of boards has just been initialized");
        }
        else
        {
            if (ExistsBoardForUser(email, name)) // board name already exists
                throw new ArgumentException($"board: {name} already exists for user: {email}");
        }
        
        Board b = Board.Create(BoardsCounter, email, name); 
        _boardsPerUser[email].Add(name, b); //adds the board to user
        _boards.Add(b.BoardId, b); // *changed* adding to new dict.
        BoardsCounter++; // updating id counter.
        
        log.Info($"added board: {name} to user: {email} successfully");
        
    }
    
    
    public void RemoveBoard(string email, string name)
    {
        ValidateBoardForUser(email,name);
        if (!_boardsPerUser[email][name].Owner.Equals(email)) // check if user is the owner
        {
            throw new ArgumentException($"user: {email} is not the owner of the board and can't remove it");
        }
        Board toRemove =  _boardsPerUser[email][name]; //get the board to remove
        
        toRemove.BoardDTO.DeleteBoard(); // FIRST REMOVE FROM DB! then from RAM
        foreach (Dictionary<string,Board> boardsPerUser in _boardsPerUser.Values) // removing board from every user that has this board.
        {
            if (boardsPerUser.ContainsKey(toRemove.Name))
                boardsPerUser.Remove(toRemove.Name); //remove the board 

        }
        _boards.Remove(toRemove.BoardId); // removing from id-board dict.
        
        log.Info($"board: {name} of user: {email} has been successfully removed");
    }

    public List<Task> InProgressTasks(string email)
    {
        List < Task > inProgressTasksList= new List<Task>();
        if (_boardsPerUser.ContainsKey(email)) // if even has some
        {
            Dictionary<string, Board> currBoards = _boardsPerUser[email]; //gets all the user boards 
            foreach (Board br in currBoards.Values)
            {
                List<Task> currInProgressTasks = br.InProgressTasks(email); // changed so the tasks returned are only those assigned by user.
                foreach (Task t in currInProgressTasks)
                {
                    inProgressTasksList.Add(t); 
                }
            }
        }
        return inProgressTasksList;
    }
    public void JoinBoard(string email, int boardId) 
    {
        if (!ExistsBoard(boardId))
            throw new ArgumentException($"board with id: {boardId} does not exists");
        Board b = _boards[boardId];
        if (ExistsBoardForUser(email, b.Name)) //already has a board with this name
            throw new ArgumentException($"user {email} already has a board with the same name or the same board.");
        
        b.BoardDTO.JoinBoard(email,boardId); // update that user joined the board in DB
        if (!_boardsPerUser.ContainsKey(email))
            _boardsPerUser[email] = new Dictionary<string, Board>();
        _boardsPerUser[email].Add(b.Name,b); // adding board to user.
       
        
        log.Info($"User {email} has joined the board id: {boardId} successfully");
    }
    public void LeaveBoard(string email, int boardId)
    {
        if (!ExistsBoard(boardId))//first check if board with this id exists
            throw new ArgumentException($"board with id: {boardId} does not exist");
        Board b=ValidateBoardForUser(email,_boards[boardId].Name);
        if (b.Owner == email) //owner may not leave
            throw new ArgumentException($"user: {email} is the owner of the board");
        
        b.BoardDTO.LeaveBoard(email,boardId); // first update DB and then RAM!
        foreach (Task t in b.GetUserTasksThatNotDone(email)) // unAssigning user's Tasks
        {
            t.Assignee = null;
        }
        _boardsPerUser[email].Remove(b.Name); // removing board from user.
        
        log.Info($"User {email} has left the board id: {boardId} successfully");

    }

    public void LimitColumn(string email, string boardName, int columnOrdinal, int limit)
    {
        Board board = ValidateBoardForUser(email, boardName);
        board.LimitColumn(columnOrdinal, limit); //checks are made there
    }

    public int GetColumnLimit(string email, string boardName, int columnOrdinal)
    {
        Board board = ValidateBoardForUser(email, boardName);
        return board.GetColumnLimit(columnOrdinal); //checks are made there
    }
    
    public int GetColumnLimit(int boardId, int columnOrdinal) //only used by UI
    {
        if (!_boards.ContainsKey(boardId))
            throw new NotSupportedException($"board: {boardId} does not exist");
        Board board = _boards[boardId];
        return board.GetColumnLimit(columnOrdinal); //checks are made there
    }

    public String GetColumnName(string email, string boardName, int columnOrdinal)
    {
        Board board = ValidateBoardForUser(email, boardName);
        return board.GetColumnName(columnOrdinal); //checks are made there
    }

    public List<Task> GetColumn(string email, string boardName, int columnOrdinal)
    {
        Board board = ValidateBoardForUser(email, boardName);
        return board.GetColumn(columnOrdinal); //checks are made there
    }

    public void AddTask(string email, string boardName, string title, string description, DateTime dueDate)
    {
        Board board = ValidateBoardForUser(email, boardName);
        board.AddTask(title, description, dueDate, null); //checks are made there
    }

    public void AdvanceTask(string email, string boardName, int columnOrdinal, int taskId)
    {
        Board board = ValidateBoardForUser(email, boardName);
        board.AdvanceTask(email, columnOrdinal, taskId); //checks are made there
    }

    public void AssignTask(string email, string boardName, int columnOrdinal, int taskID, string emailAssignee)
    {
        Board board1 = ValidateBoardForUser(email, boardName);
        Board board2 = ValidateBoardForUser(emailAssignee, boardName); 
        if (board1.BoardId != board2.BoardId) //checks that both users are in the same board
            throw new ArgumentException($"users: {email},{emailAssignee} dont have the same board:{boardName}");
        board1.AssignTask(email, columnOrdinal, taskID, emailAssignee);
    }

    
    public void TransferOwnerShip(string currentOwnerEmail, string newOwnerEmail, string boardName)
    {
        Board board1 = ValidateBoardForUser(currentOwnerEmail, boardName);
        Board board2 = ValidateBoardForUser(newOwnerEmail, boardName);
        if (board1.BoardId != board2.BoardId) //checks that both users are in the same board
            throw new ArgumentException($"users: {currentOwnerEmail},{newOwnerEmail} dont have the same board:{boardName}");
        board1.TransferOwnership(currentOwnerEmail, newOwnerEmail);
    }

    public void UpdateTaskDueDate(string email, string boardName, int columnOrdinal, int taskId, DateTime dueDate)
    {
        Board board = ValidateBoardForUser(email, boardName);
        board.UpdateTaskDueDate(email,columnOrdinal, taskId, dueDate); //checks are made there
    }
    public void UpdateTaskTitle(string email, string boardName, int columnOrdinal, int taskId, String title)
    {
        Board board = ValidateBoardForUser(email, boardName);
        board.UpdateTaskTitle(email,columnOrdinal, taskId, title); //checks are made there
    }
    
    public void UpdateTaskDescription(string email, string boardName, int columnOrdinal, int taskId, String desc)
    {
        Board board = ValidateBoardForUser(email, boardName);
        board.UpdateTaskDescription(email,columnOrdinal, taskId, desc); //checks are made there
    }

    public void LoadData()
    {
        List<BoardDTO> boards = new BoardDTOMapper().LoadAllBoardDtos(); //loading al the dto's
        Dictionary<string, List<BoardDTO>> boardsPerUser = new BoardUserMapper().LoadBoardsPerUsers();//<email,List<boardid's>>
        BoardsCounter = 0;
        foreach (var boardDto in boards)// load to id-board hashmap.
        {
            Board board = new Board(boardDto);
            _boards[board.BoardId] = board; //add board to list
            BoardsCounter = board.BoardId > BoardsCounter ? board.BoardId : BoardsCounter; //loading the boardCounter
        }
        if(_boards.Any()) //at least one board exists
            BoardsCounter++; //because id starts from zero
        
        foreach (var boardsForUser in boardsPerUser)
        {
            Dictionary<string,Board> userBoards = new Dictionary<string,Board>();
            foreach (var board in boardsForUser.Value)
            {
                Board b = _boards[board.BoardId];
                userBoards[b.Name] = b; 
            }
            _boardsPerUser[boardsForUser.Key] = userBoards; //adds the user with all his board
        }
    }

    internal void DeleteData()
    {
        //deletes all the data from everywhere
        new BoardDTOMapper().DeleteAll(); 
        new BoardUserMapper().DeleteAll(); 
        new ColumnMapper().DeleteAll();
        new TaskDTOMapper().DeleteAll();
        _boards = new();
        _boardsPerUser = new();
        BoardsCounter = 0;
    }
    
    public List<Task> GetColumn(int boardId, int columnOrdinal)
    {
        if (!_boards.ContainsKey(boardId))
            throw new Exception($"Does not contain board: {boardId}");
        Board board = _boards[boardId];
        return board.GetColumn(columnOrdinal); //checks are made there
    }
}