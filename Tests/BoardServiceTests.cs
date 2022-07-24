using IntroSE.Kanban.Backend.BusinessLayer;
using IntroSE.Kanban.Backend.ServiceLayer;
using Task = System.Threading.Tasks.Task;

namespace IntroSE.Kanban.Frontend;
using System.Reflection;
using System.Text.Json;
using log4net;
public class BoardServiceTests
{
    private BoardService _bs;
    private UserService _us;
    private DataService _ds;
    private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    

    public BoardServiceTests()
    {
        FactoryService fs = new FactoryService();
        _bs = fs.BoardService;
        _us = fs.UserService;
        _ds = fs.DataService;

    }
    
    
    //TODO: *important* for the examiner to read
    //for the tests to work we will call functions like addBoard before we test, but that will happen later in implementation.
    // some of the id's of the tasks were called just for reference. when we'll construct a BoardServiceTest object we will add all boards and tasks, so the tests will run properly.
    public Boolean RunTests()
    {
        return
            //TestAddTask() &
            //TestLimitColumn() &
            //TestGetColumnName()&
            //TestAdvanceTask()&
            //GetColumn() &
            //TestAddBoard() &
            //TestRemoveBoard() &
            //TestInProgressTasks() & 
            //GetUserBoardsTest() &
            //JoinBoardTest() &
            // LeaveBoardTest() &
            AssignTaskTest(); //&
        //TransferOwnershipTest();

    }
    
    
    /// <summary>
    /// This method checks the function LimitColumn in grading service.
    /// we will test 3 Different Inputs:
    /// <para/>
    /// 1) The function will try to set a positive limit column (integer)
    /// <para/>
    /// 2) The function will try to set a negative limit column
    /// <para/>
    /// 3) the function will receive a negative integer as its column ordinal.
    /// </summary>
    /// <returns>returns True if all tests have been successful(Trying to set limit column to a negative
    /// number or null has returned an error and trying to set a positive limit has been successful).
    /// returns false if at least one of the tests has gone wrong(did not return an error
    /// for the tests that try to set negative number or null, or did return an error message for
    /// the test that set a positive integer number).</returns>
    public bool TestLimitColumn()
    {
        _ds.DeleteData();
        
        _us.Register("test@gmail.com", "Aa123456");
        _bs.AddBoard("test@gmail.com", "testBoard1");
        _bs.AddBoard("test@gmail.com", "testBoard2");
        _bs.LimitColumn("test@gmail.com", "testBoard2", 0, 500);
        _bs.LimitColumn("test@gmail.com", "testBoard2", 0, -1);
        String check1 = _bs.GetColumnLimit("test@gmail.com", "testBoard1", 0);
        String check2 = _bs.GetColumnLimit("test@gmail.com", "testBoard2", 1);
        String check3 = _bs.GetColumnLimit("test@gmail.com", null, 1);
        Response? r1 = JsonSerializer.Deserialize<Response>(check1);
        Response? r2 = JsonSerializer.Deserialize<Response>(check2);
        Response? r3 = JsonSerializer.Deserialize<Response>(check3);

        if (r1.ErrorOccured)
        {
            log.Error("first getter of limit has a problem");
            return false;
        }
        if (r2.ErrorOccured)
        {
            log.Error("second getter of limit has a problem");
            return false;
        }
        if (!r3.ErrorOccured)
        {
            log.Error("third getter of limit has a problem, it should return an error message");
            return false;
        }
        log.Debug("finished get column limit test");
        
        return true;

        
    }

