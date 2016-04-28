using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FWest98.IdentityExtensions.EntityFramework {
    /// <summary>
    /// Default EntityFramework IUser implementation
    /// </summary>
    public class ExtendedIdentityUser : ExtendedIdentityUser<string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim, ExtendedIdentityApiKey, ExtendedIdentityAuthenticationToken> { }

    /// <summary>
    /// Default EntityFramework IUser implementation
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TLogin"></typeparam>
    /// <typeparam name="TRole"></typeparam>
    /// <typeparam name="TClaim"></typeparam>
    /// <typeparam name="TApiKey"></typeparam>
    /// <typeparam name="TAuthenticationToken"></typeparam>
    public class ExtendedIdentityUser<TKey, TLogin, TRole, TClaim, TApiKey, TAuthenticationToken> : IdentityUser<TKey, TLogin, TRole, TClaim>
        where TLogin : IdentityUserLogin<TKey>
        where TRole : IdentityUserRole<TKey>
        where TClaim : IdentityUserClaim<TKey>
        where TApiKey : ExtendedIdentityApiKey<TKey>
        where TAuthenticationToken : ExtendedIdentityAuthenticationToken<TKey>
        where TKey : IEquatable<TKey> {

        public ExtendedIdentityUser() {
            ApiKeys = new List<TApiKey>();
            AuthenticationTokens = new List<TAuthenticationToken>();
        }

        /// <summary>
        /// List of ApiKeys of the user
        /// </summary>
        public virtual ICollection<TApiKey> ApiKeys { get; private set; }
        /// <summary>
        /// List of Authentication Tokens of the user
        /// </summary>
        public virtual ICollection<TAuthenticationToken> AuthenticationTokens { get; private set; }
    }
}
