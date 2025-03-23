using Microsoft.Extensions.Logging.Abstractions;
using RegionTreeLib;
using System.Threading.Tasks;

namespace WebServiceForAdvertisingPlatforms.Service
{
    public class RegionTreeService
    {
        private RegionTree _regionTree;
        private readonly ILogger<RegionTreeService> _logger;

        //public RegionTreeService()
        //{
        //    _regionTree = new RegionTree();
        //}

        public RegionTreeService(ILogger<RegionTreeService> logger)
        {
            _regionTree = new RegionTree();
            _logger = logger;
        }

        public bool isTreeCreated()
        {
            return _regionTree.isTreeCreated();
        }

        public async Task LoadDataFromFile(string filePath)
        {
            try
            {
                _logger.LogInformation($"Loading data from file: {filePath}");
                await _regionTree.createTree(filePath);
                _logger.LogInformation("Data successfully loaded.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "File not found: {FilePath}", filePath);
                throw;
            }
        }

        public List<string> GetAdsByRegion(string region)
        {
            _logger.LogInformation($"Searching ads for region: {region}");
            var ads = _regionTree.findNote(region);
            if (ads.Count == 0)
            {
                _logger.LogWarning($"No ads found for region: {region}");
            }
            _logger.LogInformation($"Аd was successfully found: {region}");
            return ads;
            //return _regionTree.findNote(region);
        }
    }
}
