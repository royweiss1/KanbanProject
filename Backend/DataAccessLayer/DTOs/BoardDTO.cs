using System;
using System.Collections.Generic;
using static IntroSE.Kanban.Backend.BusinessLayer.Constants;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOs
{

    public class BoardDTO : DTO
    {
        public const string BoardIdName = "BoardId";
        public const string BoardNameName = "BoardName";
        public const string BoardOwnerEmailName = "OwnerEmail";
        public const string BoardTaskIncrementName = "TaskIncrement";

        private string ownerEmail;

        public string OwnerEmail
        {
            get => ownerEmail;
            set
            {
                _mapper.Update(BoardId, BoardIdName, BoardOwnerEmailName, value); //update in DB
                ownerEmail = value; //if valid update the value here too
            }
        }

        private int taskIncrement;

        public int TaskIncrement
        {
            get => taskIncrement;
            private set //may only use append to update the value
            {
                _mapper.Update(BoardId, BoardIdName, BoardTaskIncrementName, value.ToString());
                taskIncrement=value; // need always to make sure it is plus 1 to its value when using
            }
        }

        public int BoardId { get; set; }

        private string name;

        public string Name
        {
            get => name;
            set //currently may not change board name, therefore no use
            {
                _mapper.Update(BoardId,BoardIdName, BoardNameName, value); //update in DB
                name = value; //if valid update here too
            }
        }

        public BoardDTO(int boardId, string name, string ownerEmail, int taskIncrement) : base(new BoardDTOMapper())
        {
            BoardId = boardId;
            this.name = name;
            this.ownerEmail = ownerEmail;
            this.taskIncrement = taskIncrement; // when a board is created has no tasks
        }

        public override void Persist()
        {
            new BoardDTOMapper().AddBoard(this); //adds this board 
            BoardUserDTO bu = new BoardUserDTO(BoardId, ownerEmail);
            bu.Persist(); // adds board to user
            log.Info($"Board {BoardId} has been added for the first time to DB");
        }

        public void JoinBoard(string email, int boardId)
        {
            new BoardUserDTO(boardId, email).JoinBoard(); //kinda the persist
            log.Info($"User: {email} has joined the board {boardId}");
        }

        public void LeaveBoard(string email, int boardId)
        {
            new BoardUserDTO(boardId, email).LeaveBoard(); //leave board
            log.Info($"User: {email} has left the board {boardId}");

        }

        public void TransferOwnership(string newOwnerEmail)
        {
            OwnerEmail = newOwnerEmail; //takes care of update in BoardDTO
            log.Info($"User: {newOwnerEmail} is now the Owner of the board {BoardId}");

        }

        public void DeleteBoard()
        {
            List<BoardUserDTO> listBoardUser = new BoardUserMapper().SelectByBoardId(BoardId); // list of memebers
            foreach (BoardUserDTO bu in listBoardUser) //for each member need to delete
            {
                bu.DeleteBoardUser(); //delete from board user
            }

            new ColumnMapper().Delete(BoardId,ColumnDTO.ColumnBoardIdName); //deleting columns
            
            _mapper.Delete(BoardId,BoardIdName); //delete this board
            log.Info($"board {BoardId} has been deleted from DB");
        }

        public void AppendTaskCounter()
        {
            TaskIncrement = taskIncrement + 1;
            log.Info($"board {BoardId} has appended its taskIncrement");

        }

        public List<ColumnDTO> GetColumnDtos()
        {
            return new ColumnMapper().GetColumns(BoardId); // creating somewhat of column to use its functunality
        }
    }
}