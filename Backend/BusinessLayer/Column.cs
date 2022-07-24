using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using log4net.Config;
using System.IO;
using System.Runtime.CompilerServices;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;
using static IntroSE.Kanban.Backend.BusinessLayer.Constants;

namespace IntroSE.Kanban.Backend.BusinessLayer;

internal class Column
{
    
    private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    private Dictionary<int, Task> _tasks = new ();
    private int Limit { get; set; }
    
    private ColumnDTO ColumnDto { get; }

    private Column(int lim,int boardid, int ordinal)
    {
        Limit = lim;
        ColumnDto = new ColumnDTO(boardid, ordinal, lim);
        // Load configuration
        var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
        XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
    }

    public Column(ColumnDTO columnDto)
    {
        Limit = columnDto.ColumnLimit;
        ColumnDto = columnDto;
    }

    public static Column Create(int lim, int boardid, int ordinal)
    {
        Column c = new Column(lim,boardid,ordinal);
        c.ColumnDto.Persist(); //adding the coulumn in DTO
        return c;
    } 
    
    public bool ExistsTask(int Id)// returns true if success, false if fail.
    {
        bool exists= _tasks.ContainsKey(Id);
        if (exists)
            log.Info($"task (id: {Id}) exists");
        else
            log.Warn($"task (id: {Id}) does not exist");
        return exists;
    }

    public Task GetTask(int Id)
    {
        if (!_tasks.ContainsKey(Id))
            throw new ArgumentException($"task with id: {Id} does not exists");
        return _tasks[Id];
    }

    public int NumberOfTasks()
    {
        return _tasks.Count;
    }

    public void AddTask(Task t)// returns true if success, false if fail.
    {
        _tasks[t.Id] = t;
        log.Info($"Task (id: {t.Id}) has been added succssesfully to column");
    }

    public void RemoveTask(int Id)
    {
        if (!_tasks.ContainsKey(Id))
            throw new ArgumentException($"task with id: {Id} does not exists");
        _tasks.Remove(Id); //remove the task from column
        log.Info($"Task (id: {Id}) has been removed from column");

    }

    public void SetLimit(int limit)// returns true if success, false if fail.
    {
        if (limit < 0)
        {
            if (limit != NoLimit)
            {
                log.Warn("tried to limit column to negative number");
                throw new AggregateException($"limit{limit} is not valid, cant be negative");
            }
            ColumnDto.LimitColumn(NoLimit); //set limit if valid in dto
            Limit = limit; // setting to -1.
        }
        else
        {
            if (NumberOfTasks() > limit)
            {
                // if there are more tasks than inserted limit.
                log.Warn("tried to limit column but NumberOfTasks() > limit");
                throw new ArgumentException($"cant exceed{Limit} tasks in the board.");
            }

            ColumnDto.LimitColumn(limit);
            Limit = limit; //setting to limit user requeste

        }

        
    }

    public int GetLimit()
    {
        return Limit;
    }

    public List<Task> GetTasks()
    {
        return _tasks.Values.ToList();
    }

    public List<Task> GetAssigneeTasks(string assignee) // ** new ** 
    {
        return _tasks.Values.Where(t => assignee.Equals(t.Assignee)).ToList();
    }


    public void LoadTasks()
    {
        List<TaskDTO> taskDtos = ColumnDto.GetTaskDtos(); //gets all the task dto's
        foreach (var taskDto in taskDtos)
        {
            Task t = new Task(taskDto);
            AddTask(t); //adds the task

        }
    }

    public void ValidateLimit()
    {
        if (Limit != NoLimit && NumberOfTasks() >= Limit)
        {
            throw new ArgumentException($"has failed to add to column because cant exceed max limit");
        }
            
    }
}