using System;
using System.Collections.Generic;
using static IntroSE.Kanban.Backend.BusinessLayer.Constants;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOs
{

    class BoardUserDTO : DTO
    {
        internal const string BoardIdName = "BoardId";
        internal const string BoardEmailName = "Email";
        internal int BoardId { get; set; } // FK of Board

        internal string Email { get; set; } // FK of user
        
        internal BoardUserDTO(int boardId, string email) : base(new BoardUserMapper())
        {
            BoardId = boardId;
            Email = email;
        }

        internal void JoinBoard()
        {
            new BoardUserMapper().JoinBoard(this); //join board in board user
        }

        internal void LeaveBoard()
        {
            new BoardUserMapper().LeaveBoard(this);
            List<TaskDTO> list = new TaskDTOMapper().GetTaskDTOsOfBoardUser(BoardId,Email); // un-assigning task
            foreach (var taskDto in list)
            {
                taskDto.UnAssignTask(); //unassignes the task
                
            }
        }
        
        public override void Persist()
        {
            new BoardUserMapper().AddBoardUser(this); //adds to board user
            log.Info($"BoardUser id: {BoardId}, User:{Email} has been added to DB");

        }

        internal void DeleteBoardUser()
        {
            new BoardUserMapper().Delete(this); //deletes from board user
            log.Info($"BoardUser id: {BoardId}, User:{Email} has been deleted from DB");
        }
    }
}