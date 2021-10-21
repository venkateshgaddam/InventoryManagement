using System;

namespace IM.Common.Model.EntityModels
{
    public class RegisterUserModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }
        
        public string PhoneNumber { get; set; }

        public DateTime DateofBirth { get; set; }

        public UserAddress UserAddress { get; set; }

    }

    public class UserAddress
    {
        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Country { get; set; }

        public string ZipCode { get; set; }
    }
}
