--------------------------------------------------------ОПИСАНИЕ ПРОЕКТА--------------------------------------------------------------
Это приложение, написанное на WPF C#, предназначено для управления товарами на складе с раздлением доступа по ролям. 
Приложение предоставляет следующие возможности для трех типов пользователей:
  1) Консультанты: Могут просматривать товары, распределенные по категориям.
  2) Менеджеры: Могут редактировать количество товаров на складе.
  3) Администраторы: Имеют полный доступ, включая:
    1. Добавление и удаление категорий товаров.
    2. Добавление и удаление товаров в каждой категории.
    3. Управление пользователями (добавление, обновление данных и удаление из базы данных).
     
Приложение также реализует капчу, которая активируется после 5 неверных попыток входа в систему, обеспечивая доролнительную безопасность.
Приложение подключается к базе данных, развернутой в Sql Server Managment Studio. Данные из таблицы выводятся с помощью ORM Entity Framework Core. Подключение к базе данных выведено в конфигурационный файл, что обеспечивает удобство настройки. 

-----------------------------------------------------КАК ЗАПУСТИТЬ ПРИЛОЖЕНИЕ-----------------------------------------------------
Чтобы запустить приложение необходимо:
1) Открыть папку с названием репозитория (FurnitureOfficeRepos)
2) В этой папке перейти в папку с названием проекта (FurnitureProject)
3) Перейти в папку bin
4) Перейти в папку Debug
5) Найти файл FurnitureProject.exe
6) Запустить файл.

------------------------------------------------------ТЕХНОЛОГИИ В ПРИЛОЖЕНИИ----------------------------------------------------
1) Интерфейс приложения разработан на WPF(XAML)
2) Бэкэнд приложения написан на языке программирования C# и ORM Entity Framework Core для работы с базой данных.
3) База данных развернута в Sql Server Managment Studio
4) Подключение к БД выведено в JSON-файл
5) Безопасность входа реализована с помощью капчи

----------------------------------------------КАК ЗАПУСТИТЬ ПРИЛОЖЕНИЕ РАЗРАБОТЧИКУ-----------------------------------------
1) Открыть консоль (CMD, Git Bash, Power Shell)
2) Написать команду "git clone 'ссылка на репозиторий'" и нажать Enter
3) После загрузки репозитория, заходим в папку.
4) Найти файл FurnitureProject.sln и запустить.

----------------------------------------------------СКРИНШОТЫ РАБОТЫ ПРИЛОЖЕНИЯ-----------------------------------------------
1) Окно авторизации.
![image](https://github.com/user-attachments/assets/f6adfc36-9443-48f4-9799-ebde03e6c145)
2) Загрузка БД при переходе в другие окна
![image](https://github.com/user-attachments/assets/03323e6e-8d40-45f6-9b7f-01890f3f0d8c)
3) Возможность просмотра ассортимента товаров на складе в окне консультанта
![image](https://github.com/user-attachments/assets/b73ffa7b-b6f6-4a47-a048-bf382380068f) 
4) Выход из аккаунта
![image](https://github.com/user-attachments/assets/0a4635b6-090e-40d4-bbe3-9a84f865af13)
5) Возможность изменения количества товара в окне менеджера
![image](https://github.com/user-attachments/assets/7117ae98-f2c3-4b8c-bd56-a68fbc0c1410)
![image](https://github.com/user-attachments/assets/35b64dff-d31b-4416-a3eb-b0982159c8fd)
![image](https://github.com/user-attachments/assets/19c3551b-0653-47b1-8129-ddaa513b3ec5)
![image](https://github.com/user-attachments/assets/59138990-ebb3-4492-a8e6-c19914a2c7d6)
6) Окно администратора
![image](https://github.com/user-attachments/assets/2bf14ca4-81f8-47a2-a65b-2cfaefe7beb5)
7) Управление пользователями в окне администратора
![image](https://github.com/user-attachments/assets/b6f09f2d-bd59-4bcb-b325-06a14c104482)
8) Добавление нового пользователя
![image](https://github.com/user-attachments/assets/48a9996c-b4de-483e-a0d8-f9ddf90f834a)
9) Обновление данных о пользователе.
![image](https://github.com/user-attachments/assets/5d5d4958-22d2-4ae3-ae69-a424657f827d)
10) Удаление пользователя.
![image](https://github.com/user-attachments/assets/75612bbe-8223-4888-b400-acaee6af57f7)
11) Добавление новой категории товаров в коне админа
![image](https://github.com/user-attachments/assets/55aa7c57-a91e-4675-bc25-2ac9c6bce4fb)
![image](https://github.com/user-attachments/assets/11715ac0-e151-49a9-aa1c-6f4e311e0db4)
![image](https://github.com/user-attachments/assets/2af3e2e9-451d-4cb2-9e02-26b62ef22c03)
12) Добавление товаров в категорию.
![image](https://github.com/user-attachments/assets/4117e665-2edc-47f4-9b20-f4ef7bf47ec9)
![image](https://github.com/user-attachments/assets/e7f8a822-508e-4475-a5bf-74c20e42cd04)
![image](https://github.com/user-attachments/assets/be8d6b28-4c38-4da2-9ab0-0691150b7189)
13) Удаление товаров из категории.
![image](https://github.com/user-attachments/assets/30c27532-17c6-4728-a4c2-9fa424637639)
14) Удаление категории.
![image](https://github.com/user-attachments/assets/3ce82199-fc80-4a50-a4c2-953555014313)
![image](https://github.com/user-attachments/assets/134d12a6-b3e0-44f7-8ad3-eadeae7e949a)
15) Капча при 5 неправльных попытках входа в аккаунт.
![image](https://github.com/user-attachments/assets/0efeea5c-07b3-4c30-beee-e65f5ceef21f)
![image](https://github.com/user-attachments/assets/0a874deb-5f81-44cf-9b0e-adcbdbb17727)
![image](https://github.com/user-attachments/assets/19fb6379-f662-4f17-87c8-966be98a1f17)
16) Оповещение на окне авторизации с помощью картинки, о том что подключение с БД не установлено.
![image](https://github.com/user-attachments/assets/fb388bc0-1d50-4c47-9325-c5173771394a)





























