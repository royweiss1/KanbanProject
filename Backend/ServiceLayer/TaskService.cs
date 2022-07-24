using System;
using System.Text.Json;
using IntroSE.Kanban.Backend.BusinessLayer;

namespace IntroSE.Kanban.Backend.ServiceLayer;

public class TaskService
{
    private BoardController _boardController;
    private UserController _userController;

    internal TaskService(UserController uc,BoardController bc)
    {
        _boardController = bc;
        _userController = uc;

    }
          
        /// <summary>
    /// This method updates the due date of a task
    /// </summary>
    /// <param name="email">Email of the user. Must be logged in</param>
    /// <param name="boardName">The name of the board</param>
    /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
    /// <param name="taskId">The task to be updated identified task ID</param>
    /// <param name="dueDate">The new due date of the column</param>
    /// <returns>The string "{}", unless an error occurs (see <see cref="GradingService"/>)</returns>
    public string UpdateTaskDueDate(string email, string boardName, int columnOrdinal, int taskId, DateTime dueDate)
    {
        try
        {
            _userController.ValidateUser(email);
           _boardController.UpdateTaskDueDate(email, boardName, columnOrdinal,taskId,dueDate);
            return JsonSerializerExtention.Serialize(new Response(null, null));

        }
        catch (Exception e)
        {
            return JsonSerializerExtention.Serialize(new Response(e.Message, null));

        }
    }


    /// <summary>
    /// This method updates task title.
    /// </summary>
    /// <param name="email">Email of user. Must be logged in</param>
    /// <param name="boardName">The name of the board</param>
    /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
    /// <param name="taskId">The task to be updated identified task ID</param>
    /// <param name="title">New title for the task</param>
    /// <returns>The string "{}", unless an error occurs (see <see cref="GradingService"/>)</returns>
    public string UpdateTaskTitle(string email, string boardName, int columnOrdinal, int taskId, string title)
    {
        try
        {
            _userController.ValidateUser(email);
            _boardController.UpdateTaskTitle(email, boardName, columnOrdinal,taskId,title);
            return JsonSerializerExtention.Serialize(new Response(null, null));

        }
        catch (Exception e)
        {
            return JsonSerializerExtention.Serialize(new Response(e.Message, null));

        }
    }

    /// <summary>
    /// This method updates the description of a task.
    /// </summary>
    /// <param name="email">Email of user. Must be logged in</param>
    /// <param name="boardName">The name of the board</param>
    /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
    /// <param name="taskId">The task to be updated identified task ID</param>
    /// <param name="description">New description for the task</param>
    /// <returns>The string "{}", unless an error occurs (see <see cref="GradingService"/>)</returns>
    public string UpdateTaskDescription(string email, string boardName, int columnOrdinal, int taskId, string description)
    {
        try
        {
            _userController.ValidateUser(email);
            _boardController.UpdateTaskDescription(email, boardName, columnOrdinal,taskId,description);
            return JsonSerializerExtention.Serialize(new Response(null, null));

        }
        catch (Exception e)
        {
            return JsonSerializerExtention.Serialize(new Response(e.Message, null));

        }
    }
}