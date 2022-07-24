using System;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using IntroSE.Kanban.Backend.BusinessLayer;
using log4net;
using System.Reflection;
using log4net.Config;
using System.IO;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{

    class ColumnMapper : DalMapper
    {
        private const string ColumnTableName = "Columns";
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public ColumnMapper(): base(ColumnTableName)
        {
            // Load configuration
            //Right click on log4net.config file and choose Properties. 
            //Then change option under Copy to Output Directory build action into Copy if newer or Copy always.
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }
        protected override ColumnDTO ConvertReaderToObject(SQLiteDataReader reader)
        {
            ColumnDTO result = new ColumnDTO(reader.GetInt32(0), reader.GetInt32(1),reader.GetInt32(2) );
            return result;

        }
        
        internal List<ColumnDTO> GetColumns(int boardId)
        {
            List<ColumnDTO> results = new List<ColumnDTO>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"select * from {ColumnTableName} where {ColumnDTO.ColumnBoardIdName}={boardId};"; //command to recieve columns 
                SQLiteDataReader dataReader = null;
                try
                {
                    connection.Open();
                    dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        results.Add(ConvertReaderToObject(dataReader));

                    }
                }
                finally
                {
                    if (dataReader != null)
                    {
                        dataReader.Close();
                    }

                    command.Dispose();
                    connection.Close();
                }

            }

            return results;
        }

        public void AddColumn(ColumnDTO column)
        { 
            if(!Insert(column)) //check if insert went well
                throw new DataException($"column of board: {column.BoardId} with ordinal {column.ColumnOrdinal} was NOT persisted properly into columns tables");

        }
        
        private bool Insert(ColumnDTO column)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                int res = -1;
                try
                {
                    connection.Open();
                    command.CommandText = $"INSERT INTO {ColumnTableName} ({ColumnDTO.ColumnBoardIdName} ,{ColumnDTO.ColumnColumnOrdinalName},{ColumnDTO.ColumnColumnLimitName}) " +
                                          $"VALUES (@idVal,@ordinalVal,@limitVal);"; //command to insert

                    //paramters
                    SQLiteParameter idParam = new SQLiteParameter(@"idVal", column.BoardId);
                    SQLiteParameter ordinalParam = new SQLiteParameter(@"ordinalVal", column.ColumnOrdinal);
                    SQLiteParameter limitParam = new SQLiteParameter(@"limitVal", column.ColumnLimit);


                    command.Parameters.Add(idParam);
                    command.Parameters.Add(ordinalParam);
                    command.Parameters.Add(limitParam);
                    command.Prepare();
                    res = command.ExecuteNonQuery();
                    
                    log.Info("Column with BoardId: "+column.BoardId+" and ordinal: "+column.ColumnOrdinal+" has been inserted");
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close(); //dont forget to close

                }
                return res > 0;
            }
        }

        public void UpdateLimit(ColumnDTO columnDto, int limit)
        {
            if(!RunUpdateLimitQuery(columnDto,limit)) //check that update limit went well
                throw new DataException($"column in board {columnDto.BoardId}, ordinal {columnDto.ColumnOrdinal} was NOT persisted properly");

        }
        private bool RunUpdateLimitQuery(ColumnDTO columnDto, int limit)
        {
            string attributeName = ColumnDTO.ColumnColumnLimitName;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                int res = -1;
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText =
                        $"update {ColumnTableName} set [{attributeName}]=@{attributeName} where {ColumnDTO.ColumnBoardIdName}={columnDto.BoardId} and {ColumnDTO.ColumnColumnOrdinalName}={columnDto.ColumnOrdinal}"
                    //command to update limit
                };
                try
                {
                    command.Parameters.Add(new SQLiteParameter(attributeName, limit));
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close(); //dont forget to close connection
                }
                return res > 0;
            }

        }
    }
}