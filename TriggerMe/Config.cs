using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriggerMe
{


    }
    public class Config
    {
    public static string HOST_URL = "http://triggerme.net";
    public static string JsClient_Url_Live = "http://triggerme.net";
    public static string JsClient_Url_Dev = "http://localhost:4200";

    public static IEnumerable<ApiResource> GetApiResources()
    {
        var api1 = new ApiResource("api1", "My API");
        api1.ApiSecrets = new List<Secret> { new Secret("secret".Sha256()) };
            return new List<ApiResource>
            {
               api1
            };
       
    }
    public static IEnumerable<Client> GetClients()
        {
        return new List<Client>
    {
        new Client
        {
            ClientId = "native.hybrid",
            AllowedGrantTypes = GrantTypes.Hybrid,
            RedirectUris = { "http://127.0.0.1:7890/" },
            // secret for authentication
            RequirePkce=true,
            RequireConsent=false,
            RequireClientSecret=false,
            
            AccessTokenLifetime=(int)TimeSpan.FromDays(1000).TotalSeconds,
            ClientSecrets=new List<Secret>{ new Secret( "secret".Sha256(), "2017 secret")},
            AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "api1"
                    },AlwaysIncludeUserClaimsInIdToken=true
        },
       
  new Client
{
    ClientId = "js",
                    ClientName = "JavaScript Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    ClientSecrets=new List<Secret>{ new Secret( "secret".Sha256(), "2017 secret")},

                    AllowAccessTokensViaBrowser = true,
                    RequireClientSecret=false,
                    RequireConsent=false,
                   
                    RedirectUris = { $"{JsClient_Url_Live}/welcome",$"{JsClient_Url_Dev}/welcome" },
                    PostLogoutRedirectUris = { $"{JsClient_Url_Live}/welcome",$"{JsClient_Url_Dev}/welcome"  },
                    AllowedCorsOrigins = { JsClient_Url_Live,JsClient_Url_Dev},


                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1"
                    },
    AllowOfflineAccess = true,AlwaysIncludeUserClaimsInIdToken=true
}



        };
        }
    
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
    {
        new IdentityResources.OpenId(),
        new IdentityResources.Profile(),
        new IdentityResources.Email()
    };
        }
    }

