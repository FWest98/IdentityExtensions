using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace FWest98.IdentityExtensions {
    /// <summary>
    /// Stores the ApiKeys of the user
    /// </summary>
    /// <typeparam name="TApiKey"></typeparam>
    /// <typeparam name="TUser"></typeparam>
    public interface IUserApiKeyStore<TApiKey, TUser> : IUserApiKeyStore<TApiKey, TUser, string> where TApiKey : class, ISimpleApiKey where TUser : class, IUser { } 

    /// <summary>
    /// Stores the ApiKeys of the user
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TApiKey"></typeparam>
    public interface IUserApiKeyStore<TApiKey, TUser, in TKey> : IUserStore<TUser, TKey> where TApiKey : class, ISimpleApiKey<TKey> where TUser : class, IUser<TKey> where TKey : IEquatable<TKey> {
        /// <summary>
        /// Creates a new ApiKey for the user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<TApiKey> CreateNewApiKeyAsync(TUser user);

        /// <summary>
        /// Get an ApiKey with the specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TApiKey> FindKeyByIdAsync(TKey id);

        /// <summary>
        /// Get an ApiKey with the specified public key
        /// </summary>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        Task<TApiKey> FindKeyByPublicKeyAsync(string publicKey);

        /// <summary>
        /// Find the user associated with this ApiKey
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        Task<TUser> FindByApiKeyAsync(TApiKey apiKey);

        /// <summary>
        /// Get all ApiKeys for a user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<IList<TApiKey>> GetApiKeysAsync(TUser user);

        /// <summary>
        /// Remove a given ApiKey
        /// </summary>
        /// <param name="user"></param>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        Task RemoveApiKeyAsync(TUser user, TApiKey apiKey);
    }
}
