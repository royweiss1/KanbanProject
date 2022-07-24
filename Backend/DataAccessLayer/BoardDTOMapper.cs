using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;
using log4net;
using System.Reflection;
using log4net.Config;
using System.IO;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    class BoardDTOMapper: DalMapper
    {
        private const string BoardTableName = "Boards";
        
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public BoardDTOMapper(): base(BoardTableName)
        {
            // Load configuration
            //Right click on log4net.config file and choose Properties. 
            //Then change option under Copy to Output Directory build action into Copy if newer or Copy always.
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }

        public void AddBoard(BoardDTO board)
        {
            if (!Insert(board)) //check if insert went well
                throw new DataException($"board: {board.Name} with id {board.BoardId} was NOT persisted properly into boards table");
        }

        protected override BoardDTO ConvertReaderToObject(SQLiteDataReader reader)
        {
            BoardDTO result = new BoardDTO(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetInt32(3)); 
            return result;
        }

        private bool Insert(BoardDTO board)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                int res = -1;
                try
                {
                    connection.Open(); //open conenction
                    command.CommandText = $"INSERT INTO {BoardTableName} ({BoardDTO.BoardIdName} ,{BoardDTO.BoardNameName},{BoardDTO.BoardOwnerEmailName},{BoardDTO.BoardTaskIncrementName}) " +
                                          $"VALUES (@idVal,@nameVal,@ownerEmailVal,@taskIncVal);"; //the command text
                    
                    //parameters
                    SQLiteParameter idParam = new SQLiteParameter(@"idVal", board.BoardId);
                    SQLiteParameter nameParam = new SQLiteParameter(@"nameVal", board.Name);
                    SQLiteParameter ownerEmailParam = new SQLiteParameter(@"ownerEmailVal", board.OwnerEmail);
                    SQLiteParameter taskIncParam = new SQLiteParameter(@"taskIncVal", board.TaskIncrement);

                    //add parameters to command
                    command.Parameters.Add(idParam);
                    command.Parameters.Add(nameParam);
                    command.Parameters.Add(ownerEmailParam);
                    command.Parameters.Add(taskIncParam);

                    command.Prepare();
                    res = command.ExecuteNonQuery(); //execute command
                    
                    log.Info("BoardDTO id: "+board.BoardId+"has been inserted");
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
        
        internal List<BoardDTO> LoadAllBoardDtos()
        {
            List<BoardDTO> result = Select().Cast<BoardDTO>().ToList(); //returns all board DTOs
            return result;
        }
        internal BoardDTO SelectBoardById(int boardId)
        {
            BoardDTO board;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText =
                    $"select * from {BoardTableName} where {BoardDTO.BoardIdName}={boardId}"; //command to select board by id
                SQLiteDataReader dataReader = null;
                try
                {
                    connection.Open();
                    dataReader = command.ExecuteReader();
                    dataReader.Read();
                    board = ConvertReaderToObject(dataReader);
                    
                }
                finally
                {
                    if (dataReader != null)
                    {
                        dataReader.Close();
                    }

                    command.Dispose();
                    connection.Close(); //dont forget to close
                }

            }

            return board;
        }

        

    }
}