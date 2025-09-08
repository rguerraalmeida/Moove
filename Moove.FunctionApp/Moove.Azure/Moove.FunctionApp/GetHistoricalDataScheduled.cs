using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Moove.FunctionApp
{
    public class GetHistoricalDataScheduled
    {
        private readonly ILogger _logger;

        public GetHistoricalDataScheduled(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GetHistoricalDataScheduled>();
        }

        [Function("Function1")]
        public void Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            
            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
        }
    }
}
