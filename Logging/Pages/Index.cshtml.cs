using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Dell.Infrastructure.Logging.Pages
{
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
}
