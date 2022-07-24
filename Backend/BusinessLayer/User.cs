using System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Reflection;
using log4net;
using log4net.Config;
using System.IO;
using IntroSE.Kanban.Backend.DataAccessLayer.DTOs;

namespace IntroSE.Kanban.Backend.BusinessLayer;

internal class User
{
    private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    public string Email { get; set; }
    private string Password { get; set; }
    public bool IsLogged { get; set; }
    public UserDTO UserDTO { get; }
    private User(string email, string password, bool isLogged)
    {
        if (!IsValidPassword(password))
            throw new ArgumentException($"password{password} is not valid");
        if (!ValidateEmail(email))
            throw new ArgumentException($"{email} is not valid");
        Email = email;
        Password = password;
        IsLogged = isLogged;
        UserDTO = new UserDTO(email, password);
        // Load configuration
        //Right click on log4net.config file and choose Properties. 
        //Then change option under Copy to Output Directory build action into Copy if newer or Copy always.
        var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
        XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
    }

    internal User(UserDTO userDto)
    {
        Email = userDto.Email;
        Password = userDto.Password;
        IsLogged = false;
        UserDTO = userDto;
        // Load configuration
        //Right click on log4net.config file and choose Properties. 
        //Then change option under Copy to Output Directory build action into Copy if newer or Copy always.
        var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
        XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
    }

    internal static User Create(string email, string password, bool isLogged)
    {
        User u = new User(email, password, isLogged);
        u.UserDTO.Register(); // the persist
        return u;
    }

    public void Logout()
    {
        IsLogged = false;
        log.Info($"user: {Email} has logged out");
    }

    public void Login(string password)// returns true if success, false if fail.
    {
        if (!password.Equals(Password)) // check if passwords is ok
        {
            log.Error($"Wrong password was entered for user: {Email}");
            throw new ArgumentException($"Wrong password was entered for user: {Email}");
        }
        IsLogged = true;
        log.Info($"user: {Email} has logged in");
    }
    private bool ValidateEmail(String email)// returns true if success, false if fail.
    {
        var trimmedEmail = email.Trim();

        if (trimmedEmail.EndsWith(".")) {
            return false; // suggested by @TK-421
        }
        try {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == trimmedEmail;
        }
        catch {
            return false;
        }

    }
    private bool IsValidPassword(string password)// returns true if success, false if fail.
    
    {
         string pattern = @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])[0-9a-zA-Z""~/@#$%^&*+=`|{}:;!.?'()\[\]-]{6,20}$"; //regex to validate patter
        //string pattern = @"(^[a-zA-Z0-9!@#$%^&*()_+]{6,20}$)";
        return Regex.IsMatch(password,pattern);

    }
    
    
    
    
}