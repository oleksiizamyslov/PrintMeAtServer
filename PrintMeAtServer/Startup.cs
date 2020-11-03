using System.Threading.Tasks;
using Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            services.AddTransient<IMessageProcessor, MessageProcessor>();
            
            services.AddTransient<IServerConfiguration, ServerConfiguration>();
            services.AddTransient<IRedisConfiguration, RedisConfiguration>();
            services.AddTransient<IPrintMeService, PrintMeService>();
            services.AddTransient<IMessageSerializer, MessageSerializer>();
            services.AddTransient<IPersistentMessageQueue, PersistentRedisMessageQueue>();

            services.AddSingleton<IRedisConnectionFactory, RedisConnectionFactory>();
            services.AddSingleton<IScheduledMessageService, ScheduledMessageService>(sp =>
            {
                var ret = new ScheduledMessageService(sp.GetService<IMessageProcessor>(),
                    sp.GetService<IPersistentMessageQueue>());
                ret.Initialize().GetAwaiter().GetResult();
                return ret;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
