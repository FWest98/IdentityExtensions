namespace FWest98.IdentityExtensions {
    /// <summary>
    /// Interface for an ApiKey with a public and private key
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface IExtendedApiKey<out TKey> : ISimpleApiKey<TKey> {
        /// <summary>
        /// Private key used in the ApiKey
        /// </summary>
        string PrivateKey { get; set; }
    }
}