    /// <summary>
    /// This method checks the function GetColumnLimit in grading service.
    /// we will test 3 Different Inputs:
    /// <para/>
    /// 1) The function will try to get a limit of a column with ordinal between 1-3.
    /// <para/>
    /// 2) The function will try to get a limit of a column with negative ordinals
    /// <para/>
    /// 3) The function will try to get a limit of a column with null as board name
    /// </summary>
    /// <returns>returns True if all tests have been successful(Trying to get limit of a
    /// column with negative ordinals or null has returned an error and trying to get limit of
    /// a column with ordinals between 1 and 3 has returned a number).
    /// returns false if at least one of the tests has gone wrong(Trying to get limit of a
    /// column with negative ordinals or null did not return an error and trying to get limit of
    /// a column with ordinals between 1 and 3 has returned an error).</returns>
    public bool TestGetColumnLimit()
    {
        _ds.DeleteData();
        
        _us.Register("test@gmail.com", "Aa123456");
        _bs.AddBoard("test@gmail.com", "testBoard1");
        _bs.AddBoard("test@gmail.com", "testBoard2");
        _bs.LimitColumn("test@gmail.com", "testBoard2", 0, 500);
        _bs.LimitColumn("test@gmail.com", "testBoard2", 0, -1);
        String check1 = _bs.GetColumnLimit("test@gmail.com", "testBoard1", 0);
        String check2 = _bs.GetColumnLimit("test@gmail.com", "testBoard2", 1);
        String check3 = _bs.GetColumnLimit("test@gmail.com", null, 1);
        Response? r1 = JsonSerializer.Deserialize<Response>(check1);
        Response? r2 = JsonSerializer.Deserialize<Response>(check2);
        Response? r3 = JsonSerializer.Deserialize<Response>(check3);

        if (r1.ErrorOccured)
        {
            log.Error("first getter of limit has a problem");
            return false;
        }
        if (r2.ErrorOccured)
        {
            log.Error("second getter of limit has a problem");
            return false;
        }
        if (!r3.ErrorOccured)
        {
            log.Error("third getter of limit has a problem, it should return an error message");
            return false;
        }
        log.Debug("finished get column limit test");
        
        return true;
    }

    /// <summary>
    /// This method checks the function GetColumnLimit in grading service.
    /// we will test 3 Different Inputs:
    /// <para/>
    /// 1) The function will try to get the name of a column with ordinal between 1-3.
    /// <para/>
    /// 2) The function will try to get the name of a column with negative ordinals
    /// <para/>
    /// 3) The function will try to get the name of a column with null ordinals
    /// </summary>
    /// <returns>returns True if all tests have been successful(Trying to get name of a
    /// column with negative ordinals or null has returned an error and trying to get name of
    /// a column with ordinals between 1 and 3 has returned a name).
    /// returns false if at least one of the tests has gone wrong(Trying to get name of a
    /// column with negative ordinals or null did not return an error and trying to get name of
    /// a column with ordinals between 1 and 3 has returned an error).</returns>
    public bool TestGetColumnName()
    {
        _ds.DeleteData();
        
        _us.Register("test@gmail.com", "Aa123456");
        _bs.AddBoard("test@gmail.com", "testBoard1");
        _bs.AddBoard("test@gmail.com", "testBoard2");

        String check1 = _bs.GetColumnName("test@gmail.com", "testBoard1", 1);
        String check2 = _bs.GetColumnName("test@gmail.com", "testBoard2", -1);
        String check3 = _bs.GetColumnName("test@gmail.com", null, 1);
        Response? r1 = JsonSerializer.Deserialize<Response>(check1);
        Response? r2 = JsonSerializer.Deserialize<Response>(check2);
        Response? r3 = JsonSerializer.Deserialize<Response>(check3);

        if (r1.ErrorOccured)
        {
            log.Error("first getter of name has a problem");
            return false;
        }
        if (!r2.ErrorOccured)
        {
            log.Error("second getter of name has a problem, it should return an ErrorMessage");
            return false;
        }
        if (!r3.ErrorOccured)
        {
            log.Error("third getter of name has a problem, it should return an error message");
            return false;
        }
        log.Debug("finished get column name test");


        return true;
        
    }

