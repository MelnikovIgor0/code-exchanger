# Как это запустить
Установить [докер](https://www.docker.com/products/docker-desktop/) (саму систему и docker desktop) и склонировать репозиторий.

Прописать
```
docker-compose up
```
После этого будет доступно 2 контейнера:
+ База данных database на порте 5433
+ Браузерная pgadmin на порте 5050
+ API на порте 5060

Если возникают проблемы с конейнером бд или нужна пустая бд, то нужно удалить папку postgres_data в code_exchanger_db. Можно добавить любые .sql скрипты для создания и инициализации таблиц в папку init.

## Проблема с API
Запросы на подобие

POST http://194.67.119.99:5060/content/create/mycontent?password=mypassword

GET http://194.67.119.99:5060/content/49b8e45d-6bdd-4288-9b94-dffc6d12b13b

без проблем проходят через Postman, но тот же GET нельзя отправить через браузер, потому что происходит перенаправление на ненастроенный https.

Описание ошибки:

{"EventId":3,"LogLevel":"Warning","Category":"Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionMiddleware","M
essage":"Failed to determine the https port for redirect.","State":{"Message":"Failed to determine the https port for redi
rect.","{OriginalFormat}":"Failed to determine the https port for redirect."}}

Причём проект https://github.com/advasileva/MessageService без проблем развернулся на том же сервере и был доступен сваггер из браузера.

Пока не придумала как решить, если есть идеи - пишите.

Возможная причина - галочка настройек https при создании проекта.

## Подключение базы данных к проекту
Достаточно заменить строку подключения в файле appsettings.json на ```"User ID = postgres; Password = 123456; Host = host.docker.internal; Port = 5433; Database = database"``` (выбран порт 5433, чтобы не было конфликта с уже развёрнутой базой данных на порте 5432). 

Ещё вариант - сразу запустить контейнер на 5432 порте (заменить 15 строку docker-compose.yml на ```- "5432:5432"```).

## Вход в pgadmin
Будет доступна здесь http://localhost:5050
+ Вход
  + Логин: noemail@noemail.com
  + Пароль: 123456
+ Добавление сервера
  + В левой панели: Servers>Register>Server...
  + General: Name - server
  + Connection: Host - pg_db (название контейнера), port - 5432, maintenance database - postgres, username - postgres, password - 123456
  + Save
  
Теперь должна быть доступна наша database c нужными таблицами
