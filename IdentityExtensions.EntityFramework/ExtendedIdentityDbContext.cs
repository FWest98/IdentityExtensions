using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FWest98.IdentityExtensions.EntityFramework {
    /// <summary>
    /// Default ExtendedIdentityDbContext that uses the default entity types for Users, Roles, Claims, Logins, ApiKeys and AuthenticationKeys
    /// </summary>
    public class ExtendedIdentityDbContext : ExtendedIdentityDbContext<ExtendedIdentityUser, IdentityRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim, ExtendedIdentityApiKey, ExtendedIdentityAuthenticationToken> {
        /// <summary>
        /// Default constructor which uses the DefaultConnection
        /// </summary>
        public ExtendedIdentityDbContext() : this("DefaultConnection") { }

        /// <summary>
        /// Constructor which taks the connection string to use
        /// </summary>
        /// <param name="nameOrConnectionString"></param>
        public ExtendedIdentityDbContext(string nameOrConnectionString) : base(nameOrConnectionString) { }

        /// <summary>
        /// Constructs a new context instance using the existing connection to connect to a database, and initializes it from
        /// the given model. The connection will not be disposed when the context is disposed is contextOwnsConnection is false
        /// </summary>
        /// <param name="existingConnection"></param>
        /// <param name="model"></param>
        /// <param name="contextOwnsConnection"></param>
        public ExtendedIdentityDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection) : base(existingConnection, model, contextOwnsConnection) { }

        /// <summary>
        /// Constructs a new context instance using conventions to create the name of the database to which a connection will
        /// be made, and initializes it from the given model. The by-convention name is the full name (namespace + class name)
        /// of the derived context class. See the class remarks for how this is used to create a connection.
        /// </summary>
        /// <param name="model"></param>
        public ExtendedIdentityDbContext(DbCompiledModel model) : base(model) { }

        /// <summary>
        /// Constructs a new context instance using the existing connection to connect to a database. The connection will not
        /// be disposed when the context is disposed if contextOwnsConnection is false.
        /// </summary>
        /// <param name="existingConnection"></param>
        /// <param name="contextOwnsConnection"></param>
        public ExtendedIdentityDbContext(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection) { }

        /// <summary>
        /// Contructs a new context instance using the given string as the name or connection string for the database to which 
        /// a connection will be made, and initializes it from the given model. See the class remarks for how this is used
        /// to create a connection.
        /// </summary>
        /// <param name="nameOrConnectionString"></param>
        /// <param name="model"></param>
        public ExtendedIdentityDbContext(string nameOrConnectionString, DbCompiledModel model) : base(nameOrConnectionString, model) { }
    }

    /// <summary>
    /// DbContext which uses a custom user entity with a string primary key
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    public class ExtendedIdentityDbContext<TUser> : ExtendedIdentityDbContext<TUser, IdentityRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim, ExtendedIdentityApiKey, ExtendedIdentityAuthenticationToken>
        where TUser : ExtendedIdentityUser {
        /// <summary>
        /// Default constructor which uses the DefaultConnection
        /// </summary>
        public ExtendedIdentityDbContext() : this("DefaultConnection") { }

        /// <summary>
        /// Constructor which taks the connection string to use
        /// </summary>
        /// <param name="nameOrConnectionString"></param>
        public ExtendedIdentityDbContext(string nameOrConnectionString) : base(nameOrConnectionString) { }

        /// <summary>
        /// Constructs a new context instance using the existing connection to connect to a database, and initializes it from
        /// the given model. The connection will not be disposed when the context is disposed is contextOwnsConnection is false
        /// </summary>
        /// <param name="existingConnection"></param>
        /// <param name="model"></param>
        /// <param name="contextOwnsConnection"></param>
        public ExtendedIdentityDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection) : base(existingConnection, model, contextOwnsConnection) { }

        /// <summary>
        /// Constructs a new context instance using conventions to create the name of the database to which a connection will
        /// be made, and initializes it from the given model. The by-convention name is the full name (namespace + class name)
        /// of the derived context class. See the class remarks for how this is used to create a connection.
        /// </summary>
        /// <param name="model"></param>
        public ExtendedIdentityDbContext(DbCompiledModel model) : base(model) { }

        /// <summary>
        /// Constructs a new context instance using the existing connection to connect to a database. The connection will not
        /// be disposed when the context is disposed if contextOwnsConnection is false.
        /// </summary>
        /// <param name="existingConnection"></param>
        /// <param name="contextOwnsConnection"></param>
        public ExtendedIdentityDbContext(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection) { }

        /// <summary>
        /// Contructs a new context instance using the given string as the name or connection string for the database to which 
        /// a connection will be made, and initializes it from the given model. See the class remarks for how this is used
        /// to create a connection.
        /// </summary>
        /// <param name="nameOrConnectionString"></param>
        /// <param name="model"></param>
        public ExtendedIdentityDbContext(string nameOrConnectionString, DbCompiledModel model) : base(nameOrConnectionString, model) { }
    }

    /// <summary>
    /// Generic ExtendedIdentityDbContext that can be customized with entity types that extend from the base Identity types.
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <typeparam name="TRole"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TUserLogin"></typeparam>
    /// <typeparam name="TUserRole"></typeparam>
    /// <typeparam name="TUserClaim"></typeparam>
    /// <typeparam name="TApiKey"></typeparam>
    /// <typeparam name="TAuthenticationToken"></typeparam>
    public class ExtendedIdentityDbContext<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim, TApiKey, TAuthenticationToken>
        : IdentityDbContext<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim>
        where TUser : ExtendedIdentityUser<TKey, TUserLogin, TUserRole, TUserClaim, TApiKey, TAuthenticationToken>
        where TRole : IdentityRole<TKey, TUserRole>
        where TUserLogin : IdentityUserLogin<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TApiKey : ExtendedIdentityApiKey<TKey>
        where TAuthenticationToken : ExtendedIdentityAuthenticationToken<TKey>
        where TKey : IEquatable<TKey> {
        
        /// <summary>
        /// IDbSet of ApiKeys
        /// </summary>
        public virtual IDbSet<TApiKey> ApiKeys { get; set; }

        /// <summary>
        /// IDbSet of AuthenticationTokens
        /// </summary>
        public virtual IDbSet<TAuthenticationToken> AuthenticationTokens { get; set; }

        /// <summary>
        /// Default constructor which uses the DefaultConnection
        /// </summary>
        public ExtendedIdentityDbContext() : this("DefaultConnection") { }

        /// <summary>
        /// Constructor which taks the connection string to use
        /// </summary>
        /// <param name="nameOrConnectionString"></param>
        public ExtendedIdentityDbContext(string nameOrConnectionString) : base(nameOrConnectionString) { }

        /// <summary>
        /// Constructs a new context instance using the existing connection to connect to a database, and initializes it from
        /// the given model. The connection will not be disposed when the context is disposed is contextOwnsConnection is false
        /// </summary>
        /// <param name="existingConnection"></param>
        /// <param name="model"></param>
        /// <param name="contextOwnsConnection"></param>
        public ExtendedIdentityDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection) : base(existingConnection, model, contextOwnsConnection) { }

        /// <summary>
        /// Constructs a new context instance using conventions to create the name of the database to which a connection will
        /// be made, and initializes it from the given model. The by-convention name is the full name (namespace + class name)
        /// of the derived context class. See the class remarks for how this is used to create a connection.
        /// </summary>
        /// <param name="model"></param>
        public ExtendedIdentityDbContext(DbCompiledModel model) : base(model) { }

        /// <summary>
        /// Constructs a new context instance using the existing connection to connect to a database. The connection will not
        /// be disposed when the context is disposed if contextOwnsConnection is false.
        /// </summary>
        /// <param name="existingConnection"></param>
        /// <param name="contextOwnsConnection"></param>
        public ExtendedIdentityDbContext(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection) { }

        /// <summary>
        /// Contructs a new context instance using the given string as the name or connection string for the database to which 
        /// a connection will be made, and initializes it from the given model. See the class remarks for how this is used
        /// to create a connection.
        /// </summary>
        /// <param name="nameOrConnectionString"></param>
        /// <param name="model"></param>
        public ExtendedIdentityDbContext(string nameOrConnectionString, DbCompiledModel model) : base(nameOrConnectionString, model) { }

        /// <summary>
        /// Maps table names, and sets up relationships between the various user entities
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TUser>().HasMany(s => s.ApiKeys).WithRequired().HasForeignKey(s => s.UserId);
            modelBuilder.Entity<TUser>().HasMany(s => s.AuthenticationTokens).WithRequired().HasForeignKey(s => s.UserId);
        }
    }
}
