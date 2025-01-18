using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Player.Application.UnitTests;

public abstract class BaseTests
{
    protected static IServiceProvider BuildServices(Action<HostBuilderContext, IServiceCollection>? extras = null)
    {
        return new HostBuilder().ConfigureServices((context, sp) =>
        {
            sp.AddLogging(c => c.AddDebug().AddConsole()).AddValidation();
            extras?.Invoke(context, sp);
        }).Build().Services;
    }
}