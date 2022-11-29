using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VivaLearningFunction.Services;

namespace VivaLearningFunction
{
    public class TimerFunction
    {
        private readonly ILogger<ApiFunction> _logger;
        private readonly ICsvService _csvService;
        private readonly IMicrosoftGraphService _graphService;
        private readonly IConfiguration _configuration;

        public TimerFunction(ILogger<ApiFunction> log, ICsvService csvService, IMicrosoftGraphService graphService, IConfiguration configuration)
        {
            _logger = log;
            _csvService = csvService;
            _graphService = graphService;
            _configuration = configuration;
        }

        [FunctionName("TimerFunction")]
        public async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer,
            [Blob("learning-contents/learningContents.csv", FileAccess.Read)]Stream csvBlob)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            string learningProviderId = _configuration["Values:LearningProviderId"];

            await _graphService.AcquireAccessTokenAsync();

            var contents = _csvService.GetLearningContentsFromCsv(csvBlob);
            foreach (var content in contents)
            {
                await _graphService.AddLearningContent(learningProviderId, content);
            }
        }
    }
}
