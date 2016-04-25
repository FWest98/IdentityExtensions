using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace FWest98.IdentityExtensions.Tests.Models {
    public class SimpleUser : IUser<int> {
        public int Id { get; set; }
        public string UserName { get; set; }

        public List<SimpleApiKey> ApiKeys { get; set; }
    }
}
