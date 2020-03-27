namespace ncov.api
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Net.Http.Headers;
    using Microsoft.OpenApi.Models;
    using NCovid.Service.DataContext;
    using NCovid.Service.Hubs;
    using NCovid.Service.Services;


    /// <summary>
    /// Defines the <see cref="Startup" />.
    /// </summary>
    public class Startup
    {
     
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration<see cref="IConfiguration"/>.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Gets the Configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// The Configure.
        /// </summary>
        /// <param name="app">The app<see cref="IApplicationBuilder"/>.</param>
        /// <param name="env">The env<see cref="IWebHostEnvironment"/>.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Covid Info API V1");
                c.RoutePrefix = string.Empty;
            });
            app.UseCors("AnotherPolicy");
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<CoronaHub>("/coronahub");
            });
        }

        /// <summary>
        /// The ConfigureServices.
        /// </summary>
        /// <param name="services">The services<see cref="IServiceCollection"/>.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CoronaDbContext>(ServiceLifetime.Transient);
            services.AddSignalR(hubOptions =>
            {
                hubOptions.EnableDetailedErrors = true;
                //hubOptions.HandshakeTimeout = TimeSpan.FromSeconds(30);
            });
                //.AddRedis(o =>
                //{
                //    o.ConnectionFactory = async writer =>
                //    {
                //        var config = new ConfigurationOptions
                //        {
                //            AbortOnConnectFail = false
                //        };

                //        config.EndPoints.Add(Configuration.GetConnectionString("Redis"));
                //        // config.SetDefaultPorts();
                //        var connection = await ConnectionMultiplexer.ConnectAsync(config, writer).ConfigureAwait(false);
                //        connection.ConnectionFailed +=
                //            (_, e) =>
                //            {

                //            };//Logger.Error(e.Exception, "Connection to Redis failed."); }

                //       // if (!connection.IsConnected) //Logger.Info("Did not connect to Redis.");

                //        return connection;
                //    };
                //});
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("https://localhost:44361", "http://localhost:61687", "https://covid.apical.tk", "http://localhost:8081");

                    });

                options.AddPolicy("AnotherPolicy",
                    builder =>
                    {
                        builder.WithOrigins("https://localhost:44361", "http://localhost:61687", "https://covid.apical.tk", "http://localhost:8081")
                                            .AllowAnyHeader()
                                            .AllowAnyMethod().AllowCredentials(); ;
                    });

            });
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.WriteIndented = true;
                //options.JsonSerializerOptions.PropertyNamingPolicy=JsonNamingPolicy.CamelCase;
            });
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Corona Virus Global Information", Version = "v1" });
            });
            services.AddTransient<ICoronaVirusService, CoronaVirusService>();
        }
    }
}
