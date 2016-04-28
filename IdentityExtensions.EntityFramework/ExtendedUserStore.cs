using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FWest98.IdentityExtensions.EntityFramework {
    /// <summary>
    /// EntityFramework based user store implementation that supports IUserStore, IUserLoginStore, IUserClaimStore,
    /// IUserRoleStore, IUserApiKeyStore and IUserAuthenticationTokenStore
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    public class ExtendedUserStore<TUser> : ExtendedUserStore<TUser, IdentityRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim, ExtendedIdentityApiKey, ExtendedIdentityAuthenticationToken> where TUser : ExtendedIdentityUser {
        /// <summary>
        /// Default constructor which uses a new instance of the default ExtendedIdentityDbContext
        /// </summary>
        public ExtendedUserStore() : this(new ExtendedIdentityDbContext()) {
            DisposeContext = true;
        }

        /// <summary>
        /// Constructor using custom dbcontext
        /// </summary>
        /// <param name="context"></param>
        public ExtendedUserStore(DbContext context) : base(context) { }
    }

    /// <summary>
    /// EntityFramework based user store implementation that supports IUserStore, IUserLoginStore, IUserClaimStore,
    /// IUserRoleStore, IUserApiKeyStore and IUserAuthenticationTokenStore
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TRole"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TUserLogin"></typeparam>
    /// <typeparam name="TUserRole"></typeparam>
    /// <typeparam name="TUserClaim"></typeparam>
    /// <typeparam name="TApiKey"></typeparam>
    /// <typeparam name="TAuthenticationToken"></typeparam>
    public class ExtendedUserStore<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim, TApiKey, TAuthenticationToken> 
        : UserStore<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim>,
        IUserAuthenticationTokenStore<TUser, TKey>, IUserApiKeyStore<TApiKey, TUser, TKey>
        where TUser : ExtendedIdentityUser<TKey, TUserLogin, TUserRole, TUserClaim, TApiKey, TAuthenticationToken>
        where TRole : IdentityRole<TKey, TUserRole>
        where TKey : IEquatable<TKey>
        where TUserLogin : IdentityUserLogin<TKey>, new()
        where TUserRole : IdentityUserRole<TKey>, new()
        where TUserClaim : IdentityUserClaim<TKey>, new()
        where TApiKey : ExtendedIdentityApiKey<TKey>, new()
        where TAuthenticationToken : ExtendedIdentityAuthenticationToken<TKey>, new() {

        private readonly IDbSet<TApiKey> _apiKeyStore;
        private readonly IDbSet<TAuthenticationToken> _authenticationTokenStore;
        
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="context"></param>
        public ExtendedUserStore(DbContext context) : base(context) {
            _apiKeyStore = Context.Set<TApiKey>();
            _authenticationTokenStore = Context.Set<TAuthenticationToken>();
        }

        #region AuthenticationToken

        /// <summary>
        /// Issues a new authentication for the specified user
        /// </summary>
        /// <param name="user">The user to issue the token to</param>
        /// <param name="duration">The period in which the token is valid</param>
        /// <param name="maxUses">The maximal number of uses for this token before it becomes invalid
        /// 0 for unlimited</param>
        /// <returns></returns>
        public async Task<IAuthenticationToken> IssueNewAuthenticationTokenAsync(TUser user, TimeSpan duration, int maxUses) {
            if(user == null) throw new ArgumentNullException(nameof(user));
            if(duration == null) throw new ArgumentNullException(nameof(duration));

            var newToken = new TAuthenticationToken {
                UserId = user.Id,
                Issued = DateTime.Now,
                IsActive = true,
                Token = GetRandomString(),
                Expired = DateTime.Now + duration,
                MaximalUses = maxUses,
                CurrentUses = 0
            };
            _authenticationTokenStore.Add(newToken);
            user.AuthenticationTokens.Add(newToken);
            await SaveChanges();
            return newToken;
        }

        /// <summary>
        /// Get the AuthenticationToken associated with the given string
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<IAuthenticationToken> FindTokenAsync(string token) {
            if(string.IsNullOrEmpty(token)) throw new ArgumentNullException(nameof(token));
            return await _authenticationTokenStore.FirstOrDefaultAsync(s => s.Token == token);
        }

        /// <summary>
        /// Finds the user associated with the given authentication token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<TUser> FindByTokenAsync(IAuthenticationToken token) {
            if(token == null) throw new ArgumentNullException(nameof(token));
            return await GetUserAggregateAsync(s => s.AuthenticationTokens.Any(a => a.Token == token.Token));
        }

        /// <summary>
        /// Get all authentication tokens for the given user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<IList<IAuthenticationToken>> GetAuthenticationTokensAsync(TUser user) {
            if(user == null) throw new ArgumentNullException(nameof(user));
            await EnsureAuthenticationTokensLoaded(user);
            return user.AuthenticationTokens.Cast<IAuthenticationToken>().ToList();
        }

        /// <summary>
        /// Invalidates the given authentication token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task InvalidateAuthenticationTokenAsync(IAuthenticationToken token) {
            if(token == null) throw new ArgumentNullException(nameof(token));
            if(!(token is TAuthenticationToken)) throw new InvalidOperationException(nameof(token) + " is not an " + typeof(TAuthenticationToken));

            ((TAuthenticationToken) token).IsActive = false;
            await SaveChanges();
        }

        /// <summary>
        /// Returns whether the authentication token is valid for the given user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<bool> ValidateAuthenticationTokenAsync(TUser user, IAuthenticationToken token) {
            if(token == null) throw new ArgumentNullException(nameof(token));
            if(!(token is TAuthenticationToken)) throw new InvalidOperationException(nameof(token) + " is not an " + typeof(TAuthenticationToken));

            var castToken = (TAuthenticationToken) token;
            if (castToken.MaximalUses == 0) return castToken.IsValid;
            if (!castToken.IsValid) return false;

            castToken.CurrentUses++;
            castToken.IsActive = castToken.CurrentUses < castToken.MaximalUses;
            await SaveChanges();

            return true;
        }

        #endregion
        #region ApiKeys

        /// <summary>
        /// Creates a new ApiKey for the user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<TApiKey> CreateNewApiKeyAsync(TUser user) {
            if(user == null) throw new ArgumentNullException(nameof(user));

            var newKey = new TApiKey {
                UserId = user.Id,
                PublicKey = GetRandomString(),
                PrivateKey = GetRandomString()
            };

            _apiKeyStore.Add(newKey);
            user.ApiKeys.Add(newKey);
            await SaveChanges();

            return newKey;
        }

        /// <summary>
        /// Get an ApiKey with the specified id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TApiKey> FindKeyByIdAsync(TKey id) {
            if(id == null) throw new ArgumentNullException(nameof(id));

            return await _apiKeyStore.FirstOrDefaultAsync(s => s.Id.Equals(id));
        }

        /// <summary>
        /// Get an ApiKey with the specified public key
        /// </summary>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public async Task<TApiKey> FindKeyByPublicKeyAsync(string publicKey) {
            if(string.IsNullOrEmpty(publicKey)) throw new ArgumentNullException(nameof(publicKey));

            return await _apiKeyStore.FirstOrDefaultAsync(s => s.PublicKey == publicKey);
        }

        /// <summary>
        /// Find the user associated with this ApiKey
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public async Task<TUser> FindByApiKeyAsync(TApiKey apiKey) {
            if(apiKey == null) throw new ArgumentNullException(nameof(apiKey));
            return await GetUserAggregateAsync(s => s.ApiKeys.Any(a => a.Id.Equals(apiKey.Id)));
        }

        /// <summary>
        /// Get all ApiKeys for the user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<IList<TApiKey>> GetApiKeysAsync(TUser user) {
            if(user == null) throw new ArgumentNullException(nameof(user));
            await EnsureApiKeysLoaded(user);
            return user.ApiKeys.ToList();
        }

        /// <summary>
        /// Remove the given ApiKey
        /// </summary>
        /// <param name="user"></param>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public async Task RemoveApiKeyAsync(TUser user, TApiKey apiKey) {
            if(user == null) throw new ArgumentNullException(nameof(user));
            if(apiKey == null) throw new ArgumentNullException(nameof(apiKey));
            
            await EnsureApiKeysLoaded(user);

            if(!user.ApiKeys.Any(s => s.Id.Equals(apiKey.Id))) throw new InvalidOperationException("User not attached to ApiKey");

            _apiKeyStore.Remove(apiKey);
            user.ApiKeys.Remove(apiKey);
            
            await SaveChanges();
        }

        #endregion

        protected override async Task<TUser> GetUserAggregateAsync(Expression<Func<TUser, bool>> filter) {
            var user = await base.GetUserAggregateAsync(filter);
            if (user == null) return null;

            await EnsureApiKeysLoaded(user);
            await EnsureAuthenticationTokensLoaded(user);
            return user;
        }

        private bool AreApiKeysLoaded(TUser user) {
            return Context.Entry(user).Collection(s => s.ApiKeys).IsLoaded;
        }

        private bool AreAuthenticationKeysLoaded(TUser user) {
            return Context.Entry(user).Collection(s => s.AuthenticationTokens).IsLoaded;
        }

        private async Task EnsureApiKeysLoaded(TUser user) {
            if(!AreApiKeysLoaded(user)) {
                await _apiKeyStore.Where(s => s.UserId.Equals(user.Id)).LoadAsync();
                Context.Entry(user).Collection(s => s.ApiKeys).IsLoaded = true;
            }
        }

        private async Task EnsureAuthenticationTokensLoaded(TUser user) {
            if(!AreAuthenticationKeysLoaded(user)) {
                await _authenticationTokenStore.Where(s => s.UserId.Equals(user.Id)).LoadAsync();
                Context.Entry(user).Collection(s => s.AuthenticationTokens).IsLoaded = true;
            }
        }

        private async Task SaveChanges() {
            if (AutoSaveChanges)
                await Context.SaveChangesAsync();
        }

        private string GetRandomString() {
            var alg = new SHA512Managed();
            var hash = alg.ComputeHash(Guid.NewGuid().ToByteArray());
            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }
    }
}
