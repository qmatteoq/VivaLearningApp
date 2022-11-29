using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VivaLearningApp.Models;

namespace VivaLearningFunction.Services
{
    public interface ICsvService
    {
        List<LearningContentModel> GetLearningContentsFromCsv(Stream file);
    }
}