using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.BusinessLayer;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;
using log4net;
using System.Reflection;
using log4net.Config;
using System.IO;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    class UserDTOMapper : DalMapper
    {
        private const string UserTableName = "Users";
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public UserDTOMapper() : base(UserTableName)
        {
            // Load configuration
            //Right click on log4net.config file and choose Properties. 
            //Then change option under Copy to Output Directory build action into Copy if newer or Copy always.
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }
        protected override UserDTO ConvertReaderToObject(SQLiteDataReader reader)
        {
            UserDTO result = new UserDTO(reader.GetString(0), reader.GetString(1));
            return result;
        }
        
        internal List<UserDTO> LoadAllUserDtos()
        {
            List<UserDTO> result = Select().Cast<UserDTO>().ToList(); //load all user dtos
            return result;
        }
        
        public void Register(UserDTO user)
        {
            if (!Insert(user)) //check if insert went well
            {
                throw new DataException($"user: {user.Email}  was NOT persisted properly into users table");

            }
        }

        private bool Insert(UserDTO user)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                int res = -1;
                try
                {
                    connection.Open();
                    command.CommandText = $"INSERT INTO {UserTableName} ({UserDTO.UserEmailName} ,{UserDTO.UserPasswordName}) " +
                                          $"VALUES (@emailVal,@passwordVal);";
                    //command to insert user

                    SQLiteParameter emailParam = new SQLiteParameter(@"emailVal", user.Email);
                    SQLiteParameter passwordParam = new SQLiteParameter(@"passwordVal", user.Password);

                    command.Parameters.Add(emailParam);
                    command.Parameters.Add(passwordParam);
                    command.Prepare();
                    res = command.ExecuteNonQuery();
                    
                    log.Info("User: "+user.Email+" had been registered in the DB");
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
        internal UserDTO SelectUserByEmail(string email)
        {
            UserDTO user;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText =
                    $"select * from {UserTableName} where {UserDTO.UserEmailName}='{email}'";
                //command return user with given email
                SQLiteDataReader dataReader = null;
                try
                {
                    connection.Open();
                    dataReader = command.ExecuteReader();
                    dataReader.Read();
                    user = ConvertReaderToObject(dataReader);
                    
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

            return user;
        }

    }
}