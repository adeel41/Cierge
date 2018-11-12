using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cierge.Data;
using Cierge.Options;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;

namespace Cierge
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);

            // Don't initialise database if given the IgnoreInitDb argument
            var ignoreInitDb = args.FirstOrDefault(a => a.ToLower().Contains("ignoreinitdb"));
            if (ignoreInitDb == null)
            {
                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;

                    try
                    {
                        InitializeAsync(services, CancellationToken.None).GetAwaiter().GetResult();
                    }
                    catch (Exception ex)
                    {
                        var logger = services.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "An error occurred seeding the DB.");
                    }
                }
            }


            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();


        private static async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken)
        {
            using (var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
	            var ciergeOption = scope.ServiceProvider.GetRequiredService<IOptions<CiergeOption>>().Value;
	            if (!ciergeOption.InMemoryDb)
		            await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database
			            .MigrateAsync(cancellationToken);

                // Add OpenIddict clients
                await AddOpenIddictClients(cancellationToken, scope.ServiceProvider);

	            // Create roles
				var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                string[] roleNames = { "Administrator" };
                foreach (var roleName in roleNames)
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }
            }
        }

	    private static async Task AddOpenIddictClients(CancellationToken cancellationToken, IServiceProvider serviceProvider)
	    {
		    var iddictManager = serviceProvider.GetRequiredService<OpenIddictApplicationManager<OpenIddictApplication>>();
		    foreach (var client in await iddictManager.ListAsync(100, 0, cancellationToken))
		    {
			    await iddictManager.DeleteAsync(client, cancellationToken);
		    }


		    var oidClients = serviceProvider.GetRequiredService<IOptions<List<OIDCOption>>>().Value;
		    foreach (var oidClient in oidClients)
		    {
			    if (await iddictManager.FindByClientIdAsync(oidClient.ClientId, cancellationToken) == null)
			    {
				    var descriptor = new OpenIddictApplicationDescriptor
				    {
					    ClientId = oidClient.ClientId,
					    DisplayName = oidClient.DisplayName,
					    PostLogoutRedirectUris = {new Uri(oidClient.PostLogoutRedirectUri)},
					    RedirectUris = {new Uri(oidClient.RedirectUri)},
                        Permissions =
                        {
                            OpenIddictConstants.Permissions.Endpoints.Authorization,
                            OpenIddictConstants.Permissions.GrantTypes.Implicit,
                            OpenIddictConstants.Permissions.Scopes.Roles,
                            OpenIddictConstants.Permissions.Scopes.Email,
                            OpenIddictConstants.Permissions.Scopes.Profile,
                            OpenIddictConstants.Scopes.OpenId
                        }
				    };

				    await iddictManager.CreateAsync(descriptor, cancellationToken);
			    }
		    }
	    }
    }
}
