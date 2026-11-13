using System;
using System.Collections.Generic;
using System.Text;

namespace MusicPortal.DAL.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Salt { get; set; }

        public bool IsAdmin { get; set; } = false;

        public bool IsRegistrationConfirmed { get; set; } = false;

        public DateTime RegistrationDate { get; set; }
            = DateTime.UtcNow;
    }
}
