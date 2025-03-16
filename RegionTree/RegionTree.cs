using System;
using System.Globalization;
using System.IO;
using static RegionTreeLib.RegionTree;
using System.Xml.Linq;
using System.Reflection;

namespace RegionTreeLib
{
    public class RegionTree
    {
        public class Node
        {
            private string nodeName;
            private int adIndex;
            private Dictionary<string, Node?> regions = new Dictionary<string, Node?>();

            public string? NodeName
            {
                get => nodeName;
            }

            public int AdIndex
            {
                get => adIndex;
                set
                {
                    if (adIndex == 0)
                        adIndex = value;
                }
            }

            public Node? getChildRegionByName(string nodeName)
            {
                Node? returnNode = null;
                bool isFind = regions.TryGetValue(nodeName, out returnNode);
                return isFind ? returnNode : null;
            }

            public Node(string nodeName)
            {
                this.nodeName = nodeName;
            }

            public bool addRegionToExistNode(string region, Node newRegion)
            {
                return regions.TryAdd(region, newRegion);
            }
        }

        private Node? head;
        private List<string> allAds;

        public bool isTreeCreated()
        {
            return head == null ? false : true;
        }

        /// <summary>
        /// Конструктор создания дерева регионов
        /// </summary>
        /// <param name="pathToFile">Путь до файла, в котором в строчку написаны рекламные площадки и их регион</param>
        public RegionTree(string pathToFile) 
        {
            head = null;
            allAds = new List<string>() { "" };

            if (!File.Exists(pathToFile))
            {
                return;
            }

            string[] lines = File.ReadAllLines(pathToFile);
            foreach (string line in lines)
            {
                if (line != "")
                {
                    try
                    {
                        addNoteToTree(line.Trim());
                    }
                    catch
                    {
                        head = null;
                        allAds = new List<string>() { "" };
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Делает дерево
        /// </summary>
        /// <param name="note">Строчка из файла вида "Ревдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik"</param>
        /// <exception cref="Exception">Если уже было создано дерево с корневым каталогом /ru, а пользователь пытается добавить /en.</exception>
        private void addNoteToTree(string note)
        {
            string adName;

            List<string> regions = parseNote(note, out adName);

            int index = -1;
            if (!allAds.Contains(adName))
            {
                allAds.Add(adName);
                index = allAds.Count - 1;
            }

            foreach (string concreteRegion in regions)
            {
                List<string> regionPathComponents = concreteRegion.Split('/').ToList();
                regionPathComponents.RemoveAll(item => item == "");

                if (head == null)
                {
                    head = new Node(regionPathComponents[0]);
                }
                Node? current = head;

                for(int i = 0; i < regionPathComponents.Count; i++)
                {
                    if (i == regionPathComponents.Count - 1)
                    {
                        // Условие выполнится в том случае, если уже было создано дерево с корневым
                        // каталогом /ru, а пользователь пытается добавить /en.
                        // Ошибка кричисеская - кидаем исключение
                        if (regionPathComponents[i] != current.NodeName)
                        {
                            throw new Exception("Different root directory names");
                        }
                        current.AdIndex = index;
                        break;
                    }

                    Node? nextNode = current.getChildRegionByName(regionPathComponents[i + 1]);
                    if (nextNode == null)
                    {
                        Node? tmp = new Node(regionPathComponents[i + 1]);
                        current.addRegionToExistNode(regionPathComponents[i + 1], tmp);
                        current = tmp;
                    }
                    else
                    {
                        current = nextNode;
                    }
                }
            }
        }

        public static void seeFilesInExetutableDirectory()
        {
            string currentDirectory = Directory.GetCurrentDirectory();

            Console.WriteLine($"Текущая директория: {currentDirectory}");
            Console.WriteLine("----------------------------------------");

            string[] directories = Directory.GetDirectories(currentDirectory);

            Console.WriteLine("Каталоги:");
            foreach (string dir in directories)
            {
                Console.WriteLine(dir);
            }

            string[] files = Directory.GetFiles(currentDirectory);

            Console.WriteLine("\nФайлы:");
            foreach (string file in files)
            {
                Console.WriteLine(file);
            }
        }

        /// <summary>
        /// Выводит рекламные площадки для заданной локации.
        /// </summary>
        /// <param name="pathToNote">Строка с заданной локацией. Пример заданной локации: /ru/svrd/revda</param>
        /// <returns>Список рекламных площадок в этой локации</returns>
        public List<string>? findNote(string? pathToNote)
        {
            if (pathToNote == null || pathToNote == "" || isTreeCreated() == false)
                return null;
            List<string> regions = pathToNote.Split('/').ToList();
            regions.RemoveAll(item => item == "");

            if (head == null || regions.Count == 0 || head.NodeName != regions[0])
                return null;
            Node? current = head;

            List<string> ads = new List<string>();

            for (int i = 0; i < regions.Count; i++)
            {
                if (i == regions.Count - 1)
                {
                    if (allAds[current.AdIndex] != "")
                        ads.Add(allAds[current.AdIndex]);
                    return ads;
                }

                if (allAds[current.AdIndex] != "")
                    ads.Add(allAds[current.AdIndex]);
                Node? nextNode = current.getChildRegionByName(regions[i + 1]);
                if (nextNode == null)
                {
                    return ads;
                }
                else
                {
                    current = nextNode;
                }
            }
            return ads;
        }

        /// <summary>
        /// Парсинг строки из файла. Строка типа: "Ревдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik"
        /// </summary>
        /// <param name="note">Строка вида "Ревдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik"</param>
        /// <param name="adName">Выходной параметр "имя рекламы" вида: "Ревдинский рабочий"</param>
        /// <returns>
        /// Массив записей регионов вида: ["/ru/svrd/revda", "/ru/svrd/pervik"]
        /// </returns>
        private List<string> parseNote(string note, out string adName)
        {
            string[] args = note.Split(':');
            adName = args[0];

            string[] regions = args[1].Split(",");
            return regions.ToList();
        }
    }
}
