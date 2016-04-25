using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace FWest98.IdentityExtensions.Tests.Models {
    public class SimpleUserStore : IUserApiKeyStore<SimpleApiKey, SimpleUser, int> {
        public List<SimpleUser> Users { get; set; }
        public List<SimpleApiKey> ApiKeys { get; set; }

        public SimpleUserStore() {
            Users = new List<SimpleUser> {
                new SimpleUser {
                    Id = 1,
                    UserName = "FirstUser",
                    ApiKeys = new List<SimpleApiKey> {
                        new SimpleApiKey {
                            Id = 1,
                            PublicKey = "firstUserString1"
                        },
                        new SimpleApiKey {
                            Id = 2,
                            PublicKey = "firstUserString2"
                        },
                        new SimpleApiKey() {
                            Id = 3,
                            PublicKey = "firstUserString3"
                        }
                    }
                },
                new SimpleUser {
                    Id = 2,
                    UserName = "SecondUser",
                    ApiKeys = new List<SimpleApiKey> {
                        new SimpleApiKey {
                            Id = 4,
                            PublicKey = "secondUserPublic1"
                        },
                        new SimpleApiKey {
                            Id = 5,
                            PublicKey = "secondUserPublic2"
                        },
                        new SimpleApiKey() {
                            Id = 6,
                            PublicKey = "secondUserPublic3"
                        }
                    }
                }
            };
            ApiKeys = Users.SelectMany(s => s.ApiKeys).ToList();
        }

        public async Task CreateAsync(SimpleUser user) {
            Users.Add(user);
        }

        public async Task DeleteAsync(SimpleUser user) {
            if (Users.Contains(user)) Users.Remove(user);
        }

        public async Task<SimpleUser> FindByIdAsync(int userId) {
            return Users.FirstOrDefault(s => s.Id == userId);
        }
        public async Task<SimpleUser> FindByNameAsync(string username) {
            return Users.FirstOrDefault(s => s.UserName == username);
        }
        public async Task UpdateAsync(SimpleUser user) {
            throw new NotImplementedException();
        }

        public async Task<SimpleApiKey> CreateNewApiKeyAsync(SimpleUser user) {
            var newKey = new SimpleApiKey {
                PublicKey = "somePublic"
            };
            user.ApiKeys.Add(newKey);
            return newKey;
        }

        public async Task<SimpleApiKey> FindKeyByIdAsync(int id) {
            return ApiKeys.FirstOrDefault(s => s.Id == id);
        }
        public async Task<SimpleApiKey> FindKeyByPublicKeyAsync(string publicKey) {
            return ApiKeys.FirstOrDefault(s => s.PublicKey == publicKey);
        }
        public async Task<SimpleUser> FindByApiKeyAsync(SimpleApiKey apiKey) {
            return Users.FirstOrDefault(s => s.ApiKeys.Contains(apiKey));
        }
        public async Task<IList<SimpleApiKey>> GetApiKeysAsync(SimpleUser user) {
            return user.ApiKeys;
        }
        public async Task RemoveApiKeyAsync(SimpleUser user, SimpleApiKey apiKey) {
            ApiKeys.Remove(apiKey);
            user.ApiKeys.Remove(apiKey);
        }

        public void Dispose() { }
    }
}
