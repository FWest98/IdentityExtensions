using System;

namespace FWest98.IdentityExtensions.Tests.Models {
    public class AuthenticationToken : IAuthenticationToken {
        public string Token { get; set; }
        public bool IsValid => DateTime.Now > Issued && DateTime.Now < Expired && IsActive;
        public bool IsActive { get; set; }
        public DateTime Issued { get; set; }
        public DateTime Expired { get; set; }
    }
}
