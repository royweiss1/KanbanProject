using System.IO;
using System.Reflection;
using System.Text.Json;
using IntroSE.Kanban.Backend.BusinessLayer;
using log4net;
using log4net.Config;

namespace IntroSE.Kanban.Backend.ServiceLayer;

public class FactoryService
{
    private BoardController _boardController = new BoardController();
    private UserController _userController = new UserController();
    public UserService UserService { get; }
    public  BoardService BoardService { get; }
    public TaskService TaskService { get; }
    public DataService DataService { get; }

    public FactoryService()
    {
        JsonSerializerExtention.DefaultSerializerSettings.WriteIndented = true;//when initalizing the settings for json, we set it as indented.
        UserService = new UserService(_userController);
        BoardService = new BoardService(_userController, _boardController);
        TaskService = new TaskService(_userController, _boardController);
        DataService = new DataService(new Loader(_userController, _boardController),
            new Deleter(_userController, _boardController));

    }
}