using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.BusinessLayer;
using static IntroSE.Kanban.Backend.BusinessLayer.Constants;

namespace Frontend.Model
{
    public class BackendController
    {
        private FactoryService _factoryService;

        public BackendController()
        {
            _factoryService = BackendControllerFactory.GetFactory();
        }
        
        public UserModel Login(string username, string password)
        {
            Response user = JsonSerializer.Deserialize<Response>(_factoryService.UserService.Login(username, password));
            if (user.ErrorOccured)
            {
                throw new Exception(user.ErrorMessage);
            }
            return new UserModel(this, username);
        }
        
        internal UserModel Register(string username, string password)
        {
            Response reg = JsonSerializer.Deserialize<Response>(_factoryService.UserService.Register(username, password));
            if (reg.ErrorOccured)
            {
                throw new Exception(reg.ErrorMessage);
            }

            return new UserModel(this, username);
        }

        internal List<BoardModel> GetBoardsOfUser(UserModel user)
        {
            Response reg = JsonSerializer.Deserialize<Response>(_factoryService.BoardService.GetUserBoards(user.Email));
            if (reg.ErrorOccured)
            {
                throw new Exception(reg.ErrorMessage);
            }
            Object obj = ToObject<List<int>>((JsonElement)reg.ReturnValue);
            List<int> boardsIDS = (List<int>) obj;
            List<BoardModel> boardModelsOfUser = new List<BoardModel>();
            foreach (int t in boardsIDS)
            {
                boardModelsOfUser.Add(new BoardModel(this, t));
            }

            return boardModelsOfUser;
        }

        internal string GetBoardName(int boardId)
        {
            Response reg = JsonSerializer.Deserialize<Response>(_factoryService.BoardService.GetBoardName(boardId));
            if (reg.ErrorOccured)
            {
                throw new Exception(reg.ErrorMessage);
            }
            Object obj = ToObject<string>((JsonElement)reg.ReturnValue);
            return obj.ToString();
        }

        public List<TaskModel> GetColumn(int boardId, int columnOrdinal)
        {
            Response reg = JsonSerializer.Deserialize<Response>(_factoryService.BoardService.GetColumn(boardId, columnOrdinal));
            if (reg.ErrorOccured)
            {
                throw new Exception(reg.ErrorMessage);
            }
            Object obj = ToObject<List<Task>>((JsonElement)reg.ReturnValue); //TODO: BIG problem here
            List<Task> column = (List<Task>)obj;
            List<TaskModel> toReturn = new List<TaskModel>();
            foreach (Task t in column)
            { 
                toReturn.Add(new TaskModel(t.Id,t.Title,t.Description,t.Assignee,t.DueDate,t.CreationTime, this));
            }
            return toReturn;
        }

        public int GetColumnLimit(int boardId, int columnOrdinal)
        {
            Response reg = JsonSerializer.Deserialize<Response>(_factoryService.BoardService.GetColumnLimit(boardId, columnOrdinal));
            if (reg.ErrorOccured)
            {
                throw new Exception(reg.ErrorMessage);
            }
            Object obj = ToObject<int>((JsonElement)reg.ReturnValue);
            return (int)obj;
        }
        
        private static T ToObject<T>(JsonElement element)
        {
            var json = element.GetRawText();
            return JsonSerializer.Deserialize<T>(json);
        }
    }
    
    
}