    /// <summary>
    /// This method checks the function AddTask in grading service.
    /// we will test 2 Different Inputs:
    /// <para/>
    /// 1) The function will try to add a task with all valid parameters
    /// <para/>
    /// 2) The function will try to add a task with an un existing board name
    /// <para/>
    /// 3) The function will try to add a task with an invalid due date
    /// <para/>
    /// 4) the function will try to add a task will null email
    /// </summary>
    /// <returns>returns True if all tests have been successful(Tests 2-4 have all returned an
    /// error and tests 1 and 5 has successfully added a task.).
    /// returns false if at least one of the tests has gone wrong(Test 1 or 5 were un successful
    /// or one of the tests between 2-4 did not return an error).</returns>
    public bool TestAddTask()
    {
        _ds.DeleteData();
        
        _us.Register("test@gmail.com", "Aa123456");
        _bs.AddBoard("test@gmail.com", "testBoard1");
        String check1 = _bs.AddTask("test@gmail.com", "testBoard1", "helloWorld1", "", DateTime.MaxValue);// should pass
        String check2 = _bs.AddTask("test@gmail.com", "NoBoard", "helloWorld2", "Hello", DateTime.MaxValue); //no such board name
        String check3 = _bs.AddTask("test@gmail.com", "testBoard1", "helloWorld3", "Hello", DateTime.MinValue); //time has already passed compared to CreationDate
        String check4 = _bs.AddTask(null, "testBoard1", "helloWorld4", "Hello", DateTime.MaxValue); //fail email=null
        Response? r1 = JsonSerializer.Deserialize<Response>(check1);
        Response? r2 = JsonSerializer.Deserialize<Response>(check2);
        Response? r3 = JsonSerializer.Deserialize<Response>(check3);
        Response? r4 = JsonSerializer.Deserialize<Response>(check4);
        
        if (r1.ErrorOccured)
        {
            log.Error("first task adder has a problem");
            return false;
        }
        if (!r2.ErrorOccured)
        {
            log.Error("second task adder has a problem, it should return an ErrorMessage");
            return false;
        }
        if (!r3.ErrorOccured)
        {
            log.Error("third task adder has a problem, it should return an error message");
            return false;
        }
        if (!r4.ErrorOccured)
        {
            log.Error("forth task adder has a problem, it should return an error message");
            return false;
        }
        log.Debug("finished addTask test");

        return true;
        
    }

    /// <summary>
    /// in this test we will test the capabilities to advance its tasks from "backlog" to "in progress" or from "in progress" to "done" only. according to the demends
    /// THE TESTS:
    /// <para/>
    /// 1. trying to advance a task from 'backlog' to "in progress" when "in progress" capacity is not at max.
    /// <para/>
    /// 2. trying to advance a task from "in progress" to "done" when "done" capacity is not at max (should fail)
    /// <para/>
    /// 3. trying to advance a task from "done"
    /// <para/>
    /// 4. trying to advance a task from 'backlog' to "in progress" when capacity is at max (should fail)
    /// <para/>
    /// </summary>
    /// <returns>True if all tests got favorable results.</returns>
    private bool TestAdvanceTask()
    {
        _ds.DeleteData();
        
        _us.Register("test@gmail.com", "Aa123456");
        _us.Register("test1@gmail.com", "Aa123456");
        
        _bs.AddBoard("test@gmail.com", "testBoard1");
        _bs.JoinBoard("test1@gmail.com", 0);
        
        _bs.AddTask("test@gmail.com", "testBoard1", "T0", "s", new DateTime(2023, 1, 1));
        _bs.AddTask("test@gmail.com", "testBoard1", "T1", "d", new DateTime(2023, 1, 1));
        _bs.AssignTask("test@gmail.com", "testBoard1", 0, 0, "test@gmail.com");
        _bs.AssignTask("test@gmail.com", "testBoard1", 0, 1, "test@gmail.com");
        
        String check1 = _bs.AdvanceTask("test@gmail.com", "testBoard1", 0, 0); //should pass
        Response? r1 = JsonSerializer.Deserialize<Response>(check1);
        if (r1.ErrorOccured)
        {
            log.Error("first task Advancement has a problem:" +r1.ErrorMessage);
            return false;
        }
        String check2 = _bs.AdvanceTask("test@gmail.com", "testBoard1", 1, 0); //should pass
        Response? r2 = JsonSerializer.Deserialize<Response>(check2);
        if (r2.ErrorOccured)
        {
            log.Error("second task advancement has a problem:" + r2.ErrorMessage);
            return false;
        }
        String check3 = _bs.AdvanceTask("test@gmail.com", "testBoard1", 2, 0); //should fail
        Response? r3 = JsonSerializer.Deserialize<Response>(check3);
        if (!r3.ErrorOccured)
        {
            log.Error("third task advancement should return an error message");
            return false;
        }
        String check4 = _bs.AdvanceTask("test1@gmail.com", "testBoard1", 0, 1); //should fail, isnt assignee
        Response? r4 = JsonSerializer.Deserialize<Response>(check4);
        if (!r4.ErrorOccured)
        {
            log.Error("fourth task advancement has a problem, it should return an error message");
            return false;
        }
        log.Debug("finished AdvanceTask test");

        return true;
    }


