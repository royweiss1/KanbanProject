using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text.Json;
using IntroSE.Kanban.Backend.BusinessLayer;

namespace IntroSE.Kanban.Backend.ServiceLayer;

public class BoardService
{
    private BoardController _boardController;
    private UserController _userController;

    internal BoardService(UserController uc,BoardController bc)
    {
        _boardController = bc;
        _userController = uc;

    }

    /// <summary>
        /// This method limits the number of tasks in a specific column.
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="limit">The new limit value. A value of -1 indicates no limit.</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
       public string LimitColumn(string email, string boardName, int columnOrdinal, int limit)
        {
            try
            {
                _userController.ValidateUser(email);
                _boardController.LimitColumn(email,boardName,columnOrdinal,limit);
                return JsonSerializerExtention.Serialize(new Response(null, null));

            }
            catch(Exception e)
            {
                return JsonSerializerExtention.Serialize( new Response(e.Message, null)); 

                
            }
        }

        /// <summary>
        /// This method gets the limit of a specific column.
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>Response with column limit value, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string GetColumnLimit(string email, string boardName, int columnOrdinal)
        {
            try
            {
                _userController.ValidateUser(email);
                int lim = _boardController.GetColumnLimit(email, boardName, columnOrdinal);
                return JsonSerializerExtention.Serialize(new Response(null, lim));

            }
            catch (Exception e)
            {
                return JsonSerializerExtention.Serialize(new Response(e.Message, null));

            }
        }

        /// <summary>
        /// This method gets the name of a specific column
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>Response with column name value, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string GetColumnName(string email, string boardName, int columnOrdinal)
        
        {
            try
            {
                _userController.ValidateUser(email);
                String name = _boardController.GetColumnName(email, boardName, columnOrdinal);
                return JsonSerializerExtention.Serialize(new Response(null, name));

            }
            catch (Exception e)
            {
                return JsonSerializerExtention.Serialize(new Response(e.Message, null));

            }
        }
        /// <summary>
        /// This method returns a column given it's name
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>Response with a list of the column's tasks, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string GetColumn(string email, string boardName, int columnOrdinal)
        {
            try
            {
                _userController.ValidateUser(email);
                List<Task> tasks = _boardController.GetColumn(email, boardName, columnOrdinal);
                return JsonSerializerExtention.Serialize(new Response(null, tasks));

            }
            catch (Exception e)
            {
                return JsonSerializerExtention.Serialize(new Response(e.Message, null));
            }
        }

        /// <summary>
        /// This method adds a new task.
        /// </summary>
        /// <param name="email">Email of the user. The user must be logged in.</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="title">Title of the new task</param>
        /// <param name="description">Description of the new task</param>
        /// <param name="dueDate">The due date if the new task</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string AddTask(string email, string boardName, string title, string description, DateTime dueDate)
        {
            try
            {
                _userController.ValidateUser(email);
                _boardController.AddTask(email,boardName,title,description,dueDate);
                return JsonSerializerExtention.Serialize(new Response(null, null));

            }
            catch (Exception e)
            {
                return JsonSerializerExtention.Serialize(new Response(e.Message, null));
            }
        }
        
        /// <summary>
        /// This method advance task to the given column ordinal.
        /// </summary>
        /// <param name="email">Email of the user. The user must be logged in.</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column0rdinal to advance from</param>
        /// <param name="taskId">The id of the task</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string AdvanceTask(string email, string boardName, int columnOrdinal, int taskId)
        {
            try
            {
                _userController.ValidateUser(email);
                _boardController.AdvanceTask(email,boardName,columnOrdinal,taskId);
                return JsonSerializerExtention.Serialize(new Response(null, null));

            }
            catch (Exception e)
            {
                return JsonSerializerExtention.Serialize(new Response(e.Message, null));
            }
        }

        /// <summary>
        /// This method adds a new board to the given user.
        /// </summary>
        /// <param name="email">Email of the user. The user must be logged in.</param>
        /// <param name="boardName">The name of the new board. must be unique compared to the other boards of the user</param>
        /// <returns>Response The string "{}"", ,unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string AddBoard(string email, string boardName)
        {
            try
            {
                _userController.ValidateUser(email);
                _boardController.AddBoard(email,boardName);
                return JsonSerializerExtention.Serialize(new Response(null, null));

            }
            catch (Exception e)
            {
                return JsonSerializerExtention.Serialize(new Response(e.Message, null));
            }
        }

        /// <summary>
        /// This method removes the given board by it's name.
        /// </summary>
        /// <param name="email">Email of the user. The user must be logged in and the Owner of the Board.</param>
        /// <param name="boardName">The name of the new board.</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string RemoveBoard(string email, string boardName)
        {
            try
            {
                _userController.ValidateUser(email);
                _boardController.RemoveBoard(email,boardName);
                return JsonSerializerExtention.Serialize(new Response(null, null));

            }
            catch (Exception e)
            {
                return JsonSerializerExtention.Serialize(new Response(e.Message, null));
            }
        }

        /// <summary>
        /// This method returns all the tasks that are "In Progress" (in the middle column) to a user.
        /// </summary>
        /// <param name="email">Email of the user. The user must be logged in.</param>
        /// <returns>Response with a list that contains all the tasks that are in progress, ,unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string InProgressTasks(string email)
        {
            try
            {
                _userController.ValidateUser(email);
                List<Task> tasks =_boardController.InProgressTasks(email);
                return JsonSerializerExtention.Serialize(new Response(null, tasks));

            }
            catch (Exception e)
            {
                return JsonSerializerExtention.Serialize(new Response(e.Message, null));
            }
        }
        
