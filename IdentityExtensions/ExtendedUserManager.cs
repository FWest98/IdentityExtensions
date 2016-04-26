using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace FWest98.IdentityExtensions {
    public class ExtendedUserManager<TUser, TKey> : ExtendedUserManager<ISimpleApiKey<TKey>, TUser, TKey> where TUser : class, IUser<TKey> where TKey : IEquatable<TKey> {
        public ExtendedUserManager(IUserStore<TUser, TKey> store) : base(store) {}
    }

    public class ExtendedUserManager<TApiKey, TUser, TKey> : UserManager<TUser, TKey> where TUser : class, IUser<TKey> where TKey : IEquatable<TKey> where TApiKey : class, ISimpleApiKey<TKey> {
        public ExtendedUserManager(IUserStore<TUser, TKey> store) : base(store) { }

        public virtual bool SupportsUserApiKey => Store is IUserApiKeyStore<TApiKey, TUser, TKey>;

        public virtual async Task<TUser> FindByPublicKeyAsync(string publicKey) {
            var store = Store as IUserApiKeyStore<TApiKey, TUser, TKey>;
            if(store == null) throw new NotSupportedException("No ApiKeys setup");

            var key = await store.FindKeyByPublicKeyAsync(publicKey);
            if(key == null) throw new InvalidOperationException($"PublicKey {publicKey} not found");

            return await store.FindByApiKeyAsync(key);
        }

        public virtual async Task<IList<TApiKey>> GetApiKeysAsync(TKey userId) {
            var store = Store as IUserApiKeyStore<TApiKey, TUser, TKey>;
            if (store == null) throw new NotSupportedException("No ApiKeys setup");

            var user = await FindByIdAsync(userId);
            if(user == null) throw new InvalidOperationException($"UserId {userId} not found");

            return await store.GetApiKeysAsync(user);
        }

        public virtual async Task<bool> CheckApiKeyAsync(string publicKey) {
            var store = Store as IUserApiKeyStore<TApiKey, TUser, TKey>;
            if (store == null) throw new NotSupportedException("No ApiKeys setup");

            if (typeof(IExtendedApiKey<TKey>).IsAssignableFrom(typeof(TApiKey)))
                throw new NotSupportedException("Implementation requires private key as well. Use different overload");

            var key = await store.FindKeyByPublicKeyAsync(publicKey);
            return key != null;
        }

        public virtual async Task<bool> CheckApiKeyAsync(string publicKey, string privateKey) {
            var store = Store as IUserApiKeyStore<TApiKey, TUser, TKey>;
            if (store == null) throw new NotSupportedException("No ApiKeys setup");

            if (!typeof(IExtendedApiKey<TKey>).IsAssignableFrom(typeof(TApiKey)))
                throw new NotSupportedException("Implementation is not compatible with private keys. Use different overload");

            var key = await store.FindKeyByPublicKeyAsync(publicKey) as IExtendedApiKey<TKey>;
            return key != null && key.PrivateKey == privateKey;
        }
    }
}
