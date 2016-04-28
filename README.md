# IdentityExtensions for .NET Identity 2.0

[![Join the chat at https://gitter.im/FWest98/IdentityExtensions](https://badges.gitter.im/FWest98/IdentityExtensions.svg)](https://gitter.im/FWest98/IdentityExtensions?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
[![Build Status](https://travis-ci.org/FWest98/IdentityExtensions.svg?branch=develop)](https://travis-ci.org/FWest98/IdentityExtensions)

This project is an extension to the .NET Identity framework. It provides extra possibilities in order to make it more versatile and easier to use in a .NET MVC Web Api. Right now it features **API Keys** and **Authentication Tokens** as extensions for the default UserStore.

## Getting started

Since the extensions are written in the same style and use the same workflow as .NET Identity, it is easy to set it up in your project. If you're using the `IdentityDbContext` base implementation provided by Entity Framework, you can start using the new functionality right away!

### Step 1: Install packages
First step is to install the package(s) in your project. To install the base extensions, run the following command in the Package Manager Console
```Console
Install-Package FWest98.IdentityExtensions
```

If you plan on using the Entity Framework base implementations, install that package as well:
```Console
Install-Package FWest98.IdentityExtensions.EntityFramework
```

If you use this last option, see the last step on how to implement this in your current code.

### Step 2: Implement interfaces
After that, all you have to do is modify your current `IUserStore` implementation to implement `IUserApiKeyStore` or `IUserAuthenticationKeyStore`, or both if you want. The `TKey` type parameter should be the same as used for the `IUserStore` (or leave it out if you used the default in `IUserStore`).

### Step 3: Use new UserManager base class
Now the last step is to use the new `ExtendedUserManager` instead of the default `UserManager`. This last class includes all the functionality used for API Keys and Authentication Tokens.

### Step 4: Start using the new functionality
Now you're ready to start using the new functionality that comes with the API Keys and Authentication Tokens! All functionality is built into the `ExtendedUserManager` and documentation on how to use it can be found in the wiki. Also take a look at the samples included!

### Entity Framework users
Since all new functionality is already built-in in the IdentityExtensions.EntityFramework package, you just have to change the classes you use/extend to the extended classes.
If you extended the `IdentityUser` class, make sure it now extends the `ExtendedIdentityUser` class. Use `ExtendedIdentityDbContext` instead of `IdentityDbContext` and pass an instance of `ExtendedUserStore` to your `UserManager`.

## Two versions of API Keys
This package provides two interfaces for an API Key implementation, `ISimpleApiKey` and `IExtendedApiKey`. The second extends the first with an extra `PrivateKey` field, next to the `PublicKey` field. This second version can be used to implement for example HMAC Authentication in your application, while the first is more simple to use. The `ExtendedUserManager` automatically distuinguishes between both versions when validating a given API Key using `ExtendedUserManager.CheckApiKeyAsync()` and you should pass only a public key when you just use the simple API key and you should pass both a public and private key when using the extended API key.

## FAQ
### The default framework already features an UserTokenProvider, what is the difference?
.NET Identity 2.0 indeed contains similar functionality with its `IUserTokenProvider` but that implementation is less flexible. It is meant to provide one-time-use codes for email- or phone number-validation and provides less control. You can not by default revoke one token, it is all or nothing. Also, the tokens don't feature some expiration-principle.
This AuthenticationToken is really meant for authentication. It is possible to revoke tokens independent of each other, you can set expiration dates and maximum number of usages.

### How to authenticate a user using an API Key or Authentication token?
You can use, for example, a custom attribute implementing IAuthenticationFilter. This attribute then takes the role of the OWIN Cookie detection. It assigns a principal to the current thread. Such an attribute might look like this:

```c#
public class ApiKeyAuthenticationAttribute : Attribute, IAuthenticationFilter {
    public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken token) {

        /* ... Get the public (and private) key(s) in some way ... */
        /* ... make an instance of some DbContext ... */

        var userManager = new YourUserManager(dbContext);
        var user = await userManager.FindByPublicKeyAsync(publicKey);
        if(!(await userManager.CheckApiKeyAsync(publicKey, privateKey))) {
            // Handle invalid request
        }
        var identity = await userManager.CreateIdentityAsync(user, "APIKeyAuthentication");
        var principal = new ClaimsPrincipal(identity);
        context.Principal = principal;
    }
    public async Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken) {
        /* Handle errors */
    }
}
```

This attribute does essentially the same things as the default OWIN Cookie detection, and therefore you can use this in combination with the known `Authorize` attributes. You can do something similar for the authentication tokens. More information on the `IAuthenticationFilter` and in general, authentication attributes can be found in [this blog on MSDN](https://msdn.microsoft.com/en-us/magazine/dn781361.aspx?f=255&MSPPError=-2147217396).