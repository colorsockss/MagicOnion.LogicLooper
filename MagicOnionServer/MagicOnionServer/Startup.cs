using Cysharp.Threading;
using MagicOnion.Serialization;
using MagicOnion.Serialization.MemoryPack;

namespace MagicOnionServer;

public class Startup(IConfiguration configuration)
{
    public IConfiguration Configuration { get; } = configuration;

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        MagicOnionSerializerProvider.Default = MemoryPackMagicOnionSerializerProvider.Instance;

        // Register a LooperPool to the service container.
        services.AddSingleton<ILogicLooperPool>(_ => new LogicLooperPool(30, Environment.ProcessorCount, RoundRobinLogicLooperPoolBalancer.Instance));
        services.AddHostedService<LoopHostedService>();
        services.AddGrpc();
        services.AddMagicOnion();
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
            app.UseExceptionHandler("/Error");

            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapMagicOnionService();
            endpoints.MapGet("/", () => "Get");
        });
    }
}