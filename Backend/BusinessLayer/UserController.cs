using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Reflection;
using log4net;
using log4net.Config;
using System.IO;
using IntroSE.Kanban.Backend.DataAccessLayer;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;

namespace IntroSE.Kanban.Backend.BusinessLayer;

internal class UserController
{
    private Dictionary<string, User> _users;
    private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    public UserController()
    {
        _users=new Dictionary<string, User>();
        // Load configuration
        //Right click on log4net.config file and choose Properties. 
        //Then change option under Copy to Output Directory build action into Copy if newer or Copy always.
        var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
        XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
    }



    private bool ExistsUser(string email)// returns true if success, false if fail.
    {
        if (email == null)
            return false;
        bool exists=_users.ContainsKey(email); //checks that user exists
        if (exists)
            log.Info($"user (email: {email}) exists");
        else
            log.Warn($"user (email: {email}) does not exist");
        return exists;
    }
    public void  ValidateUserExists(string email)
    {
        if (!ExistsUser(email)) //check that user exists
        {
            throw new ArgumentException($"user {email} does not exist");
        }

    }

    public void ValidateUser(string email)
    {
        if (ExistsUser(email)) //check that user exists
        {
            User user = GetUser(email); 
            if (!user.IsLogged) //check if user is logged
                throw new ArgumentException($"user {email} is not logged In");
        }
        else
        {
            throw new ArgumentException($"user {email} does not exist");
            
        }
    }
    public void RegisterUser(string email, string password)// returns true if success, false if fail.
    {
        if (ExistsUser(email)) //check if user exsist
            throw new ArgumentException($"user {email} already exists");
        
        User user = User.Create(email, password, true);
        _users.Add(email,user); //add to users
       
        log.Info($"User (email:{email}) has been registered successfully");
        

    }
    
    public User GetUser(string email)
    {
        return _users[email];
    }

    

    public void Login(string email, string password)
    {
        if (!ExistsUser(email)) //check that user exists
            throw new ArgumentException($"user {email} does not exists");
        User u = GetUser(email);
        if (u.IsLogged)
            throw new ArgumentException($"user {email} is already logged in");
        u.Login(password); //logout

    }

    public void Logout(string email)
    {
        if (!ExistsUser(email)) //check that user exists
            throw new ArgumentException($"user {email} does not exists");
        User u = GetUser(email);
        if (!u.IsLogged)
            throw new ArgumentException($"user {email} is not logged in");
        u.Logout(); //logout
        
        
    }

    internal void LoadData()
    {
        List<UserDTO> users = new UserDTOMapper().LoadAllUserDtos(); //load all the user dto's from DAL
        foreach (var userDto in users)
        {
            _users[userDto.Email] = new User(userDto);
        }
    }


    internal void DeleteData()
    {
        new UserDTOMapper().DeleteAll(); //deletes all the users
        _users = new();
    }
      
}