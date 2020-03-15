using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using STP.Identity.Application;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using STP.Identity.Persistence.Context;
using STP.Identity.Domain.Entities;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Http;

namespace STP.Identity.Persistence.Seed
{
    public static class SeedData
    {
        public static async Task EnsureSeedData(IServiceProvider provider)
        {
            {
                var userMgr = provider.GetRequiredService<UserManager<User>>();

                if (!await userMgr.Users.AnyAsync())
                {
                    var alice = new User
                    {
                        UserName = "Alice_Popova",
                        Email = "alice@alice.com",
                        FirstName = "Alice",
                        LastName = "Popova",
                        PhoneNumber = "0960001122"
                    };

                    await userMgr.CreateAsync(alice, "Pass123$");
                   


                    var vasy = new User
                    {
                        UserName = "Vasy_Pupkin",
                        Email = "vasy@pupkin.com",
                        FirstName = "Vasy",
                        LastName = "Pupkin",
                        PhoneNumber = "0960001123"
                    };


                    await userMgr.CreateAsync(vasy, "Pass123$");

                    var adolf = new User
                    {
                        UserName = "Adolf",
                        Email = "Adolf@gmail.com",
                        FirstName = "Adolf",
                        LastName = "Hitler",
                        PhoneNumber = "0960001124"
                    };


                    await userMgr.CreateAsync(adolf, "Furer1488$$");

                    var benito = new User
                    {
                        UserName = "Benito",
                        Email = "Benito@gmail.com",
                        FirstName = "Benito",
                        LastName = "Mussolini",
                        PhoneNumber = "0960001125"
                    };


                    await userMgr.CreateAsync(benito, "Pass123$");

                    var franklin = new User
                    {
                        UserName = "Franklin",
                        Email = "Delano@gmail.com",
                        FirstName = "Delano",
                        LastName = "Roosevelt",
                        PhoneNumber = "0960001126"
                    };


                    await userMgr.CreateAsync(franklin, "Pass123$");

                    var stalin = new User
                    {
                        UserName = "Stalin",
                        Email = "Stalin@gmail.com",
                        FirstName = "Joseph ",
                        LastName = "Stalin",
                        PhoneNumber = "0960001127"
                    };


                    await userMgr.CreateAsync(stalin, "Pass123$");

                    var isoroku = new User
                    {
                        UserName = "Isoroku",
                        Email = "Isoroku@gmail.com",
                        FirstName = "Isoroku ",
                        LastName = "Yamamoto",
                        PhoneNumber = "0960001128"
                    };


                    await userMgr.CreateAsync(isoroku, "Pass123$");

                    var douglas = new User
                    {
                        UserName = "Douglas",
                        Email = "Douglas@gmail.com",
                        FirstName = "Douglas ",
                        LastName = "MacArthur",
                        PhoneNumber = "0960001129"
                    };


                    await userMgr.CreateAsync(douglas, "Pass123$");

                    var heinrich = new User
                    {
                        UserName = "Heinrich",
                        Email = "Heinrich@gmail.com",
                        FirstName = "Heinrich ",
                        LastName = "Himmler",
                        PhoneNumber = "0960001130"
                    };

                    await userMgr.CreateAsync(heinrich, "Pass123$");

                    var hermann = new User
                    {
                        UserName = "Hermann",
                        Email = "Hermann@gmail.com",
                        FirstName = "Hermann",
                        LastName = "Gering",
                        PhoneNumber = "0960001131"
                    };

                    await userMgr.CreateAsync(hermann, "Pass123$");

                    var winston = new User
                    {
                        UserName = "Winston",
                        Email = "Winston@gmail.com",
                        FirstName = "Winston ",
                        LastName = "Churchill",
                        PhoneNumber = "0960001132"
                    };

                    await userMgr.CreateAsync(winston, "Pass123$");

                    var saigo = new User
                    {
                        UserName = "Saigo",
                        Email = "Saigo@gmail.com",
                        FirstName = "Saigo",
                        LastName = "Takamori",
                        PhoneNumber = "0960001133"
                    };

                    await userMgr.CreateAsync(saigo, "Pass123$");
                }
            }

            {
                var context = provider.GetRequiredService<ConfigurationDbContext>();
                if (!await context.Clients.AnyAsync())
                {
                    foreach (var client in Config.GetClients())
                    {
                        await context.Clients.AddAsync(client.ToEntity());
                    }
                    await context.SaveChangesAsync();
                }

                if (!await context.IdentityResources.AnyAsync())
                {
                    foreach (var resource in Config.GetIdentityResources())
                    {
                        await context.IdentityResources.AddAsync(resource.ToEntity());
                    }
                    await context.SaveChangesAsync();
                }

                if (!await context.ApiResources.AnyAsync())
                {
                    foreach (var resource in Config.GetApis())
                    {
                        await context.ApiResources.AddAsync(resource.ToEntity());
                    }
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
