using RegionTreeLib;

namespace RegionTreeLib.Tests
{
    public class FindNoteTests
    {
        [Fact]
        public void FindNoteTest()
        {
            RegionTree regionTree = new RegionTree();

            string note1 = "Яндекс.Директ:/ru";
            string note2 = "Ревдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik";
            string note3 = "Газета уральских москвичей:/ru/msk,/ru/permobl,/ru/chelobl";
            string note4 = "Крутая реклама:/ru/svrd";

            regionTree.addNoteToTree(note1);
            regionTree.addNoteToTree(note2);
            regionTree.addNoteToTree(note3);
            regionTree.addNoteToTree(note4);

            Assert.Contains("Яндекс.Директ", regionTree.findNote("/ru"));
            Assert.Equal(new List<string>() { "Яндекс.Директ", "Ревдинский рабочий", "Крутая реклама" }, regionTree.findNote("/ru/svrd/revda"));
            Assert.Equal(new List<string>() { "Яндекс.Директ", "Ревдинский рабочий", "Крутая реклама" }, regionTree.findNote("/ru/svrd/pervik"));
            Assert.Equal(new List<string>() { "Яндекс.Директ", "Газета уральских москвичей" }, regionTree.findNote("/ru/msk"));
            Assert.Equal(new List<string>() { "Яндекс.Директ", "Газета уральских москвичей" }, regionTree.findNote("/ru/permobl"));
            Assert.Equal(new List<string>() { "Яндекс.Директ", "Газета уральских москвичей" }, regionTree.findNote("/ru/chelobl"));
            Assert.Equal(new List<string>() { "Яндекс.Директ", "Крутая реклама" }, regionTree.findNote("/ru/svrd"));
        }

        [Fact]
        public void FindNoteWithWrongPathTest()
        {
            RegionTree regionTree = new RegionTree();

            string note1 = "Яндекс.Директ:/ru";
            string note2 = "Ревдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik";
            string note3 = "Газета уральских москвичей:/ru/msk,/ru/permobl,/ru/chelobl";
            string note4 = "Крутая реклама:/ru/svrd";

            regionTree.addNoteToTree(note1);
            regionTree.addNoteToTree(note2);
            regionTree.addNoteToTree(note3);
            regionTree.addNoteToTree(note4);

            Assert.Empty(regionTree.findNote("zxc"));
            Assert.Equal(new List<string>() { }, regionTree.findNote("/ru/msk/chelobl/voronez"));
            Assert.Empty(regionTree.findNote("/////ru/////"));
            Assert.Empty(regionTree.findNote("ru"));
            Assert.Empty(regionTree.findNote("ru svrd"));
            Assert.Empty(regionTree.findNote("ru,svrd"));
            Assert.Empty(regionTree.findNote("/ru,svrd"));
        }

        [Fact]
        public void FindNoteWithoutRootNameTest()
        {
            RegionTree regionTree = new RegionTree();

            string note1 = "Ревдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik";

            regionTree.addNoteToTree(note1);

            Assert.Contains("Ревдинский рабочий", regionTree.findNote("/ru/svrd/revda"));
            Assert.Contains("Ревдинский рабочий", regionTree.findNote("/ru/svrd/pervik"));
            Assert.Empty(regionTree.findNote("/ru"));
            Assert.Empty(regionTree.findNote("/ru/svrd"));
        }

        [Fact]
        public void FindNoteIfTreeNotExistTest()
        {
            RegionTree regionTree = new RegionTree();

            Assert.Empty(regionTree.findNote("/ru"));
        }
    }
}
