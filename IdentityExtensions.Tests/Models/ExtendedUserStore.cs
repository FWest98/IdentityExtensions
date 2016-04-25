using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace FWest98.IdentityExtensions.Tests.Models {
    public class ExtendedUserStore : IUserApiKeyStore<ExtendedApiKey, ExtendedUser, int> {
        public List<ExtendedUser> Users { get; set; }
        public List<ExtendedApiKey> ApiKeys { get; set; }

        public ExtendedUserStore() {
            Users = new List<ExtendedUser> {
                new ExtendedUser {
                    Id = 1,
                    UserName = "FirstUser",
                    ApiKeys = new List<ExtendedApiKey> {
                        new ExtendedApiKey {
                            Id = 1,
                            PublicKey = "firstUserString1",
                            PrivateKey = "firstUserPrivate1"
                        },
                        new ExtendedApiKey {
                            Id = 2,
                            PublicKey = "firstUserString2",
                            PrivateKey = "firstUserPrivate2"
                        },
                        new ExtendedApiKey {
                            Id = 3,
                            PublicKey = "firstUserString3",
                            PrivateKey = "firstUserPrivate3"
                        }
                    }
                },
                new ExtendedUser {
                    Id = 2,
                    UserName = "SecondUser",
                    ApiKeys = new List<ExtendedApiKey> {
                        new ExtendedApiKey {
                            Id = 4,
                            PublicKey = "secondUserPublic1",
                            PrivateKey = "secondUserPrivate1"
                        },
                        new ExtendedApiKey {
                            Id = 5,
                            PublicKey = "secondUserPublic2",
                            PrivateKey = "secondUserPrivate2"
                        },
                        new ExtendedApiKey {
                            Id = 6,
                            PublicKey = "secondUserPublic3",
                            PrivateKey = "secondUserPrivate3"
                        }
                    }
                }
            };
            ApiKeys = Users.SelectMany(s => s.ApiKeys).ToList();
        }

        public async Task CreateAsync(ExtendedUser user) {
            Users.Add(user);
        }

        public async Task DeleteAsync(ExtendedUser user) {
            if (Users.Contains(user)) Users.Remove(user);
        }

        public async Task<ExtendedUser> FindByIdAsync(int userId) {
            return Users.FirstOrDefault(s => s.Id == userId);
        }
        public async Task<ExtendedUser> FindByNameAsync(string username) {
            return Users.FirstOrDefault(s => s.UserName == username);
        }
        public async Task UpdateAsync(ExtendedUser extendedUser) {
            throw new NotImplementedException();
        }

        public async Task<ExtendedApiKey> CreateNewApiKeyAsync(ExtendedUser user) {
            var newKey = new ExtendedApiKey {
                PublicKey = "somePublic",
                PrivateKey = "somePrivate"
            };
            user.ApiKeys.Add(newKey);
            return newKey;
        }

        public async Task<ExtendedApiKey> FindKeyByIdAsync(int id) {
            return ApiKeys.FirstOrDefault(s => s.Id == id);
        }
        public async Task<ExtendedApiKey> FindKeyByPublicKeyAsync(string publicKey) {
            return ApiKeys.FirstOrDefault(s => s.PublicKey == publicKey);
        }
        public async Task<ExtendedUser> FindByApiKeyAsync(ExtendedApiKey apiKey) {
            return Users.FirstOrDefault(s => s.ApiKeys.Contains(apiKey));
        }
        public async Task<IList<ExtendedApiKey>> GetApiKeysAsync(ExtendedUser user) {
            return user.ApiKeys;
        }
        public async Task RemoveApiKeyAsync(ExtendedUser user, ExtendedApiKey apiKey) {
            ApiKeys.Remove(apiKey);
            user.ApiKeys.Remove(apiKey);
        }

        public void Dispose() { }
    }
}
