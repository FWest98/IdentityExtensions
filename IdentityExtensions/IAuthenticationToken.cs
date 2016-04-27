using System;

namespace FWest98.IdentityExtensions {
    public interface IAuthenticationToken {
        string Token { get; set; }

        bool IsValid { get; }

        DateTime Issued { get; set; }
        DateTime Expired { get; set; }
    }
}
