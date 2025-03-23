# Веб сервис по подбору рекламных площадок для заданных локаций

## Содержание

1. [Структура данных для хранения локаций и рекламных площадок в оперативной памяти.](#структура-данных-для-хранения-локаций-и-рекламных-площадок-в-оперативной-памяти)
2. [Почему всё реализованно именно так?](#почему-всё-реализованно-именно-так)
3. [ASP\.NET Core приложение с Rest API](#aspnet-core-приложение-c-rest-api)
   - [Использование API](#использование-api)
   - [Установка и запуск](#установка-и-запуск)

## Структура данных для хранения локаций и рекламных площадок в оперативной памяти

Все данные хранятся в словаре, где ключом является путь, а значением - название рекламной площадки. Структура данных на выходе представляет из себя динамическую библиотеку классов (.dll файл), который подключаетя в проект с веб-сервисом. Класс `RegionTree` хранит в себе:

1. Словарь `adsPaths`, который хранит в себе все пути и названия рекламных площадок

```csharp
public class RegionTree {
    Dictionary<string, List<string>> adsPaths;

	public RegionTree() {
		// код
	}

	// Функции для работы с деревом //

    public bool isTreeCreated() {
		// код
	}

	public async Task createTree(string? pathToFile) {
		// код
	}

	public string addNoteToTree(string? note) {
		// код
	}

	public List<string> findNote(string? pathToNote) {
		// код
	}
}
```

Разберём на конкретном примере то, как будет выглядеть словарь.
Допустим есть файл, в котором находятся следующие строки:

> Яндекс.Директ:/ru<br>
> Ревдинский рабочий:/ru/svrd/revda,/ru/svrd/pervik<br>
> Газета уральских москвичей:/ru/msk,/ru/permobl,/ru/chelobl<br>
> Крутая реклама:/ru/svrd<br>

Словарь будет выглядеть следующим образом:

> /ru : "Яндекс.Директ" <br>
> /ru/svrd/revda : "Яндекс.Директ", "Ревдинский рабочий", "Крутая реклама" <br>
> /ru/svrd/pervik : "Яндекс.Директ", "Ревдинский рабочий", "Крутая реклама" <br>
> /ru/msk : "Яндекс.Директ", "Газета уральских москвичей" <br>
> /ru/permobl : "Яндекс.Директ", "Газета уральских москвичей" <br>
> /ru/chelobl : "Яндекс.Директ", "Газета уральских москвичей" <br>
> /ru/svrd : "Яндекс.Директ", "Крутая реклама"

### Почему всё реализованно именно так?

Такая реализация позволяет достичь сложности алгоритма поиска в O(1), что нам очень важно, потому что к словарю будут обращаться очень часто.

## ASP\.NET Core приложение c Rest API

### Использование API

1.  Загрузка данных из файла
    **Метод:** `POST /api/Ads/upload`
    **Тело запроса:**

    ```
    "string"
    ```

    **Ответ:**

    - `200: OK`:Данные успешно загружены
    - `404 Not Found`: Файл не найден.
    - `400 Bad Request`: Произошла ошибка при загрузке.

    Пример запроса в Postman:

    - URL: `http://localhost:5288/api/ads/upload`
    - Method: `POST`
    - Body (raw, JSON): `"C://path//to//your//file.txt"`

2.  Поиск рекламных площадок по региону
    **Метод:** `GET /api/ads/search`
    **Параметры запроса:**

    `region` (строка): Название региона.

    **Ответ:**

    - `200 OK`: Список рекламных площадок.
    - Пример ответа:

    ```
    [
        "Ad Platform 1",
        "Ad Platform 2"
    ]
    ```

    Пример запроса в Postman:

    - URL для `http`: `http://localhost:5104/api/ads/search?region=/ru/chelobl`
    - Или для `http`: `http://localhost:5000/api/ads/search?region=/ru/chelobl`
    - URL для `https`: `https://localhost:7014/api/ads/search?region=/ru/chelobl`
    - Method: `GET`

### Установка и запуск

1. `git clone` в удобную вам папку
2. Переходите в `.../WebServiceForAdvertisingPlatforms/bin/Release/net8.0/publish`
3. Запускаете `WebServiceForAdvertisingPlatforms.exe`.
4. Если всё успешно, то должна открыться консоль, содержание которой будет примерно следующее:
   > info: Microsoft.Hosting.Lifetime[14]<br>
   > Now listening on: http://localhost:5000<br>
   > info: Microsoft.Hosting.Lifetime[0]<br>
   > Application started. Press Ctrl+C to shut down.<br>
   > info: Microsoft.Hosting.Lifetime[0]<br>
   > Hosting environment: Production<br>
   > info: Microsoft.Hosting.Lifetime[0]<br>
   > Content root path: D:\Институт\TEST\advertising-platforms\WebServiceForAdvertisingPlatforms\bin\Release\net8.0\publish
5. Открываете Postman

   _5.1. Загрузка файла_
   `POST` запрос, вводите следующее: `http://localhost:5000/api/Ads/upload`
   В теле запроса выбираете `Body`, затем `raw` и пишете путь к файлу. Пример: `"D://regions.txt"`
   Если всё успешно, то код будет `200 OK` и будет надпись `Данные успешно загружены.`

   _5.2. Поиск рекламной площадки_
   `GET` запрос, вводите следующее: `http://localhost:5000/api/Ads/search?region=/ru`
   Если всё успешно, то будет код `200 OK`, и если вы туда загружали txt файл, который был приведён в качестве примера выше, то вам будет выведен массив: `["Яндекс.Директ"]`
