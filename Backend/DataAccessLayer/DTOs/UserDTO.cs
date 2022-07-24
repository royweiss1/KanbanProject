using System;
using System.Collections.Generic;

namespace IntroSE.Kanban.Backend.DataAccessLayer.DTOs
{
    public class UserDTO : DTO
    {
        public const string UserEmailName = "Email";
        public const string UserPasswordName = "Password";

        public string Email { get; }
        public string Password { get; } // because user may not change its password as instructions are now

        //Mind that login and log out will be done on RAM
        public UserDTO(string email, string password) : base(new UserDTOMapper())
        {
            Email = email;
            Password = password;
        }
        public void Register()
        {
            new UserDTOMapper().Register(this); //register user
            log.Info($"User: {Email} has been registered and added to DB");
        }

        public override void Persist()
        {
            Register();
        }
        
        public override bool Equals(object obj)
        { 
            return  ((UserDTO)obj).Email == Email;

        
        }
    }
}