    /// <summary>
    /// in this test we will test the capabilities to get its coloums
    /// THE TESTS:
    /// <para/>
    /// 1. trying to get a coloum that exists (should pass)
    /// <para/>
    /// 2. trying to get a coloum that doesnt exists (should fail)
    /// </summary>
    /// <returns>True if all tests got favarable results.</returns>
    private bool GetColumn()
    {
        _ds.DeleteData();
        
        _us.Register("test@gmail.com", "Aa123456");
        _bs.AddBoard("test@gmail.com", "testBoard1");

        _bs.AddTask("test@gmail.com", "testBoard1", "T0", "s", new DateTime(2023, 1, 1)); //Tid=0
        _bs.AddTask("test@gmail.com", "testBoard1", "T1", "d", new DateTime(2023, 1, 1)); //Tid=1
        //first assign in order to advance:
        _bs.AssignTask("test@gmail.com", "testBoard1", 0, 0, "test@gmail.com");
        _bs.AssignTask("test@gmail.com", "testBoard1", 0, 1, "test@gmail.com");

        _bs.AdvanceTask("test@gmail.com", "testBoard1", 0, 0);
        _bs.AdvanceTask("test@gmail.com", "testBoard1", 0, 1);
        
        String check1 = _bs.GetColumn("test@gmail.com", "testBoard1", 1); //should pass with 2 tasks
        String check2 = _bs.GetColumn("test@gmail.com", "testBoard1", 5);
        Response? r1 = JsonSerializer.Deserialize<Response>(check1);
        Response? r2 = JsonSerializer.Deserialize<Response>(check2);
        if (r1.ErrorOccured)
        {
            log.Error("first get column has a problem "+r1.ErrorMessage);
            return false;
        }
        Console.WriteLine("2 Tasks: \n");
        Console.WriteLine(r1.ReturnValue);
        if (!r2.ErrorOccured)
        {
            log.Error("second get column has a problem, it should return an error message");
            return false;
        }
        log.Debug("finished get column test");
        
        return true;
    }


    /// <summary>
    /// in this test we will test the capebilities to add boards
    /// THE TESTS:
    /// <para/>
    /// 1. trying to add a board with a name that is already taken by another board of the same user (should fail)
    /// <para/>
    /// 2. trying to add a board with a name that is not already taken by another board of the same user (should pass)
    /// <para/>
    /// 3. trying to add a board to a user that doesnt exist yet (should fail)
    /// </summary>
    /// <returns>True if all tests got favarable results.</returns>
    private bool TestAddBoard()
    {
        _ds.DeleteData();
        
        _us.Register("test@gmail.com", "Aa123456");
        
        String check1 = _bs.AddBoard("test@gmail.com", "HelloWorldBoard");//should pass
        String check2 = _bs.AddBoard("test@gmail.com", "HelloWorldBoard"); // should fail
        String check3 = _bs.AddBoard("notExist@gmail.com", "Board123"); // should fail
        Response? r1 = JsonSerializer.Deserialize<Response>(check1);
        Response? r2 = JsonSerializer.Deserialize<Response>(check2);
        Response? r3 = JsonSerializer.Deserialize<Response>(check3);
        if (r1.ErrorOccured)
        {
            log.Error("should not return an error!");
            return false;
        }

        if (!r2.ErrorOccured)
        {
            log.Error("should return an error!");
            return false;
        }

        if (!r3.ErrorOccured)
        {
            log.Error("should return an error!");
            return false;
        }

        log.Debug("finished add board test");

        return true;

    }


