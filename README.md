This sample contains how to:
* Use default dotnet core logging in Development environment
* For other environments, use Serilog to print a multiline text message as a single line so the PCF Logger can ingest it as a single log event
* Use PCF Apps Manager to change log levels at runtime

Install the nuget packages
```
install-package Serilog.AspNetCore -version 2.1.1
install-package Serilog.Formatting.Compact -version 1.0.0
install-package Serilog.Sinks.Console -version 3.1.1
install-package Steeltoe.CloudFoundry.ConnectorCore -version 2.2.0
install-package Steeltoe.Extensions.Logging.DynamicLogger -version 2.2.0
install-package Steeltoe.Management.CloudFoundryCore -version 2.2.0
```

On Solution Explorer, Select Project, Add New Folder Infrastructure

On Solution Explorer, Select Project, Select Folder Infrastructure, Add New Folder Logging

Copy the files 
* LoggerConfigurationExtensions.cs
* LoggingBuilderExtensions.cs
* SerilogConfiguration.cs
* SerilogDynamicLoggerProvider.cs

Edit Program.cs, add this line at the beginning of Main() method
```
SerilogConfiguration.Configure();
```

Edit Program.cs,add these lines after WebHost.CreateDefaultBuilder(args)
```
.ConfigureAppConfiguration((builderContext, config) =>
{
    config.SetBasePath(builderContext.HostingEnvironment.ContentRootPath)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builderContext.HostingEnvironment.EnvironmentName}.json", optional: true)
        .AddCloudFoundry()
        .AddEnvironmentVariables();
})
.ConfigureLogging((context, builder) =>
{
    var env = context.HostingEnvironment.EnvironmentName;
    if ("Development" != env)
    {
        builder.ClearProviders();
        builder.AddSerilogDynamicConsole();
    }
})
```

Edit Startup.cs, add these lines at the end of method ConfigureServices(IServiceCollection services)
```
var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
if ("Development" != env)
{
    services.AddCloudFoundryActuators(Configuration);
}
```

Edit Startup.cs, add these lines at the end of method Configure(IApplicationBuilder app, IHostingEnvironment env)
```
if (!env.IsDevelopment())
{
    app.UseCloudFoundryActuators();
}
```

Edit appsettings.json, add these lines at the bottom
```
  "management": {
    "endpoints": {
      "path": "/cloudfoundryapplication",
      "cloudfoundry": {
        "validateCertificates": false
      }
    }
  }
```

Edit Pages\Index.cshtml.cs, replace the entire class
```
    public class IndexModel : PageModel
    {
        private readonly ILogger _logger;
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }
        public void OnGet()
        {            
            _logger.LogCritical($"Critical{Environment.NewLine}Message");
            _logger.LogError($"Error{Environment.NewLine}Message");
            _logger.LogWarning($"Warning{Environment.NewLine}Message");
            _logger.LogInformation($"Information{Environment.NewLine}Message");
            _logger.LogDebug($"Debug{Environment.NewLine}Message");
            _logger.LogTrace($"Trace{Environment.NewLine}Message");

            // How to test in your local machine
            // Open launchSettings.json Kestrel profile and change ASPNETCORE_ENVIRONMENT=CI 
            // Uncomment the lines below and run the app
            //SerilogConfiguration.SetLogLevel("Default", Serilog.Events.LogEventLevel.Debug);
            //_logger.LogCritical($"Critical{Environment.NewLine}Message");
            //_logger.LogError($"Error{Environment.NewLine}Message");
            //_logger.LogWarning($"Warning{Environment.NewLine}Message");
            //_logger.LogInformation($"Information{Environment.NewLine}Message");
            //_logger.LogDebug($"Debug{Environment.NewLine}Message");
            //_logger.LogTrace($"Trace{Environment.NewLine}Message");
        }
    }
```

Edit launchSettings.json, change the profile name to Kestrel

Change hosting from "IIS Express" to "Kestrel"

