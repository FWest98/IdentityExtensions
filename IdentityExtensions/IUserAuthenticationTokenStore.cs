using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace FWest98.IdentityExtensions {
    /// <summary>
    /// Stores general purpose authentication tokens of the user
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    public interface IUserAuthenticationTokenStore<TUser> : IUserAuthenticationTokenStore<TUser, string> where TUser : class, IUser {
        
    }

    /// <summary>
    /// Stores general purpose authentication tokens of the user
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public interface IUserAuthenticationTokenStore<TUser, in TKey> : IUserStore<TUser, TKey> where TUser : class, IUser<TKey> where TKey : IEquatable<TKey> {
        /// <summary>
        /// Issues a new authentication token for the specified user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="duration"></param>
        /// <param name="maxUses"></param>
        /// <returns></returns>
        Task<IAuthenticationToken> IssueNewAuthenticationTokenAsync(TUser user, TimeSpan duration, int maxUses);

        /// <summary>
        /// Get the AuthenticationToken associated with the given string
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IAuthenticationToken> FindTokenAsync(string token);

        /// <summary>
        /// Finds the user associated with the given authentication token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<TUser> FindByTokenAsync(IAuthenticationToken token);

        /// <summary>
        /// Get all authentication tokens for the given user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<IList<IAuthenticationToken>> GetAuthenticationTokensAsync(TUser user);

        /// <summary>
        /// Invalidates the given authentication token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task InvalidateAuthenticationTokenAsync(IAuthenticationToken token);

        /// <summary>
        /// Returns whether the authentication token is valid for the given user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool> ValidateAuthenticationTokenAsync(TUser user, IAuthenticationToken token);
    }
}
