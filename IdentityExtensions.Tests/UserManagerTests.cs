using System;
using System.Linq;
using System.Threading.Tasks;
using FWest98.IdentityExtensions.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FWest98.IdentityExtensions.Tests {
    [TestClass]
    public class UserManagerTests {
        private ExtendedUserStore _extendedUserStore;
        private ExtendedUserManager<ExtendedApiKey, ExtendedUser, int> _extendedManager;

        private SimpleUserStore _simpleUserStore;
        private ExtendedUserManager<SimpleApiKey, SimpleUser, int> _simpleManager;

        [TestInitialize]
        public void Initialize() {
            _extendedUserStore = new ExtendedUserStore();
            _extendedManager = new ExtendedUserManager<ExtendedApiKey, ExtendedUser, int>(_extendedUserStore);

            _simpleUserStore = new SimpleUserStore();
            _simpleManager = new ExtendedUserManager<SimpleApiKey, SimpleUser, int>(_simpleUserStore);
        }

        [TestMethod]
        public void TestSupportsApiKey() {
            Assert.IsTrue(_extendedManager.SupportsUserApiKey);
        }

        [TestMethod]
        public async Task TestGetApiKeys() {
            var firstUser = _extendedUserStore.Users.First();

            Assert.AreEqual(await _extendedManager.GetApiKeysAsync(firstUser.Id), firstUser.ApiKeys);

            try {
                await _extendedManager.GetApiKeysAsync(-1);
                Assert.Fail();
            } catch {
                // ignored
            }
        }

        [TestMethod]
        public async Task TestFindByPublicKey() {
            var firstUser = _extendedUserStore.Users.First();
            var firstKey = firstUser.ApiKeys.First();
            Assert.AreEqual(firstUser, await _extendedManager.FindByPublicKeyAsync(firstKey.PublicKey));

            try {
                await _extendedManager.FindByPublicKeyAsync("nonExistingPublicKey");
                Assert.Fail();
            } catch {
                // ignored
            }
        }

        [TestMethod]
        public async Task TestNotUsingApiKeys() {
            var manager = new ExtendedUserManager<ExtendedUser, int>(_extendedUserStore);
            Assert.IsFalse(manager.SupportsUserApiKey);
            try {
                await manager.FindByPublicKeyAsync("");
                Assert.Fail();
            } catch (Exception) {
                // ignored
            }
            try {
                await manager.CheckApiKeyAsync("", "");
                Assert.Fail();
            } catch (Exception) {
                // ignored
            }
            try {
                await manager.CheckApiKeyAsync("");
                Assert.Fail();
            } catch (Exception) {
                // ignored
            }
            try {
                await manager.GetApiKeysAsync(0);
                Assert.Fail();
            } catch(Exception) {
                // ignored
            }
        }

        [TestMethod]
        public async Task TestCheckApiKeys() {
            bool result;

            var extendedUser = _extendedUserStore.Users.First();
            var extendedKey = extendedUser.ApiKeys.First();

            // Try public only
            try {
                result = await _extendedManager.CheckApiKeyAsync(extendedKey.PublicKey);
                Assert.Fail();
            } catch {
                // ignored
            }

            // Try public + private
            result = false;
            try {
                result = await _extendedManager.CheckApiKeyAsync(extendedKey.PublicKey, extendedKey.PrivateKey);
            } catch {
                Assert.Fail();
            }
            Assert.IsTrue(result);

            var simpleUser = _simpleUserStore.Users.First();
            var simpleKey = simpleUser.ApiKeys.First();

            // Try public only
            result = false;
            try {
                result = await _simpleManager.CheckApiKeyAsync(simpleKey.PublicKey);
            } catch {
                Assert.Fail();
            }
            Assert.IsTrue(result);

            // Try public + private
            result = false;
            try {
                result = await _simpleManager.CheckApiKeyAsync(simpleKey.PublicKey, "");
                Assert.Fail();
            } catch {
                // ignored
            }
        }
    }
}
