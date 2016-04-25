using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWest98.IdentityExtensions.Tests.Models {
    public class SimpleApiKey : ISimpleApiKey<int> {
        public int Id { get; set; }
        public string PublicKey { get; set; }
    }
    public class ExtendedApiKey : IExtendedApiKey<int> {
        public int Id { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
    }
}
