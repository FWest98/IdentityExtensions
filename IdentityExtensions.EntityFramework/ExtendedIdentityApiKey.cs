using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWest98.IdentityExtensions.EntityFramework {
    /// <summary>
    /// EntityType that represents an extended ApiKey
    /// </summary>
    public class ExtendedIdentityApiKey : ExtendedIdentityApiKey<string> { }

    /// <summary>
    /// EntityType that represents an extended ApiKey
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class ExtendedIdentityApiKey<TKey> : IExtendedApiKey<TKey> where TKey : IEquatable<TKey> {
        /// <summary>
        /// Id of the ApiKey
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual TKey Id { get; set; }

        /// <summary>
        /// Public key
        /// </summary>
        public virtual string PublicKey { get; set; }
        /// <summary>
        /// Private key
        /// </summary>
        public virtual string PrivateKey { get; set; }

        /// <summary>
        /// UserId of the user belonging to this ApiKey
        /// </summary>
        public virtual TKey UserId { get; set; }
    }
}
