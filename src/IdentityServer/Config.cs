﻿using Duende.IdentityServer.Models;

namespace IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("auctionApp", "Auction app full access")
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            // interactive client using code flow + pkce
            new Client
            {
                ClientId = "Postman",
                ClientName = "Postman",
                AllowedScopes = { "openid", "profile", "auctionApp"},
                RedirectUris = { "https://getpostman.com/oauth2/callback" },
                ClientSecrets = { new Secret("PostmanSecret".Sha256()) },
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword
            },
        };
}
