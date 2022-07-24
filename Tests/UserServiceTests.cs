using System.Reflection;
using System.Text.Json;
using IntroSE.Kanban.Backend.BusinessLayer;
using IntroSE.Kanban.Backend.ServiceLayer;
using log4net;
using log4net.Config;
using System.IO;
using Task = System.Threading.Tasks.Task;

namespace IntroSE.Kanban.Frontend;

public class UserServiceTests
{
    private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    private UserService _us;
    private DataService _ds;
    public UserServiceTests()
    {
        FactoryService fs = new FactoryService();
        _us = fs.UserService;
        _ds = fs.DataService;
        
        // Load configuration
        //Right click on log4net.config file and choose Properties. 
        //Then change option under Copy to Output Directory build action into Copy if newer or Copy always.
        var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
        XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
    }
    
    public Boolean RunTests()
    {
        return TestRegister();
            //TestLogin(); //&
            //TestLogout();
    }
    /// <summary>
    /// This method checks the function Register in grading service.
    /// we will test 3 Different Inputs:
    /// <para/>
    /// 1) The function will first register an non existing user
    /// <para/>
    /// 2) The function will later test Register on the exact same values.
    /// <para/>
    /// 3) The function will receive non valid email
    /// /// <para/>
    /// 4) The function will receive valid email with non valid password
    /// </summary>
    /// <returns>returns True if all tests have been successful(un existing user has registered fine and
    /// attempt to register an already existing user or null has returned an error).
    /// returns false if at least one of the tests has gone wrong(failed registration of un existing
    /// user or null, or successful registration of already existing user).</returns>
    private bool TestRegister()
    {
        _ds.DeleteData();

        String check1 = _us.Register("test1@gmail.com", "Aa123456");
        String check2 = _us.Register("test1@gmail.com", "Aa123456");
        String check3 = _us.Register("Not..valid@something.com1", "Aa123456");
        String check4 = _us.Register("test2@gmail.com", "123456");

        Response? r1 = JsonSerializer.Deserialize<Response>(check1);
        Response? r2 = JsonSerializer.Deserialize<Response>(check2);
        Response? r3 = JsonSerializer.Deserialize<Response>(check3);
        Response? r4 = JsonSerializer.Deserialize<Response>(check4);

        if (r1.ErrorMessage != null)
        {
            log.Fatal("first register has a problem");
            return false;
        }
        if (r2.ErrorMessage == null)
        {
            log.Fatal("second register has a problem, it should return an ErrorMessage");
            return false;
        }
        if (r3.ErrorMessage == null)
        {
            log.Fatal("third register has a problem, it should return an error message");
            return false;
        }
        if (r4.ErrorMessage == null)
        {
            log.Fatal("forth register has a problem, it should return an error message");
            return false;
        }
        
        log.Debug("finished TestRegister");
        
        return true;
    }

    /// <summary>
    /// This method checks the function Login in grading service.
    /// we will test 3 Different Inputs:
    /// <para/>
    /// 1) The function will first Login a non existing user
    /// <para/>
    /// 2) The function will try to Login an existing user
    /// <para/>
    /// 3) The function will receive null as an email
    /// </summary>
    /// <returns>returns True if all tests have been successful(Login of un existing user or null
    /// returned an error and login of an existing user has been successful).
    /// returns false if at least one of the tests has gone wrong(failed login of an existing
    /// user or Login of an un existing user or null did not return an error).</returns>
    public bool TestLogin()
    {
        _ds.DeleteData();
        _us.Register("test@gmail.com", "Aa123456");
        _us.Register("nonExisting@gmail.com", "Aa123456");
        _us.Logout("nonExisting@gmail.com");
        string e1 = "nonExisting@gmail.com";
        string e2 = "test@gmail.com";
        string e3 = null;

        String check1 = _us.Login(e1, "Aa123456"); //pass
        String check2 = _us.Login(e2, "Aa123456"); //fail
        String check3 = _us.Login(e3, "Aa123456"); //fail
        
        Response? r1 = JsonSerializer.Deserialize<Response>(check1);
        Response? r2 = JsonSerializer.Deserialize<Response>(check2);
        Response? r3 = JsonSerializer.Deserialize<Response>(check3);

        if (r1.ErrorOccured)
        {
            log.Fatal("first login has a problem");
            return false;
        }
        if (!r2.ErrorOccured)
        {
            log.Fatal("second login has a problem, it should return an ErrorMessage");
            return false;
        }
        if (!r3.ErrorOccured)
        {
            log.Fatal("third login has a problem, it should return an error message");
            return false;
        }
        
        log.Debug("finished TestLogin");

        return true;
    }

    /// <summary>
    /// This method checks the function Logout in grading service.
    /// we will test 3 Different Inputs:
    /// <para/>
    /// 1) The function will first Logout a logged user
    /// <para/>
    /// 2) The function will try to Logout an un logged user
    /// <para/>
    /// 3) The function will receive null 
    /// </summary>
    /// <returns>returns True if all tests have been successful(Logout of un logged user or null
    /// returned an error and logout of a logged user has been successful).
    /// returns false if at least one of the tests has gone wrong(failed logout of a logged
    /// user or Logout of an un logged user or null did not return an error).</returns>
    public bool TestLogout()
    {
        _ds.DeleteData();
        _us.Register("test@gmail.com", "Aa123456");

        String check1 = _us.Logout("test@gmail.com");
        String check2 = _us.Logout("test@gmail.com");
        String check3 = _us.Logout(null);
        Response? r1 = JsonSerializer.Deserialize<Response>(check1);
        Response? r2 = JsonSerializer.Deserialize<Response>(check2);
        Response? r3 = JsonSerializer.Deserialize<Response>(check3);

        if (r1.ErrorOccured)
        {
            log.Fatal("first logout has a problem");
            return false;
        }
        if (!r2.ErrorOccured)
        {
            log.Fatal("second logout has a problem, it should return an ErrorMessage");
            return false;
        }
        if (!r3.ErrorOccured)
        {
            log.Fatal("third logout has a problem, it should return an error message");
            return false;
        }
        
        log.Debug("finished TestLogout");


        return true;
    }
}

