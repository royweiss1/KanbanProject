using System;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using log4net;
using System.Reflection;
using log4net.Config;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{

    public abstract class DalMapper
    {
        protected readonly string _connectionString;
        
        private readonly string _tableName;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        internal DalMapper(string tableName)
        {
            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "kanban.db"));
            _connectionString = $"Data Source={path}; Version=3;";
            _tableName = tableName;
            
            // Load configuration
            //Right click on log4net.config file and choose Properties. 
            //Then change option under Copy to Output Directory build action into Copy if newer or Copy always.
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }

        protected abstract DTO ConvertReaderToObject(SQLiteDataReader reader);


        public void Update(int id,string IdColumnName ,string attributeName, string attributeValue)
        {
            if (!RunUpdateQuery(id, IdColumnName, attributeName, attributeValue)) //check if update went well
                throw new DataException(
                    $"trid to update {attributeName} in {_tableName} where {IdColumnName} = {id} but failed");
        }
        //update with one PK
        private bool RunUpdateQuery(int id,string IdColumnName ,string attributeName, string attributeValue) // maybe id will be obj
        {
            int res = -1;
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"update {_tableName} set [{attributeName}]=@{attributeName} where {IdColumnName}={id}"
                    //command to update 
                };
                try
                {
                    command.Parameters.Add(new SQLiteParameter(attributeName, attributeValue));
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
                    connection.Close(); //donnt forget to close
                }

            }

            return res > 0;
        }
        
        protected List<DTO> Select()
        {
            List<DTO> results = new List<DTO>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"select * from {_tableName};"; //select from table that is given
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
                    connection.Close(); //dont forget to close
                }

            }

            return results;
        }

        public void Delete(int id, string pk)
        {
            if (!RunDeleteQuery(id, pk)) //check if delete went well
                throw new Exception($"deletion from {_tableName} didnt succeed. tried to id: {id} by {pk} ");

        }
        private bool RunDeleteQuery(int ID, string PK)
        {
            int res = -1;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"delete from {_tableName} where {PK}={ID}" //dekete what needs to be deleted
                };
                try
                {
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    log.Info(PK+": "+ID+" has been deleted");
                }
                finally
                {
                    command.Dispose();
                    connection.Close(); //dont forget to close
                }

            }

            return res > 0;
        }

        public void DeleteAll()
        {
            if (!RunDeleteAllQuery()) //check if delete query went well
                throw new DataException($"failed to delete all data from {_tableName}");

        }
        public bool RunDeleteAllQuery()
        {
            int res = -1;

            using (var connection = new SQLiteConnection(_connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"delete from {_tableName}" 
                };
                try
                {
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                finally
                {
                    command.Dispose();
                    connection.Close(); //dont forget to close
                }

            }

            return res >= 0;
        }
        
    }
}