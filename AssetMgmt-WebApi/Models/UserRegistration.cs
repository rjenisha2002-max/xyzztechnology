using System;

namespace AssetMgmt_WebApi.Models
{
    //user registration
    public partial class UserRegistration
    {
        public int UId { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public int Age { get; set; }

        public string Gender { get; set; } = null!;

        public string Address { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public int LId { get; set; }

        public virtual Login Login { get; set; } = null!;
    }
}
