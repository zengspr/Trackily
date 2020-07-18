using System.Threading.Tasks;
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
using Trackily.Services.Business;
using Trackily.Services.DataAccess;

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
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.User.AllowedUserNameCharacters =
                        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-.";
                    options.User.RequireUniqueEmail = true;
                });
            }

            services.AddScoped<DbService>();
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
                    "ProjectDeletePrivileges",
                    policyBuilder => policyBuilder.AddRequirements(
                        new ProjectDeletePrivilegesRequirement()));
            });

            services.AddScoped<IAuthorizationHandler, TicketEditPrivilegesUserIdHandler>();
            services.AddScoped<IAuthorizationHandler, ProjectEditPrivilegesProjectIdHandler>();
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

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
