// See https://aka.ms/new-console-template for more information

using System.Reflection;
using IntroSE.Kanban.Backend.BusinessLayer;
using System.Text.Json;
using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Frontend;
using log4net;
using Task = System.Threading.Tasks.Task;
using System.Text.RegularExpressions;

    
    FactoryService fs = new FactoryService();
    fs.DataService.DeleteData();
    fs.UserService.Register("mail@mail.com", "Password1");
    fs.BoardService.AddBoard("mail@mail.com", "board1");
    fs.BoardService.AddBoard("mail@mail.com", "board2");
    fs.BoardService.AddTask("mail@mail.com", "board1", "task1", "desc", DateTime.MaxValue);
    fs.BoardService.AddTask("mail@mail.com", "board1", "task2", "desc", DateTime.MaxValue);
    fs.BoardService.AddTask("mail@mail.com", "board1", "task3", "desc", DateTime.MaxValue);
    fs.BoardService.AssignTask("mail@mail.com", "board1", 0, 0, "mail@mail.com");
    fs.BoardService.AssignTask("mail@mail.com", "board1", 0, 1, "mail@mail.com");
    fs.BoardService.AssignTask("mail@mail.com", "board1", 0, 2, "mail@mail.com");
    fs.BoardService.AdvanceTask("mail@mail.com", "board1", 0, 0);
    fs.BoardService.AdvanceTask("mail@mail.com", "board1", 1, 0);
    fs.BoardService.AdvanceTask("mail@mail.com", "board1", 0, 1);






/*
fs.UserService.Register("roy1@gmail.com", "Aa123456");
//fs.UserService.DeleteData();
//fs.BoardService.DeleteData();
//fs.BoardService.LoadData();
//fs.UserService.LoadData();
//fs.UserService.Login("roy1@gmail.com", "Aa123456");

fs.BoardService.AddBoard("roy1@gmail.com", "Board3");
fs.BoardService.AddBoard("roy1@gmail.com", "Board4");
fs.BoardService.AddTask("roy1@gmail.com", "Board3", "title1", "1", DateTime.MaxValue);
fs.BoardService.AddTask("roy1@gmail.com", "Board4", "title1", "2", DateTime.MaxValue);
//fs.BoardService.AddTask("roy1@gmail.com", "Board3", "title2", "3", DateTime.MaxValue);
fs.TaskService.UpdateTaskDescription("roy1@gmail.com", "Board4", 0, 0, "somesome");
fs.BoardService.LimitColumn("roy1@gmail.com", "Board3",0,5);

// fs.BoardService.LoadData();
fs.BoardService.AddTask("roy1@gmail.com", "Board3", "title2", "4", DateTime.MaxValue);

fs.BoardService.AddTask("roy1@gmail.com", "Board3", "title1", "10", DateTime.MaxValue);
fs.BoardService.AddTask("roy1@gmail.com", "Board3", "title3", "10", DateTime.MaxValue);
fs.BoardService.AddTask("roy1@gmail.com", "Board3", "title1", "10", DateTime.MaxValue);
fs.BoardService.AddTask("roy1@gmail.com", "Board3", "title1", "10", DateTime.MaxValue);
*/