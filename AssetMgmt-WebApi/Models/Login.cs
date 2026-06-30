using System;

namespace AssetMgmt_WebApi.Models
{
    //login
    public partial class Login
    {
        public int LId { get; set; }

        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string UserType { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; }

        public virtual UserRegistration? UserRegistration { get; set; }
    }
}
