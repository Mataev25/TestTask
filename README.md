# Тестовое задание 

### Папка проекта - TestTask

REST API на ASP.NET Core 10 парсит HTML, ищет email, расшифровывает AES-256-ECB и сохраняет найденные элементы в PostgreSQL

### Запуск
```bash
docker compose up --build
```

**Swagger API** <http://localhost:8090/api/swagger> \
**pgAdmin** <http://localhost:8080> \
**PostgreSQL** - `localhost:5432`, БД `testdb`, пользователь `postgres`, пароль `postgres` (пароль нужен только при прямом подключении, в pgAdmin он подставляется автоматически из `pgadmin/pgpass`) 
