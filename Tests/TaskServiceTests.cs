using System.Reflection;
using System.Text.Json;
using IntroSE.Kanban.Backend.BusinessLayer;
using IntroSE.Kanban.Backend.ServiceLayer;
using log4net;
using log4net.Config;
using System.IO;

namespace IntroSE.Kanban.Frontend;

public class TaskServiceTests
{
    
    private TaskService _ts;
    private BoardService _bs;
    private UserService _us;
    private DataService _ds;

    private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    public TaskServiceTests()
    {
        FactoryService fs = new FactoryService();
        _bs = fs.BoardService;
        _us = fs.UserService;
        _ds = fs.DataService;
        _ts = fs.TaskService;

    }
    
    
    //for the tests to work we will call functions like addTask before we test, but that will happen later in implementation.
    public Boolean RunTests()
    {
        return TestUpdateTaskTitle() & TestUpdateTaskDueDate() & TestUpdateTaskDescription();
    }
    
    
    /// <summary>
    /// in this test we will test the capabilities to update task's due date
    /// THE TESTS:
    /// <para/>
    /// 1. trying to change the date of a task that have not been finished yet (should pass)
    /// <para/>
    /// 2. trying to change the date of a task that has been finished (should fail)
    /// <para/>
    /// 3. trying to change the date of a task that does not exist.
    /// </summary>
    /// <returns>True if all tests got favarable results.</returns>

    private bool TestUpdateTaskDueDate()
    {
        _ds.DeleteData();
        _us.Register("test@gmail.com", "Aa123456");
        _bs.AddBoard("test@gmail.com", "testBoard1");
        _bs.AddTask("test@gmail.com", "testBoard1", "Tid0Column0", "something", new DateTime(2023, 5, 17));
        _bs.AddTask("test@gmail.com", "testBoard1", "Tid1Column0", "something", new DateTime(2023, 5, 19));
        _bs.AddTask("test@gmail.com", "testBoard1", "Tid2Column2", "something", new DateTime(2023, 5, 19));
        //only Assignee may update tasks features
        _bs.AssignTask("test@gmail.com", "testBoard1", 0, 2, "test@gmail.com");
        _bs.AssignTask("test@gmail.com", "testBoard1", 0, 1, "test@gmail.com");
        _bs.AssignTask("test@gmail.com", "testBoard1", 0, 0, "test@gmail.com");

        _bs.AdvanceTask("test@gmail.com", "testBoard1", 0, 2);
        _bs.AdvanceTask("test@gmail.com", "testBoard1", 1, 2);
        
        String check1 = _ts.UpdateTaskDueDate("test@gmail.com","testBoard1",0,1 ,DateTime.MaxValue); // should pass
        String check2 = _ts.UpdateTaskDueDate("test@gmail.com","testBoard1",2,2, DateTime.MaxValue); // should fail-"DONE"
        String check3 = _ts.UpdateTaskDueDate("test@gmail.com","testBoard1",0,1, DateTime.MinValue); // should fail

        Response? r1 = JsonSerializer.Deserialize<Response>(check1);
        Response? r2 = JsonSerializer.Deserialize<Response>(check2);
        Response? r3 = JsonSerializer.Deserialize<Response>(check3);

        if (r1.ErrorOccured)
        {
            log.Fatal("first update date has a problem");
            return false;
        }
        if (!r2.ErrorOccured)
        {
            log.Fatal("second update date has a problem, it should return an errormessage");
            return false;
        }
        if (!r3.ErrorOccured)
        {
            log.Fatal("third update has date a problem, it should return an error message");
            return false;
        }

        log.Debug("finished TestUpdateTaskDueDate");
        
        

        return true;

    }


    /// <summary>
    /// in this test we will test the capebilities to update task's title
    /// THE TESTS:
    /// <para/>
    /// 1. trying to change the title of a task that have not been finished yet (should pass)
    /// <para/>
    /// 2. trying to change the title of a task that has been finished (should fail)
    /// <para/>
    /// 3. trying to change the title of a task that have not been finished yet with a title longer then 50 charecters (should fail)
    /// <para/>
    /// 4. trying to change the title of a task that has been finished to null (should fail)
    /// <para/>
    /// 5. trying to change title of task that does not exist
    /// </summary>
    /// <returns>True if all tests got favarable results.</returns>
    private bool TestUpdateTaskTitle()
    {
        _ds.DeleteData();
        _us.Register("test@gmail.com", "Aa123456");
        _bs.AddBoard("test@gmail.com", "testBoard1");
        _bs.AddTask("test@gmail.com", "testBoard1", "Tid0Column0", "something", new DateTime(2023, 5, 17));
        _bs.AddTask("test@gmail.com", "testBoard1", "Tid1Column0", "something", new DateTime(2023, 5, 19));
        _bs.AddTask("test@gmail.com", "testBoard1", "Tid2Column2", "something", new DateTime(2023, 5, 19));
        //only Assignee may update tasks features
        _bs.AssignTask("test@gmail.com", "testBoard1", 0, 2, "test@gmail.com");
        _bs.AssignTask("test@gmail.com", "testBoard1", 0, 1, "test@gmail.com");
        _bs.AssignTask("test@gmail.com", "testBoard1", 0, 0, "test@gmail.com");
        _bs.AdvanceTask("test@gmail.com", "testBoard1", 0, 2);
        _bs.AdvanceTask("test@gmail.com", "testBoard1", 1, 2);

        
        String check1 = _ts.UpdateTaskTitle( "test@gmail.com","testBoard1",0,1, "Wiz khalifa");
        String check2 = _ts.UpdateTaskTitle("test@gmail.com","testBoard1",2,2, "water"); 
        String check3 = _ts.UpdateTaskTitle( "test@gmail.com","testBoard1",0,1, "sdjfnsdjfjkdsgjsdfjkgjsdfjgsdfjgjnsdfjkgjsdfjgsdjkfgjnsdfjkghjfhghdsfgdsfjklghjdsfhghdsgfsjkdfgfhdsghdfshgjklsgl");//more than 50
        String check4 = _ts.UpdateTaskTitle("test@gmail.com","testBoard1",0,1, null);
        String check5 = _ts.UpdateTaskTitle("test@gmail.com","testBoard1",0,89, "hello");

        Response? r1 = JsonSerializer.Deserialize<Response>(check1);
        Response? r2 = JsonSerializer.Deserialize<Response>(check2);
        Response? r3 = JsonSerializer.Deserialize<Response>(check3);
        Response? r4 = JsonSerializer.Deserialize<Response>(check4);
        Response? r5 = JsonSerializer.Deserialize<Response>(check5);

        if (r1.ErrorOccured)
        {
            log.Fatal("first update title has a problem");
            return false;
        }
        if (!r2.ErrorOccured)
        {
            log.Fatal("second update title has a problem, it should return an errormessage");
            return false;
        }
        if (!r3.ErrorOccured)
        {
            log.Fatal("third update has title a problem, it should return an error message");
            return false;
        }
        if (!r4.ErrorOccured)
        {
            log.Fatal("fourth update has title a problem, it should return an errormessage");
            return false;
        }
        if (!r5.ErrorOccured)
        {
            log.Fatal("fifth update has title a problem, it should return an errormessage");
            return false;
        }

        log.Debug("finished TestUpdateTaskTitle");
        
        
        return true;
    }


