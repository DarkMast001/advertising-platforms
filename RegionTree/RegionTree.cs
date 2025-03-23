namespace RegionTreeLib
{
    public class RegionTree
    {
        private class Node
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

        Dictionary<string, List<string>> adsPaths;

        public bool isTreeCreatedOld()
        {
            return head == null ? false : true;
        }

        public bool isTreeCreated()
        {
            return adsPaths.Count > 0;
        }

        /// <summary>
        /// Инициализация дерева регионов
        /// </summary>
        public RegionTree()
        {
            head = null;
            allAds = new List<string>() { "" };

            adsPaths = new Dictionary<string, List<string>>();
        }

        /// <summary>
        /// Построение дерева
        /// </summary>
        /// <param name="pathToFile">Путь до файла</param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException">В случае если такого файла не существует</exception>
        /// <exception cref="ArgumentException">В случае попытки открыть файл формата отличного от .txt</exception>
        public async Task createTree(string? pathToFile)
        {
            if (!File.Exists(pathToFile))
            {
                throw new FileNotFoundException($"File is not exist.");
            }

            head = null;
            allAds = new List<string>() { "" };

            string fileExtension = Path.GetExtension(pathToFile);
            if (fileExtension.ToLower() != ".txt")
            {
                throw new ArgumentException("File must have a .txt extension.");
            }

            try
            {
                using (var stream = new StreamReader(pathToFile))
                {
                    string? line;
                    while ((line = await stream.ReadLineAsync()) != null) 
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            addNoteToTree(line);
                        }
                    }
                }
            }
            catch
            {
                head = null;
                allAds = new List<string>() { "" };
                adsPaths = new Dictionary<string, List<string>>();
                throw;
            }

            /*string[] lines = File.ReadAllLines(pathToFile);
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
            }*/
        }

        public string addNoteToTree(string? note)
        {
            if (string.IsNullOrWhiteSpace(note))
            {
                return "";
            }

            string adName;
            List<string>? regions = parseNote(note, out adName);
            if (regions is null || regions.Count == 0)
            {
                return "";
            }

            string lastAddedPath = "";

            foreach (string region in regions)
            {
                if (!adsPaths.ContainsKey(region))
                {
                    adsPaths[region] = new List<string>();
                }
                if (!adsPaths[region].Contains(adName))
                {
                    adsPaths[region].Add(adName);
                }

                updateChildPath(region, adName);

                lastAddedPath = region;

                var pathComponents = region.Split('/').ToList();
                pathComponents.RemoveAt(0);
                if (pathComponents.Count < 1 || pathComponents.Contains(""))
                {
                    return "";
                }
                for (int i = 0; i < pathComponents.Count; i++)
                {
                    string intermediatePath = "/" + string.Join("/", pathComponents.Take(i));
                    if (adsPaths.ContainsKey(intermediatePath))
                    {
                        List<string>? adInRegion = new List<string>();
                        if (adsPaths.TryGetValue(intermediatePath, out adInRegion))
                        {
                            if (!adInRegion.Contains(""))
                            {
                                adsPaths[region].InsertRange(adsPaths[region].Count - 1, adInRegion);
                            }
                        }
                    }
                }
            }

            return lastAddedPath;
        }

        /// <summary>
        /// Проблема заключается в том, что алгоритм нормально учитывает родительские пути, но только в том случае, 
        /// если родительские пути были в файле до текущей строчки, а если они находятся после текущей строчки, 
        /// то они просто не добавляются в уже существующий список реклам.
        /// То есть у нас сначала сделается строчка в словаре
        /// /ru : Яндекс.Директ
        /// Потом сделается строчка в словаре:
        /// /ru/svrd/revda : Ревдинский рабочий, Яндекс.Директ
        /// И потом сделается строчка
        /// /ru/svrd : Крутая реклама, Яндекс.Директ
        /// Но при этом список реклам для второй строчки не обновится.
        /// То есть при добавлении /ru/svrd надо найти все ключи, которые содержат в себе эту подстроку
        /// и добавить к их списку реклам новую.
        /// </summary>
        /// <param name="parentPath">Путь только что доьавленной рекламы</param>
        /// <param name="adName">Название рекламы</param>
        private void updateChildPath(string parentPath, string adName)
        {
            foreach (var key in adsPaths.Keys.ToList())
            {
                if (key.StartsWith(parentPath) && key != parentPath)
                {
                    if (!adsPaths[key].Contains(adName))
                    {
                        adsPaths[key].Add(adName);
                    }
                }
            }
        }

        /// <summary>
        /// Добавляет узел в дерево
        /// </summary>
        /// <param name="note">Строчка вида "Ревдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik"</param>
        /// <returns>Полный путь региона, который был добавлен последним. 
        /// При успешной попытке добавить "/ru/svrd/revda,/ru/svrd/pervik" вернётся "/ru/svrd/pervik"</returns>
        /// <exception cref="Exception">В случае попытки изменить уже созданный корневой элемент дерева.
        /// Например, если уже было создано дерево с корневым элементом /ru, а пользователь пытается добавить /en.</exception>
        public string addNoteToTreeOld(string? note)
        {
            if (note == "" || note is null)
            {
                return "";
            }
            string adName;

            List<string>? regions = parseNote(note, out adName);
            if (regions is null)
            {
                return "";
            }

            int index = -1;
            if (!allAds.Contains(adName))
            {
                allAds.Add(adName);
                index = allAds.Count - 1;
            }

            foreach (string concreteRegion in regions)
            {
                List<string> regionPathComponents = concreteRegion.Split('/').ToList();
                regionPathComponents.RemoveAt(0);
                if (regionPathComponents.Count < 1 || regionPathComponents.Contains(""))
                {
                    allAds.RemoveAt(allAds.Count - 1);
                    return "";
                }

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
                        // Ошибка критическая - кидаем исключение
                        if (regionPathComponents[i] != current.NodeName)
                        {
                            head = null;
                            allAds = new List<string>() { "" };
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
            return string.Join("/", regions[regions.Count - 1]);
        }

        public List<string> findNote(string? pathToNote)
        {
            if (string.IsNullOrWhiteSpace(pathToNote) || !adsPaths.ContainsKey(pathToNote))
            {
                return new List<string>();
            }

            List<string>? ads = new List<string>();
            if (adsPaths.TryGetValue(pathToNote, out ads))
            {
                return ads;
            }
            else
            {
                return new List<string>();
            }
        }

        /// <summary>
        /// Выводит рекламные площадки для заданной локации.
        /// </summary>
        /// <param name="pathToNote">Строка с заданной локацией. Пример заданной локации: /ru/svrd/revda</param>
        /// <returns>Список рекламных площадок в этой локации</returns>
        public List<string> findNoteOld(string? pathToNote)
        {
            if (pathToNote == null || pathToNote == "" || isTreeCreated() == false)
                return new List<string>();
            List<string> regions = pathToNote.Split('/').ToList();

            regions.RemoveAt(0);
            if (regions.Count < 1 || regions.Contains(""))
            {
                return new List<string>();
            }

            if (head == null || regions.Count == 0 || head.NodeName != regions[0])
                return new List<string>();
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
        private List<string>? parseNote(string note, out string adName)
        {
            string[] args = note.Split(':');
            if (args.Length < 2)
            {
                adName = "";
                return null;
            }
            adName = args[0];

            string[] regions = args[1].Split(",");
            return regions.ToList();
        }
    }
}