    /// <summary>
    /// in this test we will test the capebilities to remove boards
    /// THE TESTS:
    /// <para/>
    /// 1. trying to remove that exist to the given user (should pass)
    /// <para/>
    /// 2. trying to remove that doesnt exist to the given user (should fail)
    /// <para/>
    /// 3. trying to remove a board to an user that doesnt exist yet (should fail)
    /// </summary>
    /// <returns>True if all tests got favarable results.</returns>
    private bool TestRemoveBoard()
    {
        _ds.DeleteData();
        
        _us.Register("test@gmail.com", "Aa123456");
        _bs.AddBoard("test@gmail.com", "testBoard1");
        String check1 = _bs.RemoveBoard("test@gmail.com", "testBoard1");//should pass
        String check2 = _bs.RemoveBoard("test@gmail.com", "testBoard1"); // should fail already removed
        String check3 = _bs.RemoveBoard("test@gmail.com", "testBoard2"); // should fail no such board to user
        String check4 = _bs.RemoveBoard("notExist@gmail.com", "Board123"); // should fail no such user,board
        Response? r1 = JsonSerializer.Deserialize<Response>(check1);
        Response? r2 = JsonSerializer.Deserialize<Response>(check2);
        Response? r3 = JsonSerializer.Deserialize<Response>(check3);
        Response? r4 = JsonSerializer.Deserialize<Response>(check4);
        if (r1.ErrorOccured)
        {
            log.Error("should not return an error!");
            return false;
        }
        if (!r2.ErrorOccured)
        {
            log.Error("should return an error!");
            return false;
        }
        if (!r3.ErrorOccured)
        {
            log.Error("should return an error!");
            return false;
        }
        if (!r4.ErrorOccured)
        {
            log.Error("should return an error!");
            return false;
        }
        log.Debug("finished remove board test");
        
        return true;
    }


    /// <summary>
    /// in this test we will test the capebilities to return all its task's that in progress to a spesific user
    /// we will create a valid user and add various tasks (some in status: in progress and some with other statuses)
    /// </summary>
    /// <returns>True if all the favorable tasks had been returned</returns>
    private bool TestInProgressTasks()
    {
        _ds.DeleteData();
        
        _us.Register("inProgTest@gmail.com", "Aa123456");
        _bs.AddBoard("inProgTest@gmail.com", "InProgBoard");
        _bs.AddTask("inProgTest@gmail.com", "InProgBoard", "T0", "g", DateTime.MaxValue); //Tid=0
        _bs.AddTask("inProgTest@gmail.com", "InProgBoard", "T1", "g", DateTime.MaxValue); //Tid=1
        _bs.AssignTask("inProgTest@gmail.com", "InProgBoard", 0, 0, "inProgTest@gmail.com");
        _bs.AssignTask("inProgTest@gmail.com", "InProgBoard", 0, 1, "inProgTest@gmail.com");
        _bs.AdvanceTask("inProgTest@gmail.com", "InProgBoard", 0, 0);
        _bs.AdvanceTask("inProgTest@gmail.com", "InProgBoard", 0, 1);
        
        String check1 = _bs.InProgressTasks("inProgTest@gmail.com"); 
        Response? r1 = JsonSerializer.Deserialize<Response>(check1);
        if (r1.ErrorOccured)
        {
            log.Error("should not return an error!");
            return false;
        }
        Console.WriteLine("2 Tasks: \n");
        Console.WriteLine(r1.ReturnValue);
        log.Debug("finished inProgTask test");
        return true;
    }
        
