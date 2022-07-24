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

    class TaskDTOMapper : DalMapper
    {
        private const string TaskTableName = "Tasks";
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        public TaskDTOMapper() : base(TaskTableName)
        {
            // Load configuration
            //Right click on log4net.config file and choose Properties. 
            //Then change option under Copy to Output Directory build action into Copy if newer or Copy always.
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }
        

        internal List<TaskDTO> GetTaskDTOsOfBoardUser(int boardId,string email)
        {
            List<TaskDTO> list = SelectByUser(boardId, email);
            return list;
        }
        
        private List<TaskDTO> SelectByUser(int boardId,string email)
        {
            List<TaskDTO> results = new List<TaskDTO>();
            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText =
                    $"select * from {TaskTableName} where {TaskDTO.TaskBoardIdName}={boardId} and {TaskDTO.TaskAssigneeName}='{email}'"; //select all tasks from board with given email
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

        protected override TaskDTO ConvertReaderToObject(SQLiteDataReader reader)
        {
            TaskDTO result = new TaskDTO(reader.GetInt32(0), reader.GetInt32(1), DateTime.Parse(reader.GetString(2)), 
                DateTime.Parse(reader.GetString(3)), 
                reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetInt32(7));
            return result;

        }

        public void Update(int boardId, int taskId, string attributeName, object attributeValue)
        {
            if (!RunUpdateQuery(boardId, taskId, attributeName, attributeValue)) //try to run query
            {
                throw new DataException(
                    $"task's {attributeName} in board: {boardId} with id: {taskId} was not updated properly.");
            }
        }
        private bool RunUpdateQuery(int boardId, int taskId, string attributeName, object attributeValue)
        {
            if (attributeValue != null) //may be null, for example: unAssigned or description
            {
                if (attributeValue is string) // string option needs to be added to class constants.
                    attributeValue = (string)attributeValue;
                else
                    attributeValue = (int)attributeValue;
            }

            using (var connection = new SQLiteConnection(_connectionString))
            {
                int res = -1;
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText =
                        $"update {TaskTableName} set [{attributeName}]=@{attributeName} where {TaskDTO.TaskBoardIdName}={boardId} and {TaskDTO.TaskIdName}={taskId}"
                    //try to update task
                };
                try
                {
                    command.Parameters.Add(new SQLiteParameter(attributeName, attributeValue));
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    log.Info("Task boardId: "+boardId+" TaskId: "+taskId+" has updated its "+attributeName);
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close(); 
                }

                return res > 0;

            }
        }

        public void AddTask(TaskDTO taskDto)
        {
            if(!Insert(taskDto)) //check if insert went well
                throw new DataException($"task id:{taskDto.TaskId}, title:{taskDto.Title} in board with id {taskDto.BoardId} was NOT persisted properly into tasks table");

            
        }

        private bool Insert(TaskDTO taskDto)
        {

            using (var connection = new SQLiteConnection(_connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                int res = -1;

                try
                {
                    connection.Open();
                    command.CommandText =
                        $"INSERT INTO {TaskTableName} ({TaskDTO.TaskBoardIdName} ,{TaskDTO.TaskIdName}, {TaskDTO.TaskCreationDateName}, {TaskDTO.TaskDueDateName}, {TaskDTO.TaskTitleName}, {TaskDTO.TaskDescriptionName}, {TaskDTO.TaskAssigneeName}, {TaskDTO.TaskColumnOrdinalName}) " +
                        $"VALUES (@BoardId,@TaskId,@CreationDate,@DueDate,@Title,@Description,@Assignee,@ColumnOrdinal);"; //commad to insert task
                    
                    SQLiteParameter boardIdParam = new SQLiteParameter(@"BoardId", taskDto.BoardId);
                    SQLiteParameter taskIdParam = new SQLiteParameter(@"TaskId", taskDto.TaskId);
                    SQLiteParameter creationDateParam = new SQLiteParameter(@"CreationDate", taskDto.CreationDate.ToString());
                    SQLiteParameter dueDateParam = new SQLiteParameter(@"DueDate", taskDto.DueDate.ToString());
                    SQLiteParameter titleParam = new SQLiteParameter(@"Title", taskDto.Title);
                    SQLiteParameter descriptionParam = new SQLiteParameter(@"Description", taskDto.Description);
                    SQLiteParameter assigneeParam = new SQLiteParameter(@"Assignee", taskDto.Assignee);
                    SQLiteParameter columnOrdinalParam = new SQLiteParameter(@"ColumnOrdinal", taskDto.ColumnOrdinal);
                    
                    command.Parameters.Add(boardIdParam);
                    command.Parameters.Add(taskIdParam);
                    command.Parameters.Add(creationDateParam);
                    command.Parameters.Add(dueDateParam);
                    command.Parameters.Add(titleParam);
                    command.Parameters.Add(descriptionParam);
                    command.Parameters.Add(assigneeParam);
                    command.Parameters.Add(columnOrdinalParam);
                    
                    command.Prepare();
                    res = command.ExecuteNonQuery();
                    
                    log.Info("Task boardId: "+taskDto.BoardId+" TaskId: "+taskDto.TaskId+" has been inserted");
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();//dont forget to close
                }
                return res > 0;
            }
        }

        public List<TaskDTO> GetTaskDtos(int boardId, int columnOrdinal)
        {
            List<TaskDTO> results = new List<TaskDTO>();
                using (var connection = new SQLiteConnection(_connectionString))
                {
                    SQLiteCommand command = new SQLiteCommand(null, connection);
                    command.CommandText = $"select * from {TaskTableName} where {TaskDTO.TaskBoardIdName} = {boardId} and {TaskDTO.TaskColumnOrdinalName} = {columnOrdinal}";
                    //command to get all tasks
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
    }
}