using System;
using System.Linq;
using System.Threading.Tasks;
using FWest98.IdentityExtensions.Tests.Models;
using NUnit.Framework;

#pragma warning disable CS1998
namespace FWest98.IdentityExtensions.Tests {
    [TestFixture]
    public class ApiKeyManagerTests {
        private ExtendedUserStore _extendedUserStore;
        private ExtendedUserManager<ExtendedApiKey, ExtendedUser, int> _extendedManager;

        private SimpleUserStore _simpleUserStore;
        private ExtendedUserManager<SimpleApiKey, SimpleUser, int> _simpleManager;

        [SetUp]
        public void Initialize() {
            _extendedUserStore = new ExtendedUserStore();
            _extendedManager = new ExtendedUserManager<ExtendedApiKey, ExtendedUser, int>(_extendedUserStore);

            _simpleUserStore = new SimpleUserStore();
            _simpleManager = new ExtendedUserManager<SimpleApiKey, SimpleUser, int>(_simpleUserStore);
        }

        [Test]
        public void TestSupportsApiKey() {
            Assert.IsTrue(_extendedManager.SupportsUserApiKey);
        }

        [Test]
        public async Task TestGetApiKeys() {
            var firstUser = _extendedUserStore.Users.First();

            Assert.AreEqual(await _extendedManager.GetApiKeysAsync(firstUser.Id), firstUser.ApiKeys);
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _extendedManager.GetApiKeysAsync(-1));
        }

        [Test]
        public async Task TestFindByPublicKey() {
            var firstUser = _extendedUserStore.Users.First();
            var firstKey = firstUser.ApiKeys.First();

            Assert.AreEqual(firstUser, await _extendedManager.FindByPublicKeyAsync(firstKey.PublicKey));
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _extendedManager.FindByPublicKeyAsync("nonExisting"));
        }

        [Test]
        public async Task TestNotUsingApiKeys() {
            var manager = new ExtendedUserManager<ExtendedUser, int>(_extendedUserStore);

            Assert.IsFalse(manager.SupportsUserApiKey);
            Assert.ThrowsAsync<NotSupportedException>(async () => await manager.FindByPublicKeyAsync(""));
            Assert.ThrowsAsync<NotSupportedException>(async () => await manager.CheckApiKeyAsync(""));
            Assert.ThrowsAsync<NotSupportedException>(async () => await manager.CheckApiKeyAsync("", ""));
            Assert.ThrowsAsync<NotSupportedException>(async () => await manager.GetApiKeysAsync(0));
            Assert.ThrowsAsync<NotSupportedException>(async () => await manager.RemoveApiKeyAsync(null, null));
            Assert.ThrowsAsync<NotSupportedException>(async () => await manager.CreateApiKeyAsync(null));
        }

        [Test]
        public async Task TestCheckApiKeys() {
            var result = false;

            var extendedUser = _extendedUserStore.Users.First();
            var extendedKey = extendedUser.ApiKeys.First();

            Assert.ThrowsAsync<NotSupportedException>(async () => await _extendedManager.CheckApiKeyAsync(extendedKey.PublicKey));
            Assert.DoesNotThrowAsync(async () => result = await _extendedManager.CheckApiKeyAsync(extendedKey.PublicKey, extendedKey.PrivateKey));
            Assert.IsTrue(result);

            result = await _extendedManager.CheckApiKeyAsync(extendedKey.PrivateKey, extendedKey.PublicKey);
            Assert.IsFalse(result);
            result = await _extendedManager.CheckApiKeyAsync(extendedKey.PublicKey, "");
            Assert.IsFalse(result);

            var simpleUser = _simpleUserStore.Users.First();
            var simpleKey = simpleUser.ApiKeys.First();
            result = false;

            Assert.DoesNotThrowAsync(async () => result = await _simpleManager.CheckApiKeyAsync(simpleKey.PublicKey));
            Assert.IsTrue(result);
            Assert.ThrowsAsync<NotSupportedException>(async () => await _simpleManager.CheckApiKeyAsync(simpleKey.PublicKey, ""));

            result = await _simpleManager.CheckApiKeyAsync("");
            Assert.IsFalse(result);
        }

        [Test]
        public async Task TestCreation() {
            // Test UserStore creates fixed keys

            var extendedUser = _extendedUserStore.Users.First();

            var apiKey = await _extendedManager.CreateApiKeyAsync(extendedUser);
            var result = await _extendedManager.CheckApiKeyAsync(apiKey.PublicKey, apiKey.PrivateKey);
            Assert.IsTrue(result);
            Assert.AreEqual(apiKey.PublicKey, "somePublic");
            Assert.AreEqual(apiKey.PrivateKey, "somePrivate");
        }

        [Test]
        public async Task TestDeletion() {
            var extendedUser = _extendedUserStore.Users.First();
            var extendedKey = extendedUser.ApiKeys.First();

            await _extendedManager.RemoveApiKeyAsync(extendedUser, extendedKey);
            var result = await _extendedManager.CheckApiKeyAsync(extendedKey.PublicKey, extendedKey.PrivateKey);
            Assert.IsFalse(result);
            Assert.IsFalse(_extendedUserStore.Users.First().ApiKeys.Contains(extendedKey));
        }
    }
}
#pragma warning restore CS1998