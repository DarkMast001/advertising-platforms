using RegionTreeLib;

namespace RegionTreeLib.Tests
{
    public class TreeCreateFromFileTests
    {
        [Fact]
        public void LoadDataFromFile()
        {
            RegionTree regionTree = new RegionTree();
            string filePath = "./regions.txt";

            regionTree.createTree(filePath);

            Assert.True(regionTree.isTreeCreated());
        }

        [Fact]
        public void LoadDataFromFileException()
        {
            RegionTree regionTree = new RegionTree();
            string? filePath = "./zxc.txt";

            Assert.ThrowsAsync<FileNotFoundException>(() => regionTree.createTree(filePath));

            filePath = null;

            Assert.ThrowsAsync<FileNotFoundException>(() => regionTree.createTree(filePath));
        }

        [Fact]
        public void LoadNotTXTFileTest()
        {
            RegionTree regionTree = new RegionTree();
            string? filePath = "./regions.json";

            Assert.ThrowsAsync<ArgumentException>(() => regionTree.createTree(filePath));
        }
    }
}