using System;
using System.Diagnostics;
using Npgsql;

class Program
{
    public static string databaseName = "SlanDB3";
    static void Main(string[] args)
    {
        
        string connectionString = $"Host=localhost;port=5432;Username=postgres;Password=admin;Database={databaseName}";
        CreateDB();
        Query(connectionString);

    }

    static void Query(string connectionString)
    {
        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();
            using (var command = new NpgsqlCommand())
            {
                command.Connection = connection;
                string sqlScript = $"Select * from words WHERE name = 'run'";

                command.CommandText = sqlScript;
                var sw = new Stopwatch();
                sw.Start();
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        Console.WriteLine("Результаты запроса:");

                        while (reader.Read())
                        {
                            Guid id = reader.GetGuid(0); 
                            string name = reader.GetString(1); 
                            string example = reader.GetString(2);

                            Console.WriteLine($"ID: {id}, Name: {name}, Example: {example}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Данные не найдены.");
                    }
                }
                sw.Stop();
                Console.WriteLine(sw.Elapsed.ToString());
            }
        }
    }

    static void CreateDB()
    {
        string connectionString = "Host=localhost;port=5432;Username=postgres;Password=admin;Database=postgres";
        string sqlScript = @"
CREATE EXTENSION IF NOT EXISTS ""uuid-ossp"";
CREATE TABLE IF NOT EXISTS Dictionary (
  ID        uuid DEFAULT uuid_generate_v4() NOT NULL, 
  Name      varchar(60) NOT NULL, 
  create_at timestamp, 
  user_id   uuid NOT NULL, 
  PRIMARY KEY (ID));
CREATE TABLE IF NOT EXISTS Tag_Value (
  ID    uuid DEFAULT uuid_generate_v4() NOT NULL, 
  ""Tag"" varchar(30) NOT NULL, 
  Value varchar(30) NOT NULL, 
  PRIMARY KEY (ID));
CREATE TABLE IF NOT EXISTS ""User"" (
  ID       uuid NOT NULL, 
  UserName varchar(30) NOT NULL UNIQUE, 
  Email    varchar(30) UNIQUE, 
  PRIMARY KEY (ID));
CREATE TABLE IF NOT EXISTS Vocabulary (
  ID          uuid DEFAULT uuid_generate_v4() NOT NULL, 
  Tag_ValueID uuid NOT NULL, 
  word_id     uuid NOT NULL,
  PRIMARY KEY (ID));
CREATE TABLE IF NOT EXISTS word_assighnment (
  ID            uuid NOT NULL, 
  user_id       uuid NOT NULL, 
  dictionary_id uuid NOT NULL, 
  word_id1      uuid NOT NULL,
  word_id2      uuid NOT NULL,
  PRIMARY KEY (ID));
CREATE TABLE IF NOT EXISTS words (
  ID      uuid NOT NULL, 
  name    varchar(30), 
  example varchar(100),  
  language varchar(30), 
  PRIMARY KEY (ID));
ALTER TABLE Vocabulary ADD CONSTRAINT FKVocabulary440486 FOREIGN KEY (Tag_ValueID) REFERENCES Tag_Value (ID);
ALTER TABLE Dictionary ADD CONSTRAINT FKDictionary939139 FOREIGN KEY (user_id) REFERENCES ""User"" (ID);
ALTER TABLE word_assighnment ADD CONSTRAINT FKword_assig914040 FOREIGN KEY (user_id) REFERENCES ""User"" (ID);
ALTER TABLE word_assighnment ADD CONSTRAINT FKword_assig514825 FOREIGN KEY (dictionary_id) REFERENCES Dictionary (ID);
ALTER TABLE word_assighnment ADD CONSTRAINT FKword_assig405610 FOREIGN KEY (word_id1) REFERENCES words (ID);
ALTER TABLE word_assighnment ADD CONSTRAINT FKword_assig405611 FOREIGN KEY (word_id2) REFERENCES words (ID);
ALTER TABLE Vocabulary ADD CONSTRAINT FKVocabulary877948 FOREIGN KEY (word_id) REFERENCES words (ID);

-- Добавление индексов
CREATE INDEX idx_words_name ON words (name);

-- Создание пользователей
INSERT INTO ""User"" (ID, UserName, Email) VALUES
(uuid_generate_v4(), 'Alice', 'alice@example.com'),
(uuid_generate_v4(), 'Bob', 'bob@example.com');

-- Создание словаря
INSERT INTO Dictionary (ID, Name, create_at, user_id) VALUES
(uuid_generate_v4(), 'English Vocabulary', NOW(), (SELECT ID FROM ""User"" WHERE UserName = 'Alice'));

-- Добавление слов
INSERT INTO words (ID, name, example, language) VALUES
(uuid_generate_v4(), 'run', 'I run every morning.', 'English'),
(uuid_generate_v4(), 'jump', 'He can jump very high.', 'English'),
(uuid_generate_v4(), 'read', 'She likes to read books.', 'English');
INSERT INTO words (ID, name, example, language) VALUES
(uuid_generate_v4(), 'бежать', 'Я бегу каждое утро.', 'Russian'),
(uuid_generate_v4(), 'прыгать', 'Он может высоко прыгать.', 'Russian'),
(uuid_generate_v4(), 'читать', 'Она любит читать книги.', 'Russian'),
(uuid_generate_v4(), 'писать', 'Он пишет письмо другу.', 'Russian'),
(uuid_generate_v4(), 'говорить', 'Они говорят по-русски.', 'Russian'),
(uuid_generate_v4(), 'слушать', 'Я слушаю музыку.', 'Russian'),
(uuid_generate_v4(), 'смотреть', 'Мы смотрим фильм.', 'Russian'),
(uuid_generate_v4(), 'думать', 'Он думает о будущем.', 'Russian'),
(uuid_generate_v4(), 'знать', 'Я знаю ответ.', 'Russian'),
(uuid_generate_v4(), 'видеть', 'Она видит красивый закат.', 'Russian'),
(uuid_generate_v4(), 'идти', 'Мы идём в парк.', 'Russian'),
(uuid_generate_v4(), 'стоять', 'Он стоит у окна.', 'Russian'),
(uuid_generate_v4(), 'сидеть', 'Она сидит за столом.', 'Russian'),
(uuid_generate_v4(), 'лежать', 'Книга лежит на столе.', 'Russian'),
(uuid_generate_v4(), 'любить', 'Я люблю свою семью.', 'Russian'),
(uuid_generate_v4(), 'хотеть', 'Он хочет пить.', 'Russian'),
(uuid_generate_v4(), 'мочь', 'Я могу помочь тебе.', 'Russian'),
(uuid_generate_v4(), 'делать', 'Что ты делаешь?', 'Russian'),
(uuid_generate_v4(), 'работать', 'Она работает в офисе.', 'Russian'),
(uuid_generate_v4(), 'учиться', 'Я учусь в университете.', 'Russian'),
(uuid_generate_v4(), 'жить', 'Мы живём в Москве.', 'Russian'),
(uuid_generate_v4(), 'играть', 'Дети играют в футбол.', 'Russian'),
(uuid_generate_v4(), 'петь', 'Она поёт прекрасно.', 'Russian'),
(uuid_generate_v4(), 'танцевать', 'Они танцуют на вечеринке.', 'Russian'),
(uuid_generate_v4(), 'готовить', 'Я готовлю ужин.', 'Russian'),
(uuid_generate_v4(), 'есть', 'Он ест яблоко.', 'Russian'),
(uuid_generate_v4(), 'пить', 'Я пью чай.', 'Russian'),
(uuid_generate_v4(), 'спать', 'Она спит уже два часа.', 'Russian'),
(uuid_generate_v4(), 'просыпаться', 'Я просыпаюсь в семь утра.', 'Russian'),
(uuid_generate_v4(), 'вставать', 'Он встаёт рано.', 'Russian'),
(uuid_generate_v4(), 'одеваться', 'Она одевается быстро.', 'Russian'),
(uuid_generate_v4(), 'мыться', 'Я моюсь каждый день.', 'Russian'),
(uuid_generate_v4(), 'чистить', 'Он чистит зубы.', 'Russian'),
(uuid_generate_v4(), 'бриться', 'Он бреется утром.', 'Russian'),
(uuid_generate_v4(), 'готовиться', 'Я готовлюсь к экзамену.', 'Russian'),
(uuid_generate_v4(), 'ждать', 'Она ждёт автобус.', 'Russian'),
(uuid_generate_v4(), 'искать', 'Он ищет ключи.', 'Russian'),
(uuid_generate_v4(), 'находить', 'Я нашёл решение.', 'Russian'),
(uuid_generate_v4(), 'давать', 'Она даёт совет.', 'Russian'),
(uuid_generate_v4(), 'брать', 'Я беру книгу.', 'Russian'),
(uuid_generate_v4(), 'покупать', 'Он покупает хлеб.', 'Russian'),
(uuid_generate_v4(), 'продавать', 'Она продаёт машину.', 'Russian'),
(uuid_generate_v4(), 'открывать', 'Я открываю дверь.', 'Russian'),
(uuid_generate_v4(), 'закрывать', 'Он закрывает окно.', 'Russian'),
(uuid_generate_v4(), 'включать', 'Она включает свет.', 'Russian'),
(uuid_generate_v4(), 'выключать', 'Я выключаю телевизор.', 'Russian');

-- Добавление тегов и их значений
INSERT INTO Tag_Value (ID, ""Tag"", Value) VALUES
(uuid_generate_v4(), 'time', 'past'), -- Например, время: настоящее
(uuid_generate_v4(), 'time', 'present'), -- Прошедшее
(uuid_generate_v4(), 'number', 'plural'), -- Множественное число
(uuid_generate_v4(), 'number', 'sinsle'); -- Множественное число

-- Связывание слов с тегами и значениями в таблице Vocabulary
INSERT INTO Vocabulary (ID, Tag_ValueID, word_id) VALUES
(uuid_generate_v4(), (SELECT ID FROM Tag_Value WHERE ""Tag"" = 'time' AND Value = 'past'), (SELECT ID FROM words WHERE name = 'run')),
(uuid_generate_v4(), (SELECT ID FROM Tag_Value WHERE ""Tag"" = 'number' AND Value = 'plural'), (SELECT ID FROM words WHERE name = 'run')),
(uuid_generate_v4(), (SELECT ID FROM Tag_Value WHERE ""Tag"" = 'time' AND Value = 'present'), (SELECT ID FROM words WHERE name = 'jump')),
(uuid_generate_v4(), (SELECT ID FROM Tag_Value WHERE ""Tag"" = 'number' AND Value = 'plural'), (SELECT ID FROM words WHERE name = 'read'));

-- Присвоение слов пользователю и словарю
INSERT INTO word_assighnment (ID, user_id, dictionary_id, word_id1, word_id2) VALUES
(uuid_generate_v4(), 
 (SELECT ID FROM ""User"" WHERE UserName = 'Alice'), 
 (SELECT ID FROM Dictionary WHERE Name = 'English Vocabulary'), 
 (SELECT ID FROM words WHERE name = 'run'), 
 (SELECT ID FROM words WHERE name = 'jump')),
(uuid_generate_v4(), 
 (SELECT ID FROM ""User"" WHERE UserName = 'Alice'), 
 (SELECT ID FROM Dictionary WHERE Name = 'English Vocabulary'), 
 (SELECT ID FROM words WHERE name = 'read'), 
 (SELECT ID FROM words WHERE name = 'run'));
";

        try
        {
            object isDBexist;
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("Подключение к PostgreSQL успешно установлено.");
                
                using (var checkCommand = new NpgsqlCommand($"SELECT 1 FROM pg_database WHERE datname = '{databaseName}'", connection))
                {
                    isDBexist = checkCommand.ExecuteScalar();
                    if (isDBexist == null)
                    {
                        using (var createDbCommand = new NpgsqlCommand($"CREATE DATABASE \"{databaseName}\"", connection))
                        {
                            createDbCommand.ExecuteNonQuery();
                            Console.WriteLine($"База данных '{databaseName}' успешно создана.");
                        }
                    } 
                    else
                    {
                        Console.WriteLine($"База данных '{databaseName}' уже существует.");
                    }
                    connection.Close();
                }
            }
            if (isDBexist == null)
            {
                connectionString = $"Host=localhost;port=5432;Username=postgres;Password=admin;Database={databaseName}";
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    using (var createDbCommand = new NpgsqlCommand())
                    {
                        createDbCommand.Connection = connection;
                        var commands = sqlScript.Split(new[] { ";\r\n", ";\n" }, StringSplitOptions.RemoveEmptyEntries);

                       
                        foreach (var cmd in commands)
                        {
                            createDbCommand.CommandText = cmd;
                            createDbCommand.ExecuteNonQuery();
                            Console.WriteLine($"Выполнена команда: {cmd}");
                        }
                        
                        Console.WriteLine($"База данных успешно заполнена!");
                    }
                    connection.Close();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
}