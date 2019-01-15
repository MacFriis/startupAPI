using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NSwag.AspNetCore;
using OpenIddict.Validation;
using StartupApi.Filters;
using StartupApi.Infrastructure;
using StartupApi.Model;
using StartupApi.Services;

namespace StartupApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Read info and default values from the appsettings.json file....
            services.Configure<ApiInfo>(Configuration.GetSection("Info"));
            services.Configure<PagingOptions>(Configuration.GetSection("DefaultPagingOptions"));

            // Add data services to the injection
            services.AddScoped<IDataService, DefaultDataService>();
            services.AddScoped<IUsersService, DefaultUserService>();

            // TODO: the appplication database must be customized to meet your needs
            services.AddDbContextPool<AppDbContext>(options =>
            {
                // TODO: make sure that you use the relevant databae for your setup
                // options.UseSqlServer(Configuration.GetConnectionString("MsSqlServer"));

                // TODO: inMemory database is fine for test and some cache senarios but not much else
                options.UseInMemoryDatabase("inMemory");
            });

            // This is the user database and should also be changed to a persistent location
            services.AddDbContextPool<UserDatabaseContext>(options =>
            {
                options.UseInMemoryDatabase("identity");
                options.UseOpenIddict<Guid>();
            });


            services.AddOpenIddict()
                .AddCore(options =>
                {
                    options.UseEntityFrameworkCore()
                    .UseDbContext<UserDatabaseContext>()
                    .ReplaceDefaultEntities<Guid>();
                })
                .AddServer(options =>
                {
                    options.UseMvc();
                    options.EnableTokenEndpoint("/token");
                    options.AllowPasswordFlow();
                    options.AcceptAnonymousClients();
                })
                .AddValidation();

            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIdConnectConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;
            });

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = OpenIddictValidationDefaults.AuthenticationScheme;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("ViewAllUsersPolicy",
                                  p => p.RequireAuthenticatedUser().RequireRole("Admin"));
            });


            services.AddMvc(options =>
            {
                options.Filters.Add<JsonExceptionFilter>();
                options.Filters.Add<RequireHttpsOrCloseAttribute>();
                options.Filters.Add<LinkRewritingFilter>();
            })
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);


            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = new MediaTypeApiVersionReader();
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowMyApp",
                                  policy => policy.AllowAnyOrigin());
            });

            AddIdentityCoreServices(services);

            services.AddAutoMapper(options => options.AddProfile<MappingProfile>());

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errorResponse = new ApiError(context.ModelState);
                    return new BadRequestObjectResult(errorResponse);
                };
            });

        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwaggerUi3();



            }
            else
            {
                app.UseHsts();
            }



            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseCors("AllowMyApp"); // TODO: ......

            app.UseMvc();
        }



        private void AddIdentityCoreServices(IServiceCollection services)
        {
            var builder = services.AddIdentityCore<UserEntity>();
            builder = new IdentityBuilder(builder.UserType, typeof(UserRoleEntity), builder.Services);

            builder.AddRoles<UserRoleEntity>()
                   .AddEntityFrameworkStores<UserDatabaseContext>()
                   .AddDefaultTokenProviders()
                   .AddSignInManager<SignInManager<UserEntity>>();
        }
    }
}
