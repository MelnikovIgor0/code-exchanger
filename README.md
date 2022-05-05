# Как это запустить
Установить [докер](https://www.docker.com/products/docker-desktop/) (саму систему и docker desktop)

Перейти в директорию с docker-compose.yaml и прописать
```
docker-compose up
```
После этого будет доступно 2 контейнера:
+ База данных database на порте 5433
+ Браузерная pgadmin на порте 5050

Если возникают проблемы с конейнером бд или нужна пустая бд, то нужно удалить папку postgres_data

## Подключение базы данных к проекту
Достаточно заменить в файле appsettings.json порт 5432 на 5433 (чтобы не было конфликта с уже развёрнутой базой данных на порте 5432). 

Ещё вариант - сразу запустить контейнер на 5432 порте (заменить 15 строку docker-compose.yml на ```      - "5432:5432"```)

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
