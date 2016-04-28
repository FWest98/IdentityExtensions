namespace FWest98.IdentityExtensions {
    /// <summary>
    /// Interface for a simple ApiKey with only a public key
    /// </summary>
    public interface ISimpleApiKey : ISimpleApiKey<string> { }

    /// <summary>
    /// Interface for a simple ApiKey with only a public key
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface ISimpleApiKey<out TKey> {
        /// <summary>
        /// Unique identifier for this ApiKey
        /// </summary>
        TKey Id { get; }

        /// <summary>
        /// Public key used in the ApiKey
        /// </summary>
        string PublicKey { get; set; }
    }
}
