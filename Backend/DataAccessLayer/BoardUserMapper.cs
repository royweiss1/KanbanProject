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
using IntroSE.Kanban.Backend.ServiceLayer;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{

    class BoardUserMapper : DalMapper
    {
        private const string BoardUserTableName = "BoardUser";
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public BoardUserMapper() : base(BoardUserTableName)
        {
            // Load configuration
            //Right click on log4net.config file and choose Properties. 
            //Then change option under Copy to Output Directory build action into Copy if newer or Copy always.
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }
        protected override BoardUserDTO ConvertReaderToObject(SQLiteDataReader reader)
        {
            BoardUserDTO result = new BoardUserDTO(reader.GetInt32(0), reader.GetString(1));
            return result;
        }

        public void JoinBoard(BoardUserDTO boardUser)
        {
            if (!Insert(boardUser)) //checks if insert went well
                throw new DataException(
                    $"tried to add user:{boardUser.Email} and board: {boardUser.BoardId} to userboards table but failed");

        }

        public void LeaveBoard(BoardUserDTO boardUser)
        {
            // if leaves => is not the owner
           Delete(boardUser);
            //log info
        }

        public void AddBoardUser(BoardUserDTO bu)
        {
            if(!Insert(bu)) //check if insert went well
                throw new DataException($"board with id {bu.BoardId} was NOT persisted properly to UserBoards table");

        }

        private bool Insert(BoardUserDTO boardUser)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                int res = -1;
                try
                {
                    connection.Open();
                    command.CommandText = $"INSERT INTO {BoardUserTableName} ({BoardUserDTO.BoardIdName} ,{BoardUserDTO.BoardEmailName}) " +
                                          $"VALUES (@idVal,@emailVal);"; //the insert command

                    //the parametrs
                    SQLiteParameter boardIdParam = new SQLiteParameter(@"idVal", boardUser.BoardId);
                    SQLiteParameter emailParam = new SQLiteParameter(@"emailVal", boardUser.Email);

                    command.Parameters.Add(boardIdParam);
                    command.Parameters.Add(emailParam);
                    command.Prepare();
                    res = command.ExecuteNonQuery();
                    
                    log.Info("boardUser with BoardId: "+boardUser.BoardId+" and Email: "+boardUser.Email+" has been inserted");
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

        internal List<BoardUserDTO> LoadAllBoardUserDtos()
        {
            List<BoardUserDTO> result = Select().Cast<BoardUserDTO>().ToList(); //recieve all BoarUserDtos
            return result;
        }
        public List<BoardUserDTO> SelectByBoardId(int boardId)
        {
            List<BoardUserDTO> results = new List<BoardUserDTO>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText =
                    $"select * from {BoardUserTableName} where {BoardUserDTO.BoardIdName}={boardId}"; //select board by id
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
                    connection.Close(); //dont forget to close connection
                }

            }

            return results;
        }

        public Dictionary<string , List<BoardDTO>> LoadBoardsPerUsers()
        {
            List<BoardUserDTO> boardUserDtos = LoadAllBoardUserDtos(); //load all board users
            Dictionary<string,List<BoardDTO>> userBoards = new Dictionary<string, List<BoardDTO>>();
            
            UserDTOMapper userDtoMapper = new UserDTOMapper();
            BoardDTOMapper boardDtoMapper = new BoardDTOMapper();
            foreach (var boardUserDto in boardUserDtos)
            {
                UserDTO user = userDtoMapper.SelectUserByEmail(boardUserDto.Email); //create new user dto by email
                string email = user.Email;
                if (!userBoards.ContainsKey(email)) //check if user boards contains such email
                {
                    userBoards.Add(email, new List<BoardDTO>()); //create new field in userboards with this email
                }
                userBoards[email].Add(boardDtoMapper.SelectBoardById(boardUserDto.BoardId)); //add this tp user board 
            }

            return userBoards;
        }

        internal void Delete(BoardUserDTO bu)
        {
            if (!RunDeleteQuery(bu)) //check if query went well
                throw new DataException($"row with boardId: {bu.BoardId}, email: {bu.Email} was not deleted properly in userboards table");


        }
        internal bool RunDeleteQuery(BoardUserDTO bu)
        {
            int res = -1;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"delete from {BoardUserTableName} where {BoardUserDTO.BoardIdName}={bu.BoardId} and {BoardUserDTO.BoardEmailName}='{bu.Email}'" //command to delete board user
                };
                try
                {
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    
                    log.Info("boardUser with BoardId: "+bu.BoardId+" and Email: "+bu.Email+" has been deleted");
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }

            }

            return res > 0;
        }
        

    }
}