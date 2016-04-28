using System.Collections.Generic;
using Microsoft.AspNet.Identity;

namespace FWest98.IdentityExtensions.Tests.Models {
    public class ExtendedUser : IUser<int> {
        public int Id { get; set; }
        public string UserName { get; set; }

        public List<ExtendedApiKey> ApiKeys { get; set; }
        public List<AuthenticationToken> AuthenticationTokens { get; set; } 
    }
}
