using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Cinema.Repository;
using Cinema.Domain.Identity;
using Cinema.Domain;
using Cinema.Repository.Interface;
using Cinema.Repository.Implementation;
using Cinema.Service.Interface;
using Cinema.Service.Implementation;
using Cinema.Domain.DomainModels;
using Cinema.Service;
using Stripe;

namespace Cinema.Web
{
    public class Startup
    {
        private EmailSettings emailService;
        public Startup(IConfiguration configuration)
        {
            emailService = new EmailSettings();
            Configuration = configuration;
            Configuration.GetSection("EmailSettings").Bind(emailService);
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<CinemaUser>(options => options.SignIn.RequireConfirmedAccount = true)
                 .AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient<ITicketService, TicketService>();
            services.AddScoped(typeof(IUserRepository), typeof(UserRepository));
            services.AddScoped(typeof(IOrderRepository), typeof(OrderRepository));
            services.AddTransient<IShoppingCartService, ShoppingCartService>();
            services.AddTransient<IOrderService, Cinema.Service.Implementation.OrderService>();

            /*  services.AddScoped<EmailSettings>(es => emailService);
              services.AddScoped<IEmailService, EmailService>(email => new EmailService(emailService));
              services.AddScoped<IBackgroundEmailSender, BackgroundEmailSender>();
              services.AddHostedService<ConsumeScopedHostedService>();
  */
            services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            StripeConfiguration.SetApiKey(Configuration.GetSection("Stripe")["SecretKey"]);
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
            CreateRoles(serviceProvider);
        }

        private void CreateRoles(IServiceProvider serviceProvider)
        {

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<CinemaUser>>();
            Task<IdentityResult> roleResult;
            string email = "admin@admin.com";

            //Check that there is an Administrator role and create if not
            Task<bool> hasAdminRole = roleManager.RoleExistsAsync("Administrator");
            hasAdminRole.Wait();
            if (!hasAdminRole.Result)
            {
                roleResult = roleManager.CreateAsync(new IdentityRole("Administrator"));
                roleResult.Wait();
            }
            Task<bool> hasUserRole = roleManager.RoleExistsAsync("StandardUser");
            hasUserRole.Wait();

            if (!hasUserRole.Result)
            {
                roleResult = roleManager.CreateAsync(new IdentityRole("StandardUser"));
                roleResult.Wait();
            }

            //Check if the admin user exists and create it if not
            //Add to the Administrator role

            Task<CinemaUser> testUser = userManager.FindByEmailAsync(email);
            testUser.Wait();

            if (testUser.Result == null)
            {
                CinemaUser administrator = new CinemaUser();
                administrator.Email = email;
                administrator.UserName = email;
                administrator.EmailConfirmed = true;
                administrator.UserCart= new ShoppingCart();

                Task<IdentityResult> newUser = userManager.CreateAsync(administrator, "_AStrongP@ssword1!");
                newUser.Wait();

                if (newUser.Result.Succeeded)
                {
                    Task<IdentityResult> newUserRole = userManager.AddToRoleAsync(administrator, "Administrator");
                    newUserRole.Wait();
                }
            }

        }
        }
    }
