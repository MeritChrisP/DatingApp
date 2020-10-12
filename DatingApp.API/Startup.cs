using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;


namespace DatingApp.API
{
    public class Startup
    {
        //readonly string corsDefaultPolicy = "_corsDefaultPolicy";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
           
           IdentityBuilder builder = services.AddIdentityCore<User>(opt => 
           {
               opt.Password.RequireDigit = false;
               opt.Password.RequiredLength = 4;
               opt.Password.RequireNonAlphanumeric = false;
               opt.Password.RequireUppercase = false;
           });
           
           builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
           builder.AddEntityFrameworkStores<DataContext>();
           builder.AddRoleValidator<RoleValidator<Role>>();
           builder.AddRoleManager<RoleManager<Role>>();
           builder.AddSignInManager<SignInManager<User>>();

           // Setup of the authentication scheme.  Used where the 'attribute' [Authorize] is assigned in the controller (presently).
           services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => 
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        // The token from the "AppSettings" is a string but needs to be encoded to bytes for the SymmetricSecurityKey object.
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                            .GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            services.AddAuthorization(options => 
            {
                options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
                options.AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));
                options.AddPolicy("VIPOnly", policy => policy.RequireRole("VIP"));
            });

           services.AddControllers(options => 
           {
               var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
           });

           services.AddDbContext<DataContext>(x => {
               x.UseLazyLoadingProxies();
               x.UseSqlServer(Configuration.GetConnectionString("DefaultConnectionString"));
           });
           //services.AddControllers().AddNewtonsoftJson();
           services.AddControllers().AddNewtonsoftJson(options =>
            {
                //options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
           services.AddCors();
           services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));
           services.AddAutoMapper(typeof(DatingRepository).Assembly);
           services.AddScoped<IDatingRepository,DatingRepository>();
           services.AddScoped<IPhotoRepository,PhotoRepository>();
           
                      
           
            services.AddScoped<LogUserActivity>();
            
           /*
           services.AddCors(options => 
           {
               options.AddPolicy(name: corsDefaultPolicy,
                                    builder =>
                                    {
                                    builder.AllowAnyOrigin()
                                    .AllowAnyHeader()
                                    .AllowAnyMethod();
                                    });
           });
           */
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Initialising the use of a global exception handler for handling errors around the API.
                app.UseExceptionHandler(builder => {
                    builder.Run(async context => {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            context.Response.AddApplicationError(error.Error.Message);
                            await context.Response.WriteAsync(error.Error.Message);
                        }
                    });
                });
            }

            // app.UseHttpsRedirection();

            app.UseRouting();
            
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            //app.UseCors(corsDefaultPolicy);
            
            app.UseAuthentication();            
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoints.MapFallbackToController("Index","Fallback");
            });
        }
    }
}
