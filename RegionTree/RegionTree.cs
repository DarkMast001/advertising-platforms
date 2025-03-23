namespace RegionTreeLib
{
    public class RegionTree
    {
        Dictionary<string, List<string>> adsPaths;

        /// <summary>
        /// Инициализация дерева регионов
        /// </summary>
        public RegionTree()
        {
            adsPaths = new Dictionary<string, List<string>>();
        }

        public bool isTreeCreated()
        {
            return adsPaths.Count > 0;
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
                adsPaths = new Dictionary<string, List<string>>();
                throw;
            }
        }

        /// <summary>
        /// Добавляет узел в дерево
        /// </summary>
        /// <param name="note">Строчка вида "Ревдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik"</param>
        /// <returns>Полный путь региона, который был добавлен последним. 
        /// При успешной попытке добавить "/ru/svrd/revda,/ru/svrd/pervik" вернётся "/ru/svrd/pervik"</returns>
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
        /// Выводит рекламные площадки для заданной локации.
        /// </summary>
        /// <param name="pathToNote">Строка с заданной локацией. Пример заданной локации: /ru/svrd/revda</param>
        /// <returns>Список рекламных площадок в этой локации</returns>
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
