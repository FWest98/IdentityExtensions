using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace FWest98.IdentityExtensions {
    /// <summary>
    /// UserManager implementation with support for ApiKeys, but not using them
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    public class ExtendedUserManager<TUser> : ExtendedUserManager<TUser, string> where TUser : class, IUser {
        public ExtendedUserManager(IUserStore<TUser, string> store) : base(store) { } 
    }

    /// <summary>
    /// UserManager implementation with support for ApiKeys, but not using them
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class ExtendedUserManager<TUser, TKey> : ExtendedUserManager<ISimpleApiKey<TKey>, TUser, TKey> where TUser : class, IUser<TKey> where TKey : IEquatable<TKey> {
        public ExtendedUserManager(IUserStore<TUser, TKey> store) : base(store) {}
    }

    /// <summary>
    /// UserManager implementation with support for ApiKeys and the setup necessary for it
    /// </summary>
    /// <typeparam name="TApiKey"></typeparam>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class ExtendedUserManager<TApiKey, TUser, TKey> : UserManager<TUser, TKey> where TUser : class, IUser<TKey> where TKey : IEquatable<TKey> where TApiKey : class, ISimpleApiKey<TKey> {
        public ExtendedUserManager(IUserStore<TUser, TKey> store) : base(store) { }

        #region ApiKeys

        /// <summary>
        /// Whether the UserStore is an IUserApiKeyStore
        /// </summary>
        public virtual bool SupportsUserApiKey => Store is IUserApiKeyStore<TApiKey, TUser, TKey>;

        private IUserApiKeyStore<TApiKey, TUser, TKey> GetUserApiKeyStore() {
            var store = Store as IUserApiKeyStore<TApiKey, TUser, TKey>;
            if (store == null) throw new NotSupportedException("No ApiKeys setup");
            return store;
        } 

        /// <summary>
        /// Returns the user associated with the ApiKey containing the given public key
        /// </summary>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public virtual async Task<TUser> FindByPublicKeyAsync(string publicKey) {
            var store = GetUserApiKeyStore();

            var key = await store.FindKeyByPublicKeyAsync(publicKey);
            if(key == null) throw new InvalidOperationException($"PublicKey {publicKey} not found");

            return await store.FindByApiKeyAsync(key);
        }

        /// <summary>
        /// Returns all ApiKeys associated with the user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<IList<TApiKey>> GetApiKeysAsync(TKey userId) {
            var store = GetUserApiKeyStore();

            var user = await FindByIdAsync(userId);
            if(user == null) throw new InvalidOperationException($"UserId {userId} not found");

            return await store.GetApiKeysAsync(user);
        }

        /// <summary>
        /// Returns whether the given public key belongs to an existing ApiKey
        /// Throws an error if TApiKey is an implementation of IExtendedApiKey, then check the private key as well
        /// </summary>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public virtual async Task<bool> CheckApiKeyAsync(string publicKey) {
            var store = GetUserApiKeyStore();

            if (typeof(IExtendedApiKey<TKey>).IsAssignableFrom(typeof(TApiKey)))
                throw new NotSupportedException("Implementation requires private key as well. Use different overload");

            var key = await store.FindKeyByPublicKeyAsync(publicKey);
            return key != null;
        }

        /// <summary>
        /// Returns whether the given key combination belongs to an existing ApiKey
        /// Throws an error if TApiKey is not an implementation of IExtendedApiKey, since then there is no private key
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public virtual async Task<bool> CheckApiKeyAsync(string publicKey, string privateKey) {
            var store = GetUserApiKeyStore();

            if (!typeof(IExtendedApiKey<TKey>).IsAssignableFrom(typeof(TApiKey)))
                throw new NotSupportedException("Implementation is not compatible with private keys. Use different overload");

            var key = await store.FindKeyByPublicKeyAsync(publicKey) as IExtendedApiKey<TKey>;
            return key != null && key.PrivateKey == privateKey;
        }

        /// <summary>
        /// Removes the apikey from the user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public virtual async Task RemoveApiKeyAsync(TUser user, TApiKey apiKey) {
            var store = GetUserApiKeyStore();

            await store.RemoveApiKeyAsync(user, apiKey);
        }

        /// <summary>
        /// Creates a new ApiKey for the given user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual async Task<TApiKey> CreateApiKeyAsync(TUser user) {
            var store = GetUserApiKeyStore();

            return await store.CreateNewApiKeyAsync(user);
        }

        #endregion
        #region AuthenticationToken

        /// <summary>
        /// Whether the UserStore is an IUserAuthenticationTokenStore
        /// </summary>
        public virtual bool SupportsAuthenticationToken => Store is IUserAuthenticationTokenStore<TUser, TKey>;

        private IUserAuthenticationTokenStore<TUser, TKey> GetUserAuthenticationTokenStore() {
            var store = Store as IUserAuthenticationTokenStore<TUser, TKey>;
            if(store == null) throw new NotSupportedException("No AuthenticationTokens setup");
            return store;
        }

        /// <summary>
        /// Returns the user associated with the given token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual async Task<TUser> FindByTokenAsync(string token) {
            var store = GetUserAuthenticationTokenStore();

            var authenticationToken = await store.FindTokenAsync(token);
            if(authenticationToken == null) throw new InvalidOperationException($"AuthenticationToken {token} not found");

            return await store.FindByTokenAsync(authenticationToken);
        }

        /// <summary>
        /// Returns all authentication tokens for the given user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<IList<IAuthenticationToken>> GetAuthenticationTokensAsync(TKey userId) {
            var store = GetUserAuthenticationTokenStore();

            var user = await FindByIdAsync(userId);
            if(user == null) throw new InvalidOperationException($"UserId {userId} not found");

            return await store.GetAuthenticationTokensAsync(user);
        }

        /// <summary>
        /// Returns whether the given token is valid for the given user.
        /// Also 'uses' the token once
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual async Task<bool> ValidateAuthenticationTokenAsync(TKey userId, string token) {
            var store = GetUserAuthenticationTokenStore();

            var authenticationToken = await store.FindTokenAsync(token);
            if (authenticationToken == null) throw new InvalidOperationException($"AuthenticationToken {token} not found");

            var user = await FindByIdAsync(userId);
            if (user == null) throw new InvalidOperationException($"UserId {userId} not found");

            return await store.ValidateAuthenticationTokenAsync(user, authenticationToken);
        }

        /// <summary>
        /// Issues a new authentication token for the given user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="duration"></param>
        /// <param name="maxUses"></param>
        /// <returns></returns>
        public virtual async Task<IAuthenticationToken> IssueAuthenticationTokenAsync(TUser user, TimeSpan duration, int maxUses) {
            var store = GetUserAuthenticationTokenStore();

            return await store.IssueNewAuthenticationTokenAsync(user, duration, maxUses);
        }

        /// <summary>
        /// Invalidates the authentication token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public virtual async Task RemoveAuthenticationTokenAsync(IAuthenticationToken token) {
            var store = GetUserAuthenticationTokenStore();

            await store.InvalidateAuthenticationTokenAsync(token);
        }

        #endregion
    }
}
