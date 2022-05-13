# Развёрнутые модули
+ База данных database http://194.67.119.99:5054
+ Браузерная pgadmin http://194.67.119.99:5050
+ API http://194.67.119.99:5062 (swagger: http://194.67.119.99:5062/swagger/index.html)

## Как запустить на своей машине
Установить [docker](https://www.docker.com/products/docker-desktop/) (саму систему и docker desktop)

Склонировать репозиторий
```
git clone https://github.com/MelnikovIgor0/code-exchanger.git
```
Перейти в нужную директорию
```
cd code-exchanger
```
Прописать
```
docker-compose up
```
Если api станет плохо, то повторить (идёт запрос на подключение к бд, а она ещё не встала, поэтому возникает ошибка)

После этого будет доступно 2 контейнера:
+ База данных database на порте 5434
+ Браузерная pgadmin на порте 5050
+ API на порте 5062

Если возникают проблемы с контейнером бд или нужна пустая бд, то нужно удалить папку postgres_data в code_exchanger_db. Также можно добавить любые .sql скрипты для создания и заполнения таблиц в папку init.

## Подключение базы данных

Строка подключения базы данных:
```
User ID = postgres; Password = 123456; Host = host.docker.internal; Port = 5434; Database = database
``` 
Выбран порт 5434, чтобы не было конфликта с уже развёрнутой базой данных на порте 5432. 

Как вариант, можно сразу запустить контейнер на 5432 порте (заменить 15 строку docker-compose.yml на ```- "5432:5432"```).

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
  
Теперь должна быть доступна наша database c нужными таблицами.
