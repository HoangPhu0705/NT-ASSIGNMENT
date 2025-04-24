using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;

namespace Infrastructure.Data;

public class OpenIdSeeder
{
    
    public async Task SeedOpenIddictDataAsync(IServiceProvider serviceProvider)
    {
        var manager = serviceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        var existingCustomerClient = await manager.FindByClientIdAsync("customer-web-client");
        if (existingCustomerClient == null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "customer-web-client",
                ClientSecret = "customer-secret",
                DisplayName = "Customer Web Portal",
                RedirectUris = { new Uri("https://localhost:7001/signin-oidc") },
                PostLogoutRedirectUris = { new Uri("https://localhost:7001/signout-callback-oidc") },
                Permissions =
                {   
                    OpenIddictConstants.Permissions.Endpoints.Authorization,
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.Endpoints.EndSession,
                    
                    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                    
                    OpenIddictConstants.Permissions.ResponseTypes.Code,
                    
                    OpenIddictConstants.Permissions.Scopes.Email,
                    OpenIddictConstants.Permissions.Scopes.Profile,
                    OpenIddictConstants.Permissions.Scopes.Roles,
                    OpenIddictConstants.Permissions.Prefixes.Scope + "offline_access",
                    OpenIddictConstants.Permissions.Prefixes.Scope + "open_id",
                    OpenIddictConstants.Permissions.Prefixes.Scope + "api",
                }
            });
        }
        var existingAdminClient = await manager.FindByClientIdAsync("admin-web-client");
        if (existingAdminClient == null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "admin-web-client",
                DisplayName = "Admin Portal",
                RedirectUris = { new Uri("http://localhost:5173/callback") },
                PostLogoutRedirectUris = { new Uri("http://localhost:5173/logout-callback") },
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Authorization,
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.Endpoints.EndSession,
                    
                    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                    
                    OpenIddictConstants.Permissions.Scopes.Email,
                    OpenIddictConstants.Permissions.Scopes.Profile,
                    OpenIddictConstants.Permissions.Scopes.Roles,
                    OpenIddictConstants.Permissions.Prefixes.Scope + "offline_access",
                    OpenIddictConstants.Permissions.Prefixes.Scope + "open_id",
                    OpenIddictConstants.Permissions.Prefixes.Scope + "api",
                    
                    OpenIddictConstants.Permissions.ResponseTypes.Code
                },
                Requirements =
                {
                    OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange
                }
            });
        }
    }
}