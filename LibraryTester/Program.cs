using RegionTreeLib;

namespace LibraryTester
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите имя файла: ");
            string pathToFile = "./regions.txt";
            //string? pathToFile = Console.ReadLine();
            RegionTree rt = new RegionTree(pathToFile);

            while (!rt.isTreeCreated())
            {
                Console.WriteLine("Такого файла нет. Попробуйте ещё раз.");
                Console.WriteLine("Введите имя файла: ");
                pathToFile = Console.ReadLine();
                try
                {
                    rt = new RegionTree(pathToFile);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            
            Console.WriteLine("Успех! Дерево построено!");

            Console.WriteLine("Введите путь до региона: ");
            string? regionPath = Console.ReadLine();

            while (regionPath != "exit")
            {
                List<string>? ads = rt.findNote(regionPath);
                if (ads != null)
                {
                    foreach (string ad in ads)
                    {
                        Console.WriteLine(ad);
                    }
                }
                else
                {
                    Console.WriteLine("Неверный адрес!");
                }

                Console.WriteLine("Введите путь до региона: ");
                regionPath = Console.ReadLine();
            }
        }
    }
}
