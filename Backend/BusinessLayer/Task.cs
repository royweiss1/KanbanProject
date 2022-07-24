using System;
using System.Reflection;
using log4net;
using log4net.Config;
using System.IO;
using System.Text.Json.Serialization;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;
using static IntroSE.Kanban.Backend.BusinessLayer.Constants;

namespace IntroSE.Kanban.Backend.BusinessLayer;

public class Task
{
    private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    public int Id {get;}
    public DateTime CreationTime { get; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public string Assignee { get; set; }
    [JsonIgnore]
    public  bool IsAssigned
    {
        get => Assignee != null;
    }
    internal TaskDTO TaskDTO { get; set; }

    private Task(int id, DateTime dd, string title,string description,string assignee, int boardId)
    {
        CreationTime = DateTime.Now;
        if (!ValidTitle(title))
            throw new ArgumentException("title is not valid");
        if (!ValidDescription(description))
            throw new ArgumentException("description is not valid");
        if (!ValidateDueDate(dd))
            throw new ArgumentException("dueDate is not valid");
        Id = id;
        DueDate = dd;
        Title = title;
        Description = description;
        Assignee = assignee;

        TaskDTO = new TaskDTO(boardId, Id, CreationTime, DueDate, Title, Description, Assignee, Backlog);
        
        
        // Load configuration
        var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
        XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
    }

    [JsonConstructor]
    public Task(int id, DateTime creationTime, string title, string description, DateTime dueDate, string assignee)
    {
        Id = id;
        CreationTime = creationTime;
        Title = title;
        Description = description;
        DueDate = dueDate;
        Assignee = assignee;
    }

    public static Task Create(int id, DateTime dd, string title, string description, string assignee, int boardId)
    {
        Task t = new Task(id, dd, title, description, assignee, boardId);
        t.TaskDTO.Persist(); //adding the task in DB
        return t;
    }

    public Task(TaskDTO task)
    {
        Id = task.TaskId;
        CreationTime = task.CreationDate;
        DueDate = task.DueDate;
        Title = task.Title;
        Description = task.Description;
        Assignee = task.Assignee;
        TaskDTO = task;
        
        // Load configuration
        var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
        XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
    }
    
    public void UpdateTaskDueDate(string email, DateTime dueDate)// returns true if success, false if fail.
    {
        if (!ValidateDueDate(dueDate)) //check date
        {
            log.Error($"Task (id:{Id}) has failed to updated its dueDate. the input was: {dueDate}");
            throw new ArgumentException($"Task (id:{Id}) has failed to updated its dueDate. the input was: {dueDate}");

        }
        if (!email.Equals(Assignee)) // check assignee.
        {
            log.Error($"Task (id:{Id}) has failed to updated its dueDate, user is not the assignee");
            throw new ArgumentException($"Task (id:{Id}) has failed to updated its dueDate, user is not the assignee");

           
        }
        TaskDTO.UpdateTaskDueDate(dueDate); //update in DAL
        DueDate = dueDate;
        log.Info($"Task (id:{Id}) has updated its DueDateTime to: {DueDate}");
    }


    public void UpdateTaskTitle(string email, string title)// returns true if success, false if fail.
    {
        if (!ValidTitle(title)) //check title
        {
            log.Error($"Task (id:{Id}) has failed to updated its title. the input was: {title}");
            throw new ArgumentException($"Task (id:{Id}) has failed to updated its title. the input was: {title}");
        }

        if (!email.Equals(Assignee)) //validate assignee
        {
            log.Error($"Task (id:{Id}) has failed to updated its title, user is not the assignee");
            throw new ArgumentException($"Task (id:{Id}) has failed to updated its title, user is not the assignee");
        }
        TaskDTO.UpdateTaskTitle(title); // update in DAL
        Title = title;
        log.Info($"Task (id:{Id}) has updated its title to {Title}");

    }

    private bool ValidTitle(string s)// returns true if success, false if fail.
    {
        if (s == null)
            return false;
        return s.Length > 0 & s.Length <= TitleLimit;
    }

    private bool ValidateDueDate(DateTime dueDate)// returns true if success, false if fail.
    {
        return DateTime.Compare(dueDate, CreationTime) >= 0; // check if due date is later or equals to creation date
    }

    public void UpdateTaskDescription(string email, string description)// returns true if success, false if fail.
    {
        if (description == null)
        {
            throw new ArgumentException("description cant be null");
        }
        if (!ValidDescription(description)) //check valid description
        {
            log.Error($"Task (id:{Id}) has failed to updated its description to: \n {description}");
            throw new ArgumentException($"Task (id:{Id}) has failed to updated its description to: \n {description}");
        }
        if (!email.Equals(Assignee)) //check valid assignee
        {
            log.Error($"Task (id:{Id}) has failed to updated its description, user is not the assignee");
            throw new ArgumentException($"Task (id:{Id}) has failed to updated its description, user is not the assignee");
        }
        TaskDTO.UpdateTaskDescription(description);// update in DAl
        Description = description;
        
        log.Info($"Task (id:{Id}) has updated its description to: \n {Description}");
    }

    private bool ValidDescription(string s)// returns true if success, false if fail.
    {
        return s.Length <= DescriptionLimit;
    }

    public void AssignTask(string assignee, string newAssignee) // if we got here we ALREADY checked if the task has an assignee.
    {
        if (!IsAssigned) // if no assignee before everyone may assign anyone
            Assignee = newAssignee;
        else if (!Assignee.Equals(assignee)) // if even allowed to
            throw new ArgumentException($"{assignee} is not the assignee, he cant change the tasks assignee");
        TaskDTO.AssignTask(newAssignee); // first dal then RAM
        Assignee = newAssignee;
        
        log.Info($"user {assignee} has assigned {newAssignee} to task: {Id} successfully");
    } 
    

}