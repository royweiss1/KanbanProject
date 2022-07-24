namespace IntroSE.Kanban.Frontend;
using IntroSE.Kanban.Backend.BusinessLayer;
using IntroSE.Kanban.Backend.ServiceLayer;
using System.Reflection;
using System.Text.Json;
using log4net;

public class DataServiceTests
{
    private BoardService _bs;
    private UserService _us;
    private DataService _ds;
    private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    
    public DataServiceTests()
    {
        FactoryService fs = new FactoryService();
        _bs = fs.BoardService;
        _us = fs.UserService;
        _ds = fs.DataService;

    }

    public bool RunTest()
    {
        return TestLoadData() && TestDeleteData();
    }

    /// <summary>
    /// in this test we will test the capabilities to load data
    /// THE TESTS:
    /// <para/>
    /// 1. trying to register a user that already exists(should fail)
    /// <para/>
    /// 2. trying to add a task to user in a board that should exists(should pass)
    /// <para/>
    /// 3. trying to assign a task for a user that isn't in the board(should fail)
    /// <para/>
    /// 4. trying to advance a task that no longer exists in this coulmn(should fail)
    /// <para/>
    /// 5. trying to transfer onwership from valid owner to valid user(should pass)
    /// <para/>
    /// 6. trying to assign a task to a user that is already assigned to it(should fail)
    /// <para/>
    /// 7. trying to join a board that user is already part of(should fail)
    /// <para/>
    /// 8. trying to join a user that isn't in board(should pass)
    /// <para/>
    /// 9. trying to transfer onwership from valid owner to valid user(should pass)
    /// </summary>
    /// <returns>True if all tests got favarable results.</returns>
    private bool TestLoadData()
    {
        _ds.DeleteData(); //assume its empty now and add stuff into it
        _us.Register("test@gmail.com", "Aa123456");
        _us.Register("test2@gmail.com", "Aa123456");
        _us.Register("test3@gmail.com", "Aa123456");
        
        _bs.AddBoard("test@gmail.com", "b1");//id 0
        _bs.AddBoard("test@gmail.com", "b2"); //id 1
        _bs.AddBoard("test@gmail.com", "b3");// id 2
        
        _bs.AddTask("test@gmail.com", "b1", "t1", "d1", DateTime.MaxValue); //id 0
        _bs.AssignTask("test@gmail.com", "b1", 0, 0, "test@gmail.com");
        
        _bs.AddTask("test@gmail.com", "b2", "t2", "d2", DateTime.MaxValue); //id 0
        _bs.AssignTask("test@gmail.com", "b2", 0, 0, "test@gmail.com");
        
        _bs.AddTask("test@gmail.com", "b3", "t3", "d3", DateTime.MaxValue); //id 0
        _bs.AssignTask("test@gmail.com", "b3", 0, 0, "test@gmail.com");
        
        _bs.AdvanceTask("test@gmail.com", "b1", 0, 0);
        _bs.JoinBoard("test2@gmail.com", 1);
        _bs.TransferOwnership("test@gmail.com", "test2@gmail.com", "b2");
        
        _bs.AddTask("test@gmail.com", "b2", "t2.1", "d2.1", DateTime.MaxValue);//id 1
        
        _bs.JoinBoard("test3@gmail.com", 1);
        _bs.AssignTask("test2@gmail.com", "b2", 0, 1, "test3@gmail.com");

        _ds.LoadData();
        String check1 = _us.Register("test@gmail.com", "Aa123456"); //should fail, user exists
        _us.Login("test@gmail.com", "Aa123456");
        _us.Login("test2@gmail.com", "Aa123456");
        _us.Login("test3@gmail.com", "Aa123456");
        _bs.AddBoard("test@gmail.com", "b4");
        String check2 = _bs.AddTask("test@gmail.com", "b1", "t1", "d1", DateTime.MaxValue);//should pass
        String check3 = _bs.AssignTask("test3@gmail.com", "b1", 0, 0, "test@gmail.com"); //should fail test3 is not in board
        String check4 = _bs.AdvanceTask("test@gmail.com", "b1", 0, 0); //should fail, it no longer exists in this column
        String check5 = _bs.TransferOwnership("test2@gmail.com", "test@gmail.com", "b2");//should pass
        String check6 = _bs.AssignTask("test2@gmail.com", "b2", 0, 1, "test3@gmail.com");//should fail, user is already assigned here
        String check7 = _bs.JoinBoard("test3@gmail.com", 1); //should fail, user already in board
        _us.Register("test4@gmail.com", "Aa123456");
        String check8 = _bs.JoinBoard("test4@gmail.com", 0); //should pass
        String check9 = _bs.TransferOwnership("test@gmail.com", "test4@gmail.com", "b1"); //should pass
        Response? r1 = JsonSerializer.Deserialize<Response>(check1);
        Response? r2 = JsonSerializer.Deserialize<Response>(check2);
        Response? r3 = JsonSerializer.Deserialize<Response>(check3);
        Response? r4 = JsonSerializer.Deserialize<Response>(check4);
        Response? r5 = JsonSerializer.Deserialize<Response>(check5);
        Response? r6 = JsonSerializer.Deserialize<Response>(check6);
        Response? r7 = JsonSerializer.Deserialize<Response>(check7);
        Response? r8 = JsonSerializer.Deserialize<Response>(check8);
        Response? r9 = JsonSerializer.Deserialize<Response>(check9);
        if (!r1.ErrorOccured)
        {
            log.Error("should return an error!");
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
        if (r8.ErrorOccured)
        {
            log.Error("should not return an error!");
            return false;
        }
        if (r9.ErrorOccured)
        {
            log.Error("should not return an error!");
            return false;
        }
        log.Debug("finished load data test");
        return true;
    }
    
    /// <summary>
    /// in this test we will test the capabilities to delete data
    /// THE TESTS:
    /// <para/>
    /// 1. trying to register a user that should've been deleted(should pass)
    /// <para/>
    /// 2. trying to add a board that should've been deleted to user(should pass)
    /// <para/>
    /// 3. trying to add a task that should've been deleted(should pass)
    /// <para/>
    /// 4. trying to assign a task by a user that should've been deleted(should fail)
    /// <para/>
    /// 5. trying to transfer onwership by a user that should've been deleted(should fail)
    /// <para/>
    /// 6. trying to assign a task by a user that doesn't exist(should fail)
    /// <para/>
    /// 7. trying to join a board for a user that doesn't exist(should fail)
    /// <para/>
    /// 8. trying to join a user that isn't in board(should pass)
    /// <para/>
    /// 9. trying to transfer onwership from valid owner to valid user(should pass)
    /// </summary>
    /// <returns>True if all tests got favarable results.</returns>
    private bool TestDeleteData()
    {
        _ds.DeleteData(); //assume its empty now and add stuff into it
        _us.Register("test@gmail.com", "Aa123456");
        _us.Register("test2@gmail.com", "Aa123456");
        _us.Register("test3@gmail.com", "Aa123456");
        
        _bs.AddBoard("test@gmail.com", "b1");//id 0
        _bs.AddBoard("test@gmail.com", "b2"); //id 1
        _bs.AddBoard("test@gmail.com", "b3");// id 2
        
        _bs.AddTask("test@gmail.com", "b1", "t1", "d1", DateTime.MaxValue); //id 0
        _bs.AssignTask("test@gmail.com", "b1", 0, 0, "test@gmail.com");
        
        _bs.AddTask("test@gmail.com", "b2", "t2", "d2", DateTime.MaxValue); //id 0
        _bs.AssignTask("test@gmail.com", "b2", 0, 0, "test@gmail.com");
        
        _bs.AddTask("test@gmail.com", "b3", "t3", "d3", DateTime.MaxValue); //id 0
        _bs.AssignTask("test@gmail.com", "b3", 0, 0, "test@gmail.com");
        
        _bs.AdvanceTask("test@gmail.com", "b1", 0, 0);
        _bs.JoinBoard("test2@gmail.com", 1);
        _bs.TransferOwnership("test@gmail.com", "test2@gmail.com", "b2");
        
        _bs.AddTask("test@gmail.com", "b2", "t2.1", "d2.1", DateTime.MaxValue);//id 1
        
        _bs.JoinBoard("test3@gmail.com", 1);
        _bs.AssignTask("test2@gmail.com", "b2", 0, 1, "test3@gmail.com");
        _ds.DeleteData();
        _ds.LoadData(); //we know that load works already
        
        String check1 = _us.Register("test@gmail.com", "Aa123456"); //should pass
        String check2 = _bs.AddBoard("test@gmail.com", "b1"); //should pass
        String check3 = _bs.AddTask("test@gmail.com", "b1", "t1", "d1", DateTime.MaxValue);//should pass
        String check4 = _bs.AssignTask("test3@gmail.com", "b1", 0, 0, "test@gmail.com"); //should fail test3 is not in board
        String check5 = _bs.TransferOwnership("test2@gmail.com", "test@gmail.com", "b2");//should fail, user test3 doesnt exist
        String check6 = _bs.AssignTask("test2@gmail.com", "b2", 0, 1, "test3@gmail.com");//should fail, user test2 doesnt exist
        String check7 = _bs.JoinBoard("test3@gmail.com", 1); //should fail, user test3 doesnt exist
        _us.Register("test4@gmail.com", "Aa123456");
        String check8 = _bs.JoinBoard("test4@gmail.com", 0); //should pass
        String check9 = _bs.TransferOwnership("test@gmail.com", "test4@gmail.com", "b1"); //should pass
        Response? r1 = JsonSerializer.Deserialize<Response>(check1);
        Response? r2 = JsonSerializer.Deserialize<Response>(check2);
        Response? r3 = JsonSerializer.Deserialize<Response>(check3);
        Response? r4 = JsonSerializer.Deserialize<Response>(check4);
        Response? r5 = JsonSerializer.Deserialize<Response>(check5);
        Response? r6 = JsonSerializer.Deserialize<Response>(check6);
        Response? r7 = JsonSerializer.Deserialize<Response>(check7);
        Response? r8 = JsonSerializer.Deserialize<Response>(check8);
        Response? r9 = JsonSerializer.Deserialize<Response>(check9);
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
        if (r3.ErrorOccured)
        {
            log.Error("should not return an error!");
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
        if (r8.ErrorOccured)
        {
            log.Error("should  not return an error!");
            return false;
        }
        if (r9.ErrorOccured)
        {
            log.Error("should not return an error!");
            return false;
        }
        log.Debug("finished delete data test");
        return true;
    }
}