    //****new*****
    private bool GetUserBoardsTest()
    {
        _ds.DeleteData();
        
        _us.Register("testUser@gmail.com", "Aa123456");
        _us.Register("inProgTest@gmail.com", "Aa123456");
        _bs.AddBoard("inProgTest@gmail.com", "InProgBoard"); //id=0
        _bs.AddBoard("inProgTest@gmail.com", "testBoard1"); //id=1
        _bs.AddBoard("inProgTest@gmail.com", "testBoard2");// id=2
        //we also want to test if joined a board: this assuming joinBoard works:
        _us.Register("testUser2@gmail.com", "Aa123456");
        _bs.JoinBoard("testUser2@gmail.com", 0);

        String check1 = _bs.GetUserBoards("inProgTest@gmail.com"); //should pass, return all boards of user.
        String check2 = _bs.GetUserBoards("testUser@gmail.com"); //should pass, return empty board.
        String check3 = _bs.GetUserBoards("nonExist123@gmail.com"); // should return an error. this user does not exist.
        String check4 = _bs.GetUserBoards("notLegalEmail123"); // should fail,illegal email address.
        String check5 = _bs.GetUserBoards("testUser2@gmail.com"); // should pass, containing list of 1 boards.
        Response? r1 = JsonSerializer.Deserialize<Response>(check1);
        Response? r2 = JsonSerializer.Deserialize<Response>(check2);
        Response? r3 = JsonSerializer.Deserialize<Response>(check3);
        Response? r4 = JsonSerializer.Deserialize<Response>(check4);
        Response? r5 = JsonSerializer.Deserialize<Response>(check5);
        if (r1.ErrorOccured)
        {
            log.Error("should not return an error!");
            return false;
        }
        Console.WriteLine("3 boards: \n");
        Console.WriteLine(r1.ReturnValue);
        if (r2.ErrorOccured)
        {
            log.Error("should not return an error!");
            return false;
        }
        Console.WriteLine("0 boards: \n");
        Console.WriteLine(r2.ReturnValue);
        if (!r3.ErrorOccured)
        {
            log.Error("should return an error!");
            return false;
        }
        if (!r4.ErrorOccured)
        {
            log.Error("should return an error!");
            return false;
        }
        if (r5.ErrorOccured)
        {
            log.Error("should not return an error!");
            return false;
        }
        Console.WriteLine("1 boards: \n");
        Console.WriteLine(r5.ReturnValue);
        log.Debug("finished GetUserBoards test");
        

        return true;

    }
    private bool JoinBoardTest()
    {
        _ds.DeleteData();
        
        _us.Register("joinTest@gmail.com", "Aa123456");
        _bs.AddBoard("joinTest@gmail.com", "b1"); // we assumme this boards id is 0. 
        _bs.AddBoard("joinTest@gmail.com", "b2");// we assumme this boards id is 1.
        _us.Register("joinTestJoiner@gmail.com", "adasd213AV");
        _bs.AddBoard("joinTestJoiner@gmail.com", "b1");// we assumme this boards id is 2.

        
        String check1 = _bs.JoinBoard("joinTestJoiner@gmail.com",1); //should pass, join to baord.
        String check2 = _bs.JoinBoard("testUserJoier@gmail.com",100000); //should fail, no board with id 100000
        string check3 = _bs.JoinBoard("nonExist123@gmail.com",5); // should return an error. this user does not exist.
        string check4 = _bs.JoinBoard("joinTestJoiner@gmail.com",1); // should fail,trying to join to a board that has the same name as a baord of the user.
        Response? r1 = JsonSerializer.Deserialize<Response>(check1);
        Response? r2 = JsonSerializer.Deserialize<Response>(check2);
        Response? r3 = JsonSerializer.Deserialize<Response>(check3);
        Response? r4 = JsonSerializer.Deserialize<Response>(check4);
        if (r1.ErrorOccured)
        {
            log.Error("should not return an error!");
            return false;
        }

        if (!r2.ErrorOccured)
        {
            log.Error("should  return an error!");
            return false;
        }

        if (!r3.ErrorOccured)
        {
            log.Error("should return an error!");
            return false;
        }
        if (!r4.ErrorOccured)
        {
            log.Error("should return an error!");
            return false;
        }
        log.Debug("finished joinboard test");

        return true;
    }
    private bool LeaveBoardTest()
    {
        _ds.DeleteData();
        
        _us.Register("leaveTestOwner@gmail.com", "Aa123456");
        _bs.AddBoard("leaveTestOwner@gmail.com", "b3"); // we assumme this boards id is 0. 
        _bs.AddBoard("leaveTestOwner@gmail.com", "b4");// we assumme this boards id is 1.
        _us.Register("leaveTestLeaver@gmail.com", "adasd213AV");
        _bs.JoinBoard("leaveTestLeaver@gmail.com", 1);
        _bs.AddTask("leaveTestLeaver@gmail.com", "b4", "task1", "some", DateTime.MaxValue);
        _bs.AddTask("leaveTestLeaver@gmail.com", "b4", "task2", "some", DateTime.MaxValue);
        _bs.AssignTask("leaveTestLeaver@gmail.com", "b4", 0, 0, "leaveTestLeaver@gmail.com");
        _bs.AssignTask("leaveTestLeaver@gmail.com", "b4", 0, 1, "leaveTestLeaver@gmail.com");

        String check1 = _bs.LeaveBoard("leaveTestLeaver@gmail.com",1); //should pass, join to baord.
        String check2 = _bs.LeaveBoard("leaveTestOwner@gmail.com",100000); //should fail, owner cant leave his board.
        string check3 = _bs.LeaveBoard("leaveTestLeaver@gmail.com",1235); // should return an error.no such board.
        String check4 = _bs.LeaveBoard("leaveTestLeaver@gmail.com",1); //should fail, already left this board.
        String check5 = _bs.LeaveBoard("leaveTestLeaver@gmail.com123@",1); //should fail,illegal email.
        Response? r1 = JsonSerializer.Deserialize<Response>(check1);
        Response? r2 = JsonSerializer.Deserialize<Response>(check2);
        Response? r3 = JsonSerializer.Deserialize<Response>(check3);
        Response? r4 = JsonSerializer.Deserialize<Response>(check4);
        Response? r5 = JsonSerializer.Deserialize<Response>(check5);

        if (r1.ErrorOccured)
        {
            log.Error("should not return an error!");
            return false;
        }

        if (!r2.ErrorOccured)
        {
            log.Error("should  return an error!");
            return false;
        }

        if (!r3.ErrorOccured)
        {
            log.Error("should return an error!");
            return false;
        }
        if (!r4.ErrorOccured)
        {
            log.Error("should return an error!");
            return false;
        }
        if (!r5.ErrorOccured)
        {
            log.Error("should return an error!");
            return false;
        }
        Console.WriteLine("Task that should be unAssigned: ");
        Console.WriteLine(_bs.GetColumn("leaveTestOwner@gmail.com","b4",0));
        log.Debug("finished leaveBoard test");

        return true;
        
    }
    private bool AssignTaskTest()
    {
        _ds.DeleteData();
        
        _us.Register("user1@gmail.com", "Aa123456");
        _us.Register("user2@gmail.com", "Aa123456");
        _us.Register("user3@gmail.com", "Aa123456");

        _bs.AddBoard("user1@gmail.com", "b0"); //assume id is 0.
        _bs.JoinBoard("user2@gmail.com", 0);
        _bs.JoinBoard("user3@gmail.com", 0);

        _bs.AddTask("user2@gmail.com", "b0", "test0", "test", DateTime.MaxValue);// assume Tid =0
        _bs.AddTask("user2@gmail.com", "b0", "test1", "test", DateTime.MaxValue);// assume Tid =1
        _bs.AddTask("user2@gmail.com", "b0", "test2", "test", DateTime.MaxValue);// assume Tid =2
        _bs.AddTask("user1@gmail.com", "b0", "test2", "test", DateTime.MaxValue);// assume Tid =3
        _bs.AddTask("user2@gmail.com", "b0", "test3", "test", DateTime.MaxValue);// assume Tid =4

        _bs.AssignTask("user2@gmail.com", "b0", 0, 0,"user2@gmail.com");
        _bs.AdvanceTask("user2@gmail.com", "b0", 0, 0);

        String check1 = _bs.AssignTask("user1@gmail.com","b0",0,1,"user1@gmail.com"); //should pass, user1 assigns a task to himself.
        String check2 = _bs.AssignTask("user2@gmail.com","b0",0,1,"user2@gmail.com"); //should fail, user2 trying to assign a task to himself but the task is assigned to user 1.
        String check3 = _bs.AssignTask("user2@gmail.com","b0",1,0,"user1@gmail.com"); //should pass, user2 assigns an unassigned-task in column 1 to user1
        String check4 = _bs.AssignTask("user2@gmail.com","b0",0,10,"user1@gmail.com"); //should fail, no such task.
        String check5 = _bs.AssignTask("user2@gmail.com","b0",0,4,"user1234@gmail.com"); //should fail, no such user to assign.
        String check6 = _bs.AssignTask("user1@gmail.com","b0",0,1,"user2@gmail.com"); // should pass. user1 is the assignee and trying to assign user 2;
        Response? r1 = JsonSerializer.Deserialize<Response>(check1);
        Response? r2 = JsonSerializer.Deserialize<Response>(check2);
        Response? r3 = JsonSerializer.Deserialize<Response>(check3);
        Response? r4 = JsonSerializer.Deserialize<Response>(check4);
        Response? r5 = JsonSerializer.Deserialize<Response>(check5);
        Response? r6 = JsonSerializer.Deserialize<Response>(check6);


        if (r1.ErrorOccured)
        {
            log.Error("should not return an error!");
            return false;
        }
        if (!r2.ErrorOccured)
        {
            log.Error("should return an error!");
            return false;
        }

        if (r3.ErrorOccured)
        {
            log.Error("should return an error!");
            return false;
        }
        if (!r4.ErrorOccured)
        {
            log.Error("should return an error!");
            return false;
        }
        if (!r5.ErrorOccured)
        {
            log.Error("should return an error!");
            return false;
        }
        if (r6.ErrorOccured)
        {
            log.Error("should not return an error!");
            return false;
        }
        log.Debug("finished leaveBoard test");



        return true;
      
        

    }
    private bool TransferOwnershipTest()
    {
        _ds.DeleteData();
        
        
        _us.Register("owner1@gmail.com", "Aa123456");
        _us.Register("owner2@gmail.com", "adasd213AV");
        _us.Register("owner3@gmail.com", "adasd213AV");

        _bs.AddBoard("owner1@gmail.com", "b0"); // we assumme this boards id is 0. 
        _bs.AddBoard("owner1@gmail.com", "b1");// we assumme this boards id is 1.
        _bs.AddBoard("owner1@gmail.com", "b2");// we assumme this boards id is 2.

        _bs.AddBoard("owner2@gmail.com", "b2"); //id=3

        //assuming joinBoard
        _bs.JoinBoard("owner2@gmail.com", 0); //join to b0
        _bs.JoinBoard("owner2@gmail.com", 1); //join to b1
        _bs.JoinBoard("owner3@gmail.com", 0); //join to b0
        _bs.JoinBoard("owner3@gmail.com", 1); //join to b1


        
        String check1 = _bs.TransferOwnership("owner1@gmail.com","owner2@gmail.com","b0"); //should pass.
        String check2 = _bs.TransferOwnership("owner2@gmail.com","owner1@gmail.com","b0"); //should pass.
        String check3 = _bs.TransferOwnership("owner1@gmail.com","owner2@gmail.com","b2"); //should fail, owner2 has a board with the same name which owner 1 is not really a member of.
        String check4 = _bs.TransferOwnership("owner1@gmail.com","owner3@gmail.com","b1"); //should pass.
        String check5 = _bs.TransferOwnership("owner1@gmail.com","owner1@gmail.com","b0"); //should pass, owner 1 is the owner and transfers to himself.
        String check6 = _bs.TransferOwnership("owne72@gmail.com","owner2@gmail.com","b0"); //should fail, no such user.
        String check7 = _bs.TransferOwnership("owner1@gmail.com","owner20@gmail.com","b0"); //should fail, no such user.
        Response? r1 = JsonSerializer.Deserialize<Response>(check1);
        Response? r2 = JsonSerializer.Deserialize<Response>(check2);
        Response? r3 = JsonSerializer.Deserialize<Response>(check3);
        Response? r4 = JsonSerializer.Deserialize<Response>(check4);
        Response? r5 = JsonSerializer.Deserialize<Response>(check5);
        Response? r6 = JsonSerializer.Deserialize<Response>(check6);
        Response? r7 = JsonSerializer.Deserialize<Response>(check7);


        if (r1.ErrorOccured)
        {
            log.Error("should not return an error!");
            return false;
        }

        if (r2.ErrorOccured)
        {
            log.Error("should not return an error!");
            return false;
        }

        if (!r3.ErrorOccured)
        {
            log.Error("should return an error!");
            return false;
        }
        if (r4.ErrorOccured)
        {
            log.Error("should not return an error!");
            return false;
        }
        if (r5.ErrorOccured)
        {
            log.Error("should not return an error!");
            return false;
        }
        if (!r6.ErrorOccured)
        {
            log.Error("should return an error!");
            return false;
        }
        if (!r7.ErrorOccured)
        {
            log.Error("should return an error!");
            return false;
        }
        log.Debug("finished leaveBoard test");

        return true;

    }
    
    

}