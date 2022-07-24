using System;
using System.Text.Json;
using IntroSE.Kanban.Backend.BusinessLayer;

namespace IntroSE.Kanban.Backend.ServiceLayer;

public class UserService
{
    private UserController _userController;
    
    internal UserService(UserController uc)
    {
        _userController = uc;
    }
    
    /// <summary>
    /// This method registers a new user to the system.
    /// </summary>
    /// <param name="email">The user email address, used as the username for logging the system.</param>
    /// <param name="password">The user password.</param>
    /// <returns>The string "{}", unless an error occurs (see <see cref="GradingService"/>)</returns>
    public string Register(string email, string password)
    {
        try
        {
            _userController.RegisterUser(email,password);
            return JsonSerializerExtention.Serialize(new Response(null, null));

        }
        catch (Exception e)
        {
            return JsonSerializerExtention.Serialize(new Response(e.Message, null));

        }
        

    }
    
    /// <summary>
    ///  This method logs in an existing user.
    /// </summary>
    /// <param name="email">The email address of the user to login</param>
    /// <param name="password">The password of the user to login</param>
    /// <returns>Response with user email, unless an error occurs (see <see cref="GradingService"/>)</returns>
    public string Login(string email, string password)
    {
        try
        {
            _userController.Login(email,password);
            return JsonSerializerExtention.Serialize(new Response(null, email));

        }
        catch (Exception e)
        {
            return JsonSerializerExtention.Serialize(new Response(e.Message, null));

        } 
       
    }

    /// <summary>
    /// This method logs out a logged in user. 
    /// </summary>
    /// <param name="email">The email of the user to log out</param>
    /// <returns>The string "{}", unless an error occurs (see <see cref="GradingService"/>)</returns>
    public string Logout(string email)
    {
        try
        {
            _userController.Logout(email);
            return JsonSerializerExtention.Serialize(new Response(null, null));

        }
        catch (Exception e)
        {
            return JsonSerializerExtention.Serialize(new Response(e.Message, null));

        } 
    }


}