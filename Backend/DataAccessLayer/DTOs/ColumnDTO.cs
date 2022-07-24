using System;
using System.Collections.Generic;
using IntroSE.Kanban.Backend.BusinessLayer;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOs
{

    public class ColumnDTO : DTO
    {
        public const string ColumnBoardIdName = "BoardId";
        public const string ColumnColumnOrdinalName = "ColumnOrdinal";
        public const string ColumnColumnLimitName = "ColumnLimit";
        
        public int BoardId { get; } // FK of Board

        public int ColumnOrdinal { get; } //0,1,2

        private int columnLimit;

        public int ColumnLimit { get=>columnLimit;
            private set //may use "LimitColumn" method
            { 
                new ColumnMapper().UpdateLimit(this,value);
                columnLimit = value;
            }
        }

        public ColumnDTO(int boardId, int columnOrdinal, int columnLimit) : base(new ColumnMapper())
        {
            BoardId = boardId;
            ColumnOrdinal = columnOrdinal;
            this.columnLimit = columnLimit;
        }

        public void LimitColumn(int limit)
        {
            ColumnLimit = limit; //takes care of update
            log.Info($"Column {ColumnOrdinal} in Board: {BoardId} has updated its columnLimit to {columnLimit}");
        }

        public override void Persist()
        {
            new ColumnMapper().AddColumn(this); //adds coulumn
        }

        // this function returns list of task dto's by baordId and column ordinal.
        public List<TaskDTO> GetTaskDtos()
        {
            return new TaskDTOMapper().GetTaskDtos(BoardId, ColumnOrdinal); //return all task DTOs

        }
    }
}