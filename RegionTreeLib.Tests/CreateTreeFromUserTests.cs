using RegionTreeLib;

namespace RegionTreeLib.Tests
{
    public class CreateTreeFromUserTests
    {
        [Fact]
        public void CreateTreeNodeTest()
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

            Assert.True(regionTree.isTreeCreated());
        }

        [Fact]
        public void CreateTreeWrongRootNodeTest()
        {
            RegionTree regionTree = new RegionTree();

            string note1 = "Яндекс.Директ:/ru";
            string note2 = "Ревдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik";
            string note3 = "Европейская реклама:/eu";

            Assert.Equal("/ru", regionTree.addNoteToTree(note1));
            Assert.Equal("/ru/svrd/pervik", regionTree.addNoteToTree(note2));
            Assert.Throws<Exception>(() => regionTree.addNoteToTree(note3));

            Assert.False(regionTree.isTreeCreated());



            RegionTree regionTree1 = new RegionTree();

            Assert.Equal("/ru", regionTree.addNoteToTree(note1));
            Assert.Throws<Exception>(() => regionTree.addNoteToTree(note3));

            Assert.False(regionTree1.isTreeCreated());
        }

        [Fact]
        public void CreateNodeWithoutRootNodeName()
        {
            RegionTree regionTree = new RegionTree();

            string note1 = "Ревдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik";

            Assert.Equal("/ru/svrd/pervik", regionTree.addNoteToTree(note1));
        }

        [Fact]
        public void CreateWrongNodeName()
        {
            RegionTree regionTree = new RegionTree();

            string note1 = "Яндекс.Директ:/ru";
            string note2 = "Ревдинский рабочий/ru/svrd/revda";
            string note3 = "Ревдинский рабочий:ru svrd revda";
            string note4 = "Ревдинский рабочий:////ru/////svrd///////revda";
            string note5 = "Ревдинский рабочий:/ru/svrd/revda,,,,,/ru/svrd/pervik";
            string note6 = "Ревдинский рабочий:/ru/svrd/revda,,zxc,,,/ru/svrd/pervik";

            Assert.Equal("/ru", regionTree.addNoteToTree(note1));
            Assert.Equal("", regionTree.addNoteToTree(note2));
            Assert.Equal("", regionTree.addNoteToTree(note3));
            Assert.Equal("", regionTree.addNoteToTree(note4));
            Assert.Equal("", regionTree.addNoteToTree(note5));
            Assert.Equal("", regionTree.addNoteToTree(note6));
        }

        [Fact]
        public void IsTreeCreatedWithWrongNodeName()
        {
            RegionTree regionTree = new RegionTree();

            string note1 = "Ревдинский рабочий/ru/svrd/revda";

            Assert.Equal("", regionTree.addNoteToTree(note1));
            Assert.True(!regionTree.isTreeCreated());
        }
    }
}
