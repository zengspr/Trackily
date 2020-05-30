using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.AspNetCore.Authorization;
using Trackily.Areas.Identity.Data;
using Trackily.Areas.Identity.Policies.Handlers;
using Trackily.Areas.Identity.Policies.Requirements;
using Trackily.Data;
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Updated default DbContext and DefaultIdentity to TrackilyContext and TrackilyUser. 
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
                    options.Password.RequiredLength = 0;
                    options.Password.RequiredUniqueChars = 0;

                    options.Lockout.AllowedForNewUsers = false;
                });
            }

            services.AddScoped<DbService>();
            services.AddScoped<TicketService>();
            services.AddScoped<UserTicketService>();
            services.AddScoped<CommentService>();
            services.AddScoped<IAuthorizationHandler, EditPrivilegesHandler>();

            services.AddRazorPages().AddRazorRuntimeCompilation(); // Workaround to enable Browser Link in VS2019.

            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    "IsManager",
                    policyBuilder => policyBuilder.RequireClaim("IsManager"));
                options.AddPolicy(
                    "HasEditPrivileges",
                    policyBuilder => policyBuilder.AddRequirements(
                        new EditPrivilegesRequirement()
                    ));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
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
        }
        }
    }
