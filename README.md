Название: BloggingPlatformAPI
Уровень: Beginner
Тип: web api
Технологии: C#, ASP.NET Core with Controllers, Minimal Api
Источник задания: [BloggingPlatformAPI](https://roadmap.sh/projects/blogging-platform-api)
Статус: Готово

RESTful API для персональной платформы ведения блогов. API позволяет пользователям выполнять следующие операции:
- Создавать новую запись в блоге
- Обновлять существующую запись в блоге
- Удалить существующую запись в блоге
- Получить одну запись в блоге
- Получить все записи в блоге
- Фильтровать записи в блоге по поисковому запросу

**Внимание!!!**
Отслеживание изменений у файла postgres-password.txt отключено командой
```bash
git update-index --skip-worktree postgres-password.txt
```

Чтобы включить отслеживание используйте команду
```bash
git update-index --no-skip-worktree postgres-password.txt
```

```shell
dotnet ef database update --connection "Host=localhost;Port=5432;Database=<databasename>;Username=postgres;Password=<password>"
```
********************************
Name: BloggingPlatformAPI
Level: Beginner/Intermediate/Advanced
Type: CLI/web api
Technology: C#, ASP.NET Core with Controllers, Minimal Api
Source: [BloggingPlatformAPI](https://roadmap.sh/projects/blogging-platform-api)
Stage: Ready

A RESTful API for a personal blogging platform. The API allow users to perform the following operations:
- Create a new blog post
- Update an existing blog post
- Delete an existing blog post
- Get a single blog post
- Get all blog posts
- Filter blog posts by a search term

**Attention!!!**
Tracking changes in the postgres-password file.txt is disabled by the command
```bash
git update-index --skip-worktree postgres-password.txt
```

To enable tracking, use the command
```bash
git update-index --no-skip-worktree postgres-password.txt
```

```shell
dotnet ef database update --connection "Host=localhost;Port=5432;Database=<databasename>;Username=postgres;Password=<password>"
```