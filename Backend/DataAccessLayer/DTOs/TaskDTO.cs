using System;
using System.Collections.Generic;


namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOs
{
    public class TaskDTO : DTO
    {
        public const string TaskBoardIdName = "BoardId";
        public const string TaskIdName = "TaskId";
        public const string TaskCreationDateName = "CreationDate";
        public const string TaskDueDateName = "DueDate";
        public const string TaskTitleName = "Title";
        public const string TaskDescriptionName = "Description";
        public const string TaskAssigneeName = "Assignee";
        public const string TaskColumnOrdinalName = "ColumnOrdinal";
        
        public int BoardId { get; set; } // FK or board
        public int TaskId { get; set; }
        public DateTime CreationDate { get; }
        private DateTime dueDate;

        public DateTime DueDate
        {
            get => dueDate;
            private set
            {
                //TODO: mind the toString
                string time = value.ToString();
                new TaskDTOMapper().Update(BoardId, TaskId, TaskDueDateName, time);
                dueDate = value;
            }
        }

        private string title;

        public string Title
        {
            get => title;
            private set
            {
                new TaskDTOMapper().Update(BoardId, TaskId, TaskTitleName, value); //update in DB
                title = value; //if valid update here to
            }
        }

        private string description;

        public string Description
        {
            get => description;
            private set
            {
                new TaskDTOMapper().Update(BoardId, TaskId, TaskDescriptionName, value); //update in DB
                description = value; //if valid update here too
            }
        }

        private string assignee;

        public string Assignee
        {
            get => assignee;
            private set
            {
                new TaskDTOMapper().Update(BoardId, TaskId, TaskAssigneeName, value); //update in DB
                assignee = value; //if valid update here too
            }
        } // FK of User

        private int columnOrdinal;
        public int ColumnOrdinal
        {
            get => columnOrdinal;
            private set
            {
                new TaskDTOMapper().Update(BoardId, TaskId, TaskColumnOrdinalName, value); //update in DB
                columnOrdinal = value; //if valid update here too
            }
        }

        public TaskDTO(int boardId, int taskId,DateTime cd,DateTime dd, string title,string description,string assignee, int columnOrdinal)
            :base(new TaskDTOMapper())
        {
            BoardId = boardId;
            TaskId = taskId;
            CreationDate = cd;
            dueDate = dd;
            this.title = title;
            this.description = description;
            this.assignee = assignee;
            this.columnOrdinal = columnOrdinal;
            
            
        }

        public void UpdateTaskTitle(string title)
        {
            Title = title;
            log.Info($"Task (id={TaskId} in board: {BoardId} has updated its title to: {Title})");
        }

        public void UpdateTaskDescription(string description)
        {
            Description = description;
            log.Info($"Task (id={TaskId} in board: {BoardId} has updated its description to: {Description})");

        }

        public void UpdateTaskDueDate(DateTime dueDate)
        {
            DueDate = dueDate;
            log.Info($"Task (id={TaskId} in board: {BoardId} has updated its dueDate to: {DueDate})");

        }

        public void AssignTask(string newAssignee)
        {
            Assignee = newAssignee;
            log.Info($"Task (id={TaskId} in board: {BoardId} has updated its assignee to: {Assignee})");

        }

        public void AdvanceTask()
        {
            ColumnOrdinal = columnOrdinal+1;
            log.Info($"Task (id={TaskId} in board: {BoardId} has updated its column to: {ColumnOrdinal})");

        }

        internal void UnAssignTask()
        {
            Assignee = null;
            log.Info($"Task (id={TaskId} in board: {BoardId} has been UnAssigned (Assignee=null))");

        }

        public override void Persist()
        {
            new TaskDTOMapper().AddTask(this); //add this task
            log.Info($"Task (id={TaskId} in board: {BoardId} has been added to DB");

        }
    }
}