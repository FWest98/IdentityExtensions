using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FWest98.IdentityExtensions.EntityFramework {
    public class ExtendedIdentityAuthenticationToken : ExtendedIdentityAuthenticationToken<string> { }

    /// <summary>
    /// EntityType that represents an authentication token
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class ExtendedIdentityAuthenticationToken<TKey> : IAuthenticationToken {
        /// <summary>
        /// Id of the Authentication Token
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }

        /// <summary>
        /// UserId of the user belonging to this token
        /// </summary>
        public virtual TKey UserId { get; set; }

        /// <summary>
        /// Token string
        /// </summary>
        public virtual string Token { get; set; }

        /// <summary>
        /// Whether the token is still valid
        /// </summary>
        [NotMapped]
        public virtual bool IsValid => DateTime.Now > Issued && DateTime.Now < Expired && IsActive;

        /// <summary>
        /// When the token was issued
        /// </summary>
        public virtual DateTime Issued { get; set; }

        /// <summary>
        /// When the token will expire
        /// </summary>
        public virtual DateTime Expired { get; set; }

        /// <summary>
        /// Whether the token is still active/usable
        /// </summary>
        public virtual bool IsActive { get; set; }

        /// <summary>
        /// Maximum number of uses of the token
        /// 0 is unlimited
        /// </summary>
        public virtual int MaximalUses { get; set; }

        /// <summary>
        /// Current number of uses of the token
        /// </summary>
        public virtual int CurrentUses { get; set; }
    }
}