Run the app. You should see logs in the console in the dotnet core format
```
info: Microsoft.AspNetCore.DataProtection.KeyManagement.XmlKeyManager[0]
      User profile is available. Using 'C:\Users\Gustavo_Armenta\AppData\Local\ASP.NET\DataProtection-Keys' as key repository and Windows DPAPI to encrypt keys at rest.
Hosting environment: Development
Content root path: C:\Users\Gustavo_Armenta\source\repos\Dell.Infrastructure\Logging
Now listening on: https://localhost:5001
Now listening on: http://localhost:5000
Application started. Press Ctrl+C to shut down.
info: Microsoft.AspNetCore.Hosting.Internal.WebHost[1]
      Request starting HTTP/1.1 GET https://localhost:5001/
info: Microsoft.AspNetCore.Routing.EndpointMiddleware[0]
      Executing endpoint 'Page: /Index'
info: Microsoft.AspNetCore.Mvc.RazorPages.Internal.PageActionInvoker[3]
      Route matched with {page = "/Index"}. Executing page /Index
info: Microsoft.AspNetCore.Mvc.RazorPages.Internal.PageActionInvoker[101]
      Executing handler method Dell.Infrastructure.Logging.Pages.IndexModel.OnGet with arguments ((null)) - ModelState is Valid
crit: Dell.Infrastructure.Logging.Pages.IndexModel[0]
      Critical
      Message
fail: Dell.Infrastructure.Logging.Pages.IndexModel[0]
      Error
      Message
warn: Dell.Infrastructure.Logging.Pages.IndexModel[0]
      Warning
      Message
info: Dell.Infrastructure.Logging.Pages.IndexModel[0]
      Information
      Message
dbug: Dell.Infrastructure.Logging.Pages.IndexModel[0]
      Debug
      Message
info: Microsoft.AspNetCore.Mvc.RazorPages.Internal.PageActionInvoker[102]
      Executed handler method OnGet, returned result .
info: Microsoft.AspNetCore.Mvc.RazorPages.Internal.PageActionInvoker[103]
      Executing an implicit handler method - ModelState is Valid
info: Microsoft.AspNetCore.Mvc.RazorPages.Internal.PageActionInvoker[104]
      Executed an implicit handler method, returned result Microsoft.AspNetCore.Mvc.RazorPages.PageResult.
info: Microsoft.AspNetCore.Mvc.RazorPages.Internal.PageActionInvoker[4]
      Executed page /Index in 136.2204ms
info: Microsoft.AspNetCore.Routing.EndpointMiddleware[1]
      Executed endpoint 'Page: /Index'
info: Microsoft.AspNetCore.Hosting.Internal.WebHost[2]
      Request finished in 254.5363ms 200 text/html; charset=utf-8
```

Edit launchSettings.json, on the profile Kestrel, set ASPNETCORE_ENVIRONMENT=CI 

Run the app. You should see logs in the console in the serilog format
```
Hosting environment: Development2
Content root path: C:\Users\Gustavo_Armenta\source\repos\Dell.Infrastructure\Logging
Now listening on: https://localhost:5001
Now listening on: http://localhost:5000
Application started. Press Ctrl+C to shut down.
{"@t":"2019-04-12T12:40:02.7350394Z","@m":"Critical\r\nMessage","@i":"fb6b7971","@l":"Fatal","SourceContext":"Dell.Infrastructure.Logging.Pages.IndexModel","ActionId":"ae00b1b1-870f-4c34-ad32-b7766143b4df","ActionName":"/Index","RequestId":"0HLLV8FTMFU4M:00000001","RequestPath":"/","CorrelationId":null,"ConnectionId":"0HLLV8FTMFU4M"}
{"@t":"2019-04-12T12:40:02.7487825Z","@m":"Error\r\nMessage","@i":"394d2097","@l":"Error","SourceContext":"Dell.Infrastructure.Logging.Pages.IndexModel","ActionId":"ae00b1b1-870f-4c34-ad32-b7766143b4df","ActionName":"/Index","RequestId":"0HLLV8FTMFU4M:00000001","RequestPath":"/","CorrelationId":null,"ConnectionId":"0HLLV8FTMFU4M"}
{"@t":"2019-04-12T12:40:02.7490005Z","@m":"Warning\r\nMessage","@i":"94ba2d93","@l":"Warning","SourceContext":"Dell.Infrastructure.Logging.Pages.IndexModel","ActionId":"ae00b1b1-870f-4c34-ad32-b7766143b4df","ActionName":"/Index","RequestId":"0HLLV8FTMFU4M:00000001","RequestPath":"/","CorrelationId":null,"ConnectionId":"0HLLV8FTMFU4M"}
```

Edit launchSettings.json, on the profile Kestrel, set ASPNETCORE_ENVIRONMENT=Development

Deploy to PCF. Open Apps Manager, Select the app, Select Logs, Select Configure Logging Levels

Happy Logging!