    /// <summary>
    /// in this test we will test the capabilities to update task's description
    /// THE TESTS:
    /// <para/>
    /// 1. trying to change the description of a task that have not been finished yet (should pass)
    /// <para/>
    /// 2. trying to change the description of a task that has been finished (should fail)
    /// <para/>
    /// 3. trying to change the description of a task that have not been finished yet with a description longer then 300 charecters (should fail)
    /// <para/>
    /// 4. trying to change the description of a task that has been finished to null (should not pass)
    /// <para/>
    /// 5. trying to change the description of a task that does not exist
    /// </summary>
    /// <returns>True if all tests got favarable results.</returns>
    private bool TestUpdateTaskDescription()
    {
        _ds.DeleteData();
        _us.Register("test@gmail.com", "Aa123456");
        _bs.AddBoard("test@gmail.com", "testBoard1");
        _bs.AddTask("test@gmail.com", "testBoard1", "Tid0Column0", "something", new DateTime(2023, 5, 17));
        _bs.AddTask("test@gmail.com", "testBoard1", "Tid1Column0", "something", new DateTime(2023, 5, 19));
        _bs.AddTask("test@gmail.com", "testBoard1", "Tid2Column2", "something", new DateTime(2023, 5, 19));
        //only Assignee may update tasks features
        _bs.AssignTask("test@gmail.com", "testBoard1", 0, 2, "test@gmail.com");
        _bs.AssignTask("test@gmail.com", "testBoard1", 0, 1, "test@gmail.com");
        _bs.AssignTask("test@gmail.com", "testBoard1", 0, 0, "test@gmail.com");

        _bs.AdvanceTask("test@gmail.com", "testBoard1", 0, 2);
        _bs.AdvanceTask("test@gmail.com", "testBoard1", 1, 2);

        String check1 = _ts.UpdateTaskDescription("test@gmail.com","testBoard1",0,1, "Wiz khalifa");
        String check2 = _ts.UpdateTaskDescription("test@gmail.com","testBoard1",2,2, "water");
        String check3 = _ts.UpdateTaskDescription( "test@gmail.com","testBoard1",0,1, "sdjfnsdjfjkdsgjsdfjkgjsdfjgsdfjgjnsdfjkgjsdfjgsdjkfgjnsdfjkghjfhghdsfgdsfjklghjdsfhghdsgfsjkdfgfhdsghdfshgjklsglsdjfnsdjfjkdsgjsdfjkgjsdfjgsdfjgjnsdfjkgjsdfjgsdjkfgjnsdfjkghjfhghdsfgdsfjklghjdsfhghdsgfsjkdfgfhdsghdfshgjklsglsdjfnsdjfjkdsgjsdfjkgjsdfjgsdfjgjnsdfjkgjsdfjgsdjkfgjsdfdfnsdfjkghjfhghdsfgdsfjklg");//more than 300
        String check4 = _ts.UpdateTaskDescription("test@gmail.com","testBoard1",0,2, null);
        String check5 = _ts.UpdateTaskDescription("test@gmail.com","testBoard1",0,89, "hello");

        Response? r1 = JsonSerializer.Deserialize<Response>(check1);
        Response? r2 = JsonSerializer.Deserialize<Response>(check2);
        Response? r3 = JsonSerializer.Deserialize<Response>(check3);
        Response? r4 = JsonSerializer.Deserialize<Response>(check4);
        Response? r5 = JsonSerializer.Deserialize<Response>(check5);

        if (r1.ErrorOccured)
        {
            log.Fatal("first update has a problem");
            return false;
        }
        if (!r2.ErrorOccured)
        {
            log.Fatal("second update desc has a problem, it should return an errormessage");
            return false;
        }
        if (!r3.ErrorOccured)
        {
            log.Fatal("third update desc has a problem, it should return an error message");
            return false;
        }
        if (!r4.ErrorOccured)
        {
            log.Fatal("fourth update desc has a problem, it should return error message");
            return false;
        }
        if (!r5.ErrorOccured)
        {
            log.Fatal("fourth update desc has a problem, it should return an error message");
            return false;
        }

        log.Debug("finished TestUpdateDesc");
        
        
        return true;
    }
}
