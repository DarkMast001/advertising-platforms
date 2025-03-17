using RegionTreeLib;

namespace WebServiceForAdvertisingPlatforms.Service
{
    public class RegionTreeService
    {
        private RegionTree _regionTree;

        public RegionTreeService()
        {
            _regionTree = new RegionTree();
        }

        public bool isTreeCreated()
        {
            return _regionTree.isTreeCreated();
        }

        public void LoadDataFromFile(string filePath)
        {
            try
            {
                _regionTree.createTree(filePath);
            }
            catch
            {
                throw;
            }
        }

        public List<string> GetAdsByRegion(string region)
        {
            return _regionTree.findNote(region);
        }
    }
}
