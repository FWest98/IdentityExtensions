using System;
using System.Linq;
using System.Threading.Tasks;
using FWest98.IdentityExtensions.Tests.Models;
using NUnit.Framework;

#pragma warning disable CS1998
namespace FWest98.IdentityExtensions.Tests {
    [TestFixture]
    public class AuthenticationTokenTests {
        private ExtendedUserStore _userStore;
        private ExtendedUserManager<ExtendedUser, int> _manager;
        
        [SetUp]
        public void Initialize() {
            _userStore = new ExtendedUserStore();
            _manager = new ExtendedUserManager<ExtendedUser, int>(_userStore);
        }

        [Test]
        public void TestSupportsAuthenticationToken() {
            Assert.IsTrue(_manager.SupportsAuthenticationToken);
        }

        [Test]
        public async Task TestGetAuthenticationTokens() {
            var firstUser = _userStore.Users.First();

            Assert.AreEqual(await _manager.GetAuthenticationTokensAsync(firstUser.Id), firstUser.AuthenticationTokens);
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _manager.GetAuthenticationTokensAsync(-1));
        }

        [Test]
        public async Task TestByToken() {
            var firstUser = _userStore.Users.First();
            var firstToken = firstUser.AuthenticationTokens.First();

            Assert.AreEqual(firstUser, await _manager.FindByTokenAsync(firstToken.Token));
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _manager.FindByTokenAsync("nonExisting"));
        }

        [Test]
        public async Task TestNotUsingAuthenticationTokens() {
            var manager = new ExtendedUserManager<SimpleUser, int>(new SimpleUserStore());

            Assert.IsFalse(manager.SupportsAuthenticationToken);
            Assert.ThrowsAsync<NotSupportedException>(async () => await manager.FindByTokenAsync(""));
            Assert.ThrowsAsync<NotSupportedException>(async () => await manager.GetAuthenticationTokensAsync(0));
            Assert.ThrowsAsync<NotSupportedException>(async () => await manager.ValidateAuthenticationTokenAsync(0, ""));
            Assert.ThrowsAsync<NotSupportedException>(async () => await manager.IssueAuthenticationTokenAsync(null, TimeSpan.Zero, 0));
            Assert.ThrowsAsync<NotSupportedException>(async () => await manager.RemoveAuthenticationTokenAsync(null));
        }

        [Test]
        public async Task TestValidateTokens() {
            var user = _userStore.Users.First();
            var token = user.AuthenticationTokens.First();

            Assert.IsTrue(token.IsValid);

            Assert.ThrowsAsync<InvalidOperationException>(async () => await _manager.ValidateAuthenticationTokenAsync(0, ""));
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _manager.ValidateAuthenticationTokenAsync(1, ""));

            var result = await _manager.ValidateAuthenticationTokenAsync(1, "firstUserfirstToken");
            Assert.IsTrue(result);

            // Should expire after usage
            result = await _manager.ValidateAuthenticationTokenAsync(1, "firstUserfirstToken");
            Assert.IsFalse(result);
        }

        [Test]
        public async Task TestIssueToken() {
            var user = _userStore.Users.First();

            var newToken = await _manager.IssueAuthenticationTokenAsync(user, TimeSpan.FromDays(1), 0);
            var result = await _manager.ValidateAuthenticationTokenAsync(user.Id, newToken.Token);
            Assert.IsTrue(result);
            Assert.AreEqual(newToken.Token, "newToken");
        }

        [Test]
        public async Task TestDeletion() {
            var user = _userStore.Users.First();
            var token = user.AuthenticationTokens.First();

            Assert.IsTrue(token.IsValid);
            await _manager.RemoveAuthenticationTokenAsync(token);
            Assert.IsFalse(token.IsValid);

            var result = await _manager.ValidateAuthenticationTokenAsync(user.Id, token.Token);
            Assert.IsFalse(result);
        }
    }
}
#pragma warning restore CS1998