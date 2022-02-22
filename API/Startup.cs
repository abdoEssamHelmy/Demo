using System;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using DataAccessLayer;
using Serilog;
using APIs.Middlewares.Logging;
using System.Reflection;
using Business;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyModel;
using Serilog.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Models.AppConfig;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace API
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }
        private AppConfig _config;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            SetAppSettingsFile();

            services.AddAutoMapper(typeof(Startup));

            services.AddControllers();
            services.AddSingleton(_config);
            services.RegisterBusinessDI();
            services.RegisterDataAccessLayer(_config.DBConfig.ConnectionString);
            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ILoggerProvider>(sp =>
            {
                var functionDependencyContext = DependencyContext.Load(typeof(Startup).Assembly);

                var hostConfig = sp.GetRequiredService<IConfiguration>();
                var logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(Configuration)
                    .CreateLogger();

                return new SerilogLoggerProvider(logger, dispose: true);
            });

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _config.SecurityConfig.TokenAudience,
                    ValidAudience = _config.SecurityConfig.TokenIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.SecurityConfig.SigningKey)),
                    TokenDecryptionKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.SecurityConfig.SigningKey)),
                };
            });

            services.AddControllersWithViews()
                .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            // Disable automatic model validation
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = false;
            });

            services.AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v2", new OpenApiInfo
                {
                    Version = "v2",
                    Title = "Demo",
                    Description = "API Documentation",
                    Contact = new OpenApiContact
                    {
                        Name = "Demo",
                        Email = "help@Demo.com",
                    },
                });

                options.EnableAnnotations();

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Basic",
                    In = ParameterLocation.Header,
                    Description = "API Key Authorization along with paramater encryption"
                });
                options.DescribeAllParametersInCamelCase();

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                              new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = "basic"
                                    }
                                },
                                Array.Empty<string>()
                        }
                    });

                // Set the comments path for the Swagger JSON and UI.
                /*
                var ApisXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var ApisXmlPath = Path.Combine(AppContext.BaseDirectory, ApisXmlFile);
                options.IncludeXmlComments(ApisXmlPath);


                var ModelsAssembly = typeof(Merchant).Assembly.GetName().Name;
                var ModelsXmlPath = Path.Combine(AppContext.BaseDirectory, $"{ModelsAssembly}.xml");
                options.IncludeXmlComments(ModelsXmlPath);
                */
            });

            // Add Quartz services
            //services.AddTransient<IJobFactory, SingletonJobFactory>();
            //services.AddTransient<ISchedulerFactory, StdSchedulerFactory>();

            //// Add HostedService
            //services.AddHostedService<QuartzHostedService>();

            //// Add our jobs
            //services.AddTransient<StayAliveJob>();
            //services.AddSingleton(new JobSchedule(jobType: typeof(StayAliveJob),
            //    cronExpression: "0 0/15 * 1/1 * ? *"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseExceptionHandler(new ExceptionHandlerOptions()
            {
                ExceptionHandlingPath = new PathString(@"/exceptions/logs")
            });

            app.UseStaticFiles();

            app.UseDefaultFiles();

            //app.UseHttpsRedirection();

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v2/swagger.json", "Demo");
            });

            app.UseRouting();

            // Global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .WithExposedHeaders("refresh-token")
                .AllowAnyHeader());
            app.Use(async (context, next) => {
                context.Request.EnableBuffering();
                await next();
            });

            app.UseAuthentication();

            app.UseAuthorization();

            // Logging and debugging
            app.UseMiddleware<RequestLoggingMiddleware>();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        // Add appsettings files
        private void SetAppSettingsFile()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var builder = new ConfigurationBuilder()
                                 .SetBasePath(Directory.GetCurrentDirectory())
                                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                                 .AddJsonFile($"appsettings.{env}.json", true, true);
            Configuration = builder.Build();
            _config = Configuration.Get<AppConfig>();




        }
    }
}
