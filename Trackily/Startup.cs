using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Trackily.Areas.Identity.Data;
using Trackily.Areas.Identity.Policies.Handlers;
using Trackily.Areas.Identity.Policies.Requirements;
using Trackily.Services;

namespace Trackily
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TrackilyContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("TrackilyContextConnection")));

            services.AddDefaultIdentity<TrackilyUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<TrackilyContext>();

            if (_env.IsDevelopment())
            {
                services.Configure<IdentityOptions>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 3;
                    options.Password.RequiredUniqueChars = 0;

                    options.Lockout.AllowedForNewUsers = false;
                });
            }
            else
            {
                services.Configure<IdentityOptions>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 5;
                    options.Password.RequiredUniqueChars = 0;
                    options.User.AllowedUserNameCharacters =
                        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-.";
                });
            }

            services.AddScoped<TicketService>();
            services.AddScoped<UserTicketService>();
            services.AddScoped<CommentService>();
            services.AddScoped<ProjectService>();
            services.AddScoped<UserProjectService>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    "TicketEditPrivileges",
                    policyBuilder => policyBuilder.AddRequirements(
                        new TicketEditPrivilegesRequirement()
                    ));
                options.AddPolicy(
                    "ProjectEditPrivileges",
                    policyBuilder => policyBuilder.AddRequirements(
                        new ProjectEditPrivilegesRequirement()));
                options.AddPolicy(
                    "ProjectDetailsPrivileges",
                    policyBuilder => policyBuilder.AddRequirements(
                        new ProjectDetailsPrivilegesRequirement()));
                options.AddPolicy(
                    "ProjectDeletePrivileges",
                    policyBuilder => policyBuilder.AddRequirements(
                        new ProjectDeletePrivilegesRequirement()));
            });

            services.AddScoped<IAuthorizationHandler, TicketEditPrivilegesUserIdHandler>();
            services.AddScoped<IAuthorizationHandler, TicketEditPrivilegesCommentThreadHandler>();
            services.AddScoped<IAuthorizationHandler, TicketEditPrivilegesCommentHandler>();
            services.AddScoped<IAuthorizationHandler, ProjectEditPrivilegesProjectIdHandler>();
            services.AddScoped<IAuthorizationHandler, ProjectDetailsPrivilegesProjectIdHandler>();
            services.AddScoped<IAuthorizationHandler, ProjectDeletePrivilegesProjectIdHandler>();
        }

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<TrackilyUser> userManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404)
                {
                    context.Request.Path = "/Home/Error/404";
                    await next();
                }
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            DbSeeder.SeedUsers(userManager);
        }
    }
}
