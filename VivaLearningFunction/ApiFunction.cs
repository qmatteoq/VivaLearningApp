using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.OpenApi.Models;
using VivaLearningFunction.Services;

namespace VivaLearningFunction
{
    public class ApiFunction
    {
        private readonly ILogger<ApiFunction> _logger;
        private readonly ICsvService _csvService;
        private readonly IMicrosoftGraphService _graphService;
        private readonly IConfiguration _configuration;

        public ApiFunction(ILogger<ApiFunction> log, ICsvService csvService, IMicrosoftGraphService graphService, IConfiguration configuration)
        {
            _logger = log;
            this._csvService = csvService;
            this._graphService = graphService;
            this._configuration = configuration;
        }

        [FunctionName("ApiFunction")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IList<LearningContent>), Description = "The list of learning contents")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            [Blob("learning-contents/LearningContents.csv", FileAccess.Read)] Stream csvBlob)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string learningProviderId = _configuration["Values:LearningProviderId"];
            
            await _graphService.AcquireAccessTokenAsync();

            var contents = _csvService.GetLearningContentsFromCsv(csvBlob);
            foreach (var content in contents)
            {
                await _graphService.AddLearningContent(learningProviderId, content);
            }

            var result = await _graphService.GetLearningContentAsync(learningProviderId);

            return new OkObjectResult(result);
        }
    }
}

