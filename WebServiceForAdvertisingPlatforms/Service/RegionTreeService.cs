using RegionTreeLib;

namespace WebServiceForAdvertisingPlatforms.Service
{
    public class RegionTreeService
    {
        private RegionTree _regionTree;

        public void LoadDataFromFile(string filePath)
        {
            _regionTree = new RegionTree(filePath);
        }

        public List<string> GetAdsByRegion(string region)
        {
            return _regionTree.findNote(region) ?? new List<string>();
        }
    }
}
