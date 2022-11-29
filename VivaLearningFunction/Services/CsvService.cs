using CsvHelper;
using CsvHelper.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VivaLearningApp.Models;

namespace VivaLearningFunction.Services
{
    public class CsvService : ICsvService
    {
        public List<LearningContentModel> GetLearningContentsFromCsv(Stream file)
        {
            using (var reader = new StreamReader(file))
            {
                CsvConfiguration config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";",
                    TrimOptions = TrimOptions.Trim
                };

                using (var csv = new CsvReader(reader, config))
                {
                    var records = csv.GetRecords<LearningContentModel>();
                    return records.ToList();
                }
            }
        }
    }
}
