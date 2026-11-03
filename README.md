# dnd_api

Бэкенд для приложения-помощника по D&D: регистрация и вход, справочники рас и классов, персонажи пользователя.
Клиент — Flutter-приложение [hiluk/dnd_app](https://github.com/hiluk/dnd_app).

Стек: ASP.NET Core 8, ASP.NET Core Identity (bearer-токены), EF Core + PostgreSQL.

## Запуск

### Всё в Docker

```bash
docker compose up --build
```

API будет доступен на `http://localhost:5000`, Swagger — на `http://localhost:5000/swagger`.

### Локально

Нужны .NET 8 SDK и PostgreSQL. Строка подключения для разработки лежит в
`DndSolution.Core.Api/appsettings.Development.json` (`Host=localhost;Database=dnd_db;Username=dnd;Password=dnd`).
Можно поднять только базу из compose:

```bash
docker compose up -d postgres
```

```bash
dotnet run --project DndSolution.Core.Api --launch-profile http
```

При старте приложение само применяет миграции и заполняет справочники рас и классов.

### Тесты

Интеграционные тесты поднимают API в памяти поверх отдельной базы `dnd_test`
(пересоздаётся при каждом прогоне). Строку подключения можно переопределить переменной `DND_TEST_POSTGRES`.

```bash
dotnet test
```

## API

| Метод | Путь | Описание |
|---|---|---|
| POST | `/register` | Регистрация `{ email, login, password }` |
| POST | `/login` | Вход по логину или эмейлу `{ login, password }` → `{ accessToken, refreshToken, expiresIn }` |
| POST | `/refresh` | Новая пара токенов `{ refreshToken }` |
| GET | `/races`, `/classes` | Справочники (без авторизации) |
| POST | `/races/save-new`, `/classes/save-new` | Добавить в справочник (роль `Admin`) |
| POST | `/abilities/roll` | Бросить 4к6 для шести характеристик → `{ values, token }` |
| POST | `/characters/preview` | Проверить черновик по правилам и посчитать лист, не сохраняя |
| POST | `/characters/create` | Создать персонажа 1 уровня из черновика |
| GET | `/characters` | Персонажи текущего пользователя (с вычисленным листом `sheet`) |
| GET | `/characters/{id}` | Персонаж по Id |
| DELETE | `/characters/{id}` | Удалить персонажа |

Ошибки возвращаются в формате `ProblemDetails` / `ValidationProblemDetails`.

Персонаж создаётся по правилам «Книги игрока»: стандартный набор, покупка очков или броски,
бонусы расы и производные значения считает сервер (`CharacterRules`). Подробности — в
[спецификации](docs/specs/2026-10-31-ability-generation-design.md).

Выдать пользователю роль администратора:

```sql
INSERT INTO asp_net_roles (id, name, normalized_name) VALUES (gen_random_uuid(), 'Admin', 'ADMIN');
INSERT INTO asp_net_user_roles (user_id, role_id)
SELECT u.id, r.id FROM asp_net_users u, asp_net_roles r WHERE u.user_name = 'login' AND r.name = 'Admin';
```

## Структура

| Проект | Назначение |
|---|---|
| `Core.Api` | Контроллеры, маппинг DTO ↔ модели, настройка приложения |
| `Core.Sdk` | DTO и enum'ы контракта API |
| `Application.*` | Доменные модели, сервисы и их интерфейсы |
| `Data.Abstractions` | Интерфейсы репозиториев (работают с доменными моделями) |
| `Data.Entities` | Сущности хранения (таблицы БД), о них знают только репозитории |
| `Data.Repositories` | EF Core контекст, репозитории с маппингом сущность ↔ модель, миграции, сидинг |
| `Tests` | Интеграционные тесты API |