         //****************** NEW FUNCTIONS ***************
        /// <summary>
        /// This method returns a list of IDs of all user's boards.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>A response with a list of IDs of all user's boards, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string GetUserBoards(string email)
        {
            try
            {
                _userController.ValidateUser(email);
                List<int> boards =_boardController.GetUserBoards(email);
                return JsonSerializerExtention.Serialize(new Response(null, boards));

            }
            catch (Exception e)
            {
                return JsonSerializerExtention.Serialize(new Response(e.Message, null));
            }
            
        }
        /// <summary>
        /// This method adds a user as member to an existing board.
        /// </summary>
        /// <param name="email">The email of the user that joins the board. Must be logged in</param>
        /// <param name="boardId">The board's ID</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string JoinBoard(string email, int boardId)
        {
            try
            {
                _userController.ValidateUser(email);
                _boardController.JoinBoard(email,boardId);
                return JsonSerializerExtention.Serialize(new Response(null, null));

            }
            catch (Exception e)
            {
                return JsonSerializerExtention.Serialize(new Response(e.Message, null));
            }
            
        }
        /// <summary>
        /// This method removes a user from the members list of a board.
        /// </summary>
        /// <param name="email">The email of the user. Must be logged in</param>
        /// <param name="boardId">The board's ID</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string LeaveBoard(string email, int boardId)
        {
            try
            {
                _userController.ValidateUser(email);
                _boardController.LeaveBoard(email,boardId);
                return JsonSerializerExtention.Serialize(new Response(null, null));

            }
            catch (Exception e)
            {
                return JsonSerializerExtention.Serialize(new Response(e.Message, null));
            }
        }

        /// <summary>
        /// This method assigns a task to a user
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column number. The first column is 0, the number increases by 1 for each column</param>
        /// <param name="taskID">The task to be updated identified a task ID</param>        
        /// <param name="emailAssignee">Email of the asignee user</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string AssignTask(string email, string boardName, int columnOrdinal, int taskID, string emailAssignee)
        {
            try
            {
                _userController.ValidateUser(email);
                _userController.ValidateUserExists(emailAssignee);
                _boardController.AssignTask(email,boardName,columnOrdinal,taskID,emailAssignee);
                return JsonSerializerExtention.Serialize(new Response(null, null));

            }
            catch (Exception e)
            {
                return JsonSerializerExtention.Serialize(new Response(e.Message, null));
            }
        }
        
        /// <summary>
        /// This method transfers a board ownership.
        /// </summary>
        /// <param name="currentOwnerEmail">Email of the current owner. Must be logged in</param>
        /// <param name="newOwnerEmail">Email of the new owner</param>
        /// <param name="boardName">The name of the board</param>
        /// <returns>An empty response, unless an error occurs (see <see cref="GradingService"/>)</returns>
        public string TransferOwnership(string currentOwnerEmail, string newOwnerEmail, string boardName)
        {
            try
            {
                _userController.ValidateUser(currentOwnerEmail);
                _userController.ValidateUserExists(newOwnerEmail);
                _boardController.TransferOwnerShip(currentOwnerEmail,newOwnerEmail,boardName);
                return JsonSerializerExtention.Serialize(new Response(null, null));

            }
            catch (Exception e)
            {
                return JsonSerializerExtention.Serialize(new Response(e.Message, null));
            }
            
        }

        /// <summary>
        /// This method gets a boards name by its ID
        /// </summary>
        /// <param name="boardId">boardId, must exist</param>
        /// <returns>BoardName if succeeded, if not messege error</returns>
        public string GetBoardName(int boardId)
        {
            try
            {
                string name= _boardController.GetBoardNameById(boardId);
                return JsonSerializerExtention.Serialize(new Response(null, name));

            }
            catch (Exception e)
            {
                return JsonSerializerExtention.Serialize(new Response(e.Message, null));
            }
        }
        
        /// <summary>
        /// This method returns List of tasks of board by its ID
        /// </summary>
        /// <param name="boardId">boardId, must exist</param>
        /// <returns>List<int columnOrdinal, <Task>> if succeeded, if not messege error</returns>
        public string GetColumn(int boardId, int columnOrdinal)
        {
            try
            {
                List<Task> list = _boardController.GetColumn(boardId, columnOrdinal);
                return JsonSerializerExtention.Serialize(new Response(null, list));

            }
            catch (Exception e)
            {
                return JsonSerializerExtention.Serialize(new Response(e.Message, null));
            }
        }
        
        /// <summary>
        /// This method gets the columns limit via boardId
        /// </summary>
        /// <param name="boardId">boardId, must exist</param>
        /// <param name="columnOrdinal">columnOrdinal</param>
        /// <returns>BoardName if succeeded, if not messege error</returns>
        public string GetColumnLimit(int boardId, int columnOrdinal)
        {
            try
            {
                int name= _boardController.GetColumnLimit(boardId, columnOrdinal);
                return JsonSerializerExtention.Serialize(new Response(null, name));

            }
            catch (Exception e)
            {
                return JsonSerializerExtention.Serialize(new Response(e.Message, null));
            }
        }



}