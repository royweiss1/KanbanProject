using System;

namespace Frontend.Model;

public class TaskModel : NotifiableModelObject
{
    private int taskId;

    public int TaskId
    {
        get=>taskId;
        set
        {
            taskId = value;
            RaisePropertyChanged("TaskId");
        }
    }

    private string title;

    public string Title
    {
        get=>title;
        set
        {
            title = value;
            RaisePropertyChanged("Title");
        }
    }
    private string description;

    public string Description
    {
        get=>description;
        set
        {
            description = value;
            RaisePropertyChanged("Description");
        }
    }
    private string assignee;

    public string Assignee
    {
        get=>assignee;
        set
        {
            assignee = value;
            RaisePropertyChanged("Assignee");
        }
    }
    private DateTime dueDate;

    public DateTime DueDate
    {
        get=>dueDate;
        set
        {
            dueDate = value;
            RaisePropertyChanged("DueDate");
        }
    }
    private DateTime creationDate;

    public DateTime CreationDate
    {
        get=>creationDate;
        set
        {
            creationDate = value;
            RaisePropertyChanged("CreationDate");
        }
    }


    //thats all the essential info to show
    public TaskModel(int taskId, string title, string description, string assignee, DateTime dd, DateTime cd, BackendController controller) : base(controller)
    {
        TaskId = taskId;
        Title = title;
        Description = description;
        Assignee = assignee;
        DueDate = dd;
        CreationDate = cd;
    }


}