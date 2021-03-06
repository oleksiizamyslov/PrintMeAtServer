using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PrintMeAtServer.Core.Impl;
using PrintMeAtServer.Core.Interfaces;
using PrintMeAtServer.MiddleWare;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace PrintMeAtServer
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
            services.AddControllers();
            services.AddTransient<IMessageProcessor, SimpleMessageProcessor>();
            
            services.AddTransient<IServerConfiguration, ServerConfiguration>();
            services.AddTransient<IRedisConfiguration, RedisConfiguration>();
            services.AddTransient<IPrintMeAtService, PrintMeAtService>();
            services.AddTransient(typeof(ISerializer<>), typeof(Serializer<>));
            services.AddTransient<IMessageQueue, RedisMessageQueue>();
            services.AddTransient<IDateTimeProvider, DateTimeProvider>();
            services.AddTransient<ITimerFactory, TimerFactory>();

            services.AddSingleton<IRedisConnectionFactory, RedisConnectionFactory>();
            services.AddSingleton<ISchedulingService, SchedulingService>(sp =>
            {
                var ret = new SchedulingService(
                    sp.GetService<IMessageQueue>(), 
                    sp.GetService<IMessageProcessor>(),
                    sp.GetService<IDateTimeProvider>(),
                    sp.GetService<ITimerFactory>(),
                    sp.GetService<ILogger<SchedulingService>>());
                ret.Initialize().GetAwaiter().GetResult();
                return ret;
            });

            services.AddTransient<IPrintMeAtService, PrintMeAtService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<ErrorLoggingMiddleware>();
            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
