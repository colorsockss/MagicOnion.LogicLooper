using MagicOnionServer;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureServices(services =>
        {
            services.Configure<HostOptions>(options =>
            {
                options.ShutdownTimeout = TimeSpan.FromSeconds(10);
            });
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
}