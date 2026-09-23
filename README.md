# IsLabApp — ASP.NET Core Web API (лабораторная работа)

Проект уже содержит решения для заданий 2–5. Ниже — как развернуть его
у себя на рабочей станции.

## Задание 2. Создание и запуск проекта

Вариант А — использовать уже готовый код из этого архива:

1. Распакуйте архив, например, в `C:\is-labs\lab4\IsLabApp`.
2. Откройте терминал в папке проекта:
   ```
   cd C:\is-labs\lab4\IsLabApp
   dotnet restore
   dotnet run
   ```
3. Консоль покажет адрес, на котором слушает приложение, например:
   `Now listening on: http://localhost:5080`

Вариант Б — создать проект с нуля самостоятельно (как в задании) и затем
заменить `Program.cs` и `appsettings.json` файлами из этого архива:
```
mkdir C:\is-labs\lab4
cd C:\is-labs\lab4
dotnet new webapi -n IsLabApp
cd IsLabApp
dotnet run
```

Проверка:
- Swagger: `http://localhost:<port>/swagger`
- curl:
  ```
  curl http://localhost:<port>/weatherforecast
  ```

## Задание 3. /health и /version

Эндпоинты уже реализованы в `Program.cs`, значения берутся из
`appsettings.json` (`App:Name`, `App:Version`).

Проверка:
```
curl http://localhost:<port>/health
curl http://localhost:<port>/version
```

Пример ответа `/health`:
```json
{ "status": "ok", "timestamp": "2026-09-22T10:15:00Z" }
```

Пример ответа `/version`:
```json
{ "name": "IsLabApp", "version": "1.0.0" }
```

## Задание 4. CRUD "Заметки" (in-memory)

Реализовано в `Program.cs` + модель `Models/Note.cs`.
Хранилище — `ConcurrentDictionary<int, Note>` в памяти процесса (данные
пропадут при перезапуске приложения — это ожидаемо для лабораторной).

Маршруты:
- `POST /api/notes` — создать. Тело запроса:
  ```json
  { "title": "Моя первая заметка", "text": "Текст заметки" }
  ```
- `GET /api/notes` — список всех заметок
- `GET /api/notes/{id}` — получить одну заметку
- `DELETE /api/notes/{id}` — удалить заметку

Валидация: `title` обязателен, не пустой, до 200 символов; `text` обязателен
(может быть пустой строкой, но не `null`). При ошибке — `400 Bad Request`
с телом `{ "error": "..." }`.

Проверка через curl:
```
curl -X POST http://localhost:<port>/api/notes ^
  -H "Content-Type: application/json" ^
  -d "{\"title\":\"Тест\",\"text\":\"Первая заметка\"}"

curl http://localhost:<port>/api/notes
curl http://localhost:<port>/api/notes/1
curl -X DELETE http://localhost:<port>/api/notes/1
```
(на Linux/macOS используйте одинарные кавычки вместо `^` и экранирования)

Либо всё то же самое — через Swagger UI (`/swagger`).

## Задание 5. Заготовка под MS SQL Server и /db/ping

В `appsettings.json` добавлена секция:
```json
"ConnectionStrings": {
  "Mssql": "Server=localhost,1433;Database=IsLabDb;User Id=sa;Password=Your_password123;TrustServerCertificate=True;"
}
```
Это заглушка — замените значения на реальные, когда разверните SQL Server.

Эндпоинт `GET /db/ping`:
- читает строку подключения из конфигурации,
- пытается открыть подключение (таймаут 5 секунд),
- возвращает `{ "status": "ok", ... }` при успехе или
  `{ "status": "error", "message": "..." }` при ошибке — это нормально,
  пока SQL Server не развёрнут.

Проверка:
```
curl http://localhost:<port>/db/ping
```

## Docker

Проект содержит multi-stage `Dockerfile`:
- этап сборки — официальный образ `mcr.microsoft.com/dotnet/sdk:8.0`;
- этап запуска — официальный образ `mcr.microsoft.com/dotnet/aspnet:8.0`
  (только рантайм, без SDK — образ меньше);
- в финальный образ копируются только опубликованные артефакты
  (`dotnet publish`), а не весь исходный код;
- приложение внутри контейнера слушает порт `8080`
  (`ASPNETCORE_URLS=http://+:8080`, `EXPOSE 8080`);
- запуск — `dotnet IsLabApp.dll`.

Сборка образа:
```
cd C:\is-labs\lab4\IsLabApp
docker build -t islabapp:1.0 .
```

Запуск контейнера (пробрасываем порт 8080 контейнера на 8080 хоста):
```
docker run --rm -p 8080:8080 islabapp:1.0
```

Проверка:
```
curl http://localhost:8080/health
curl http://localhost:8080/version
curl http://localhost:8080/weatherforecast
```
Swagger в контейнере по умолчанию выключен (доступен только в Development,
а в контейнере `ASPNETCORE_ENVIRONMENT` не задан — используется Production).
Если нужен Swagger в контейнере, добавьте `-e ASPNETCORE_ENVIRONMENT=Development`
к команде `docker run`.

## Структура проекта

```
IsLabApp/
├── Dockerfile
├── .dockerignore
├── IsLabApp.csproj
├── Program.cs
├── appsettings.json
├── appsettings.Development.json
├── Models/
│   └── Note.cs
└── Properties/
    └── launchSettings.json
```

## Примечания

- Требуется установленный .NET 8 SDK (`dotnet --version` должен показать 8.x).
- Пакет `Microsoft.Data.SqlClient` подтянется автоматически при `dotnet restore`
  / `dotnet run` (нужен доступ в интернет для NuGet при первом запуске).
- Пакет `Swashbuckle.AspNetCore` обеспечивает Swagger UI.
