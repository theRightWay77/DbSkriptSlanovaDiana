using System;
using Npgsql;

class Program
{
    static void Main(string[] args)
    {
        string connectionString = "Host=localhost;port=5432;Username=postgres;Password=admin;Database=postgres";
        string databaseName = "SlanDB1";
        string sqlScript = @"
CREATE EXTENSION IF NOT EXISTS ""uuid-ossp"";
CREATE TABLE Dictionary (
  ID        uuid DEFAULT uuid_generate_v4() NOT NULL, 
  Name      text NOT NULL, 
  create_at timestamp, 
  user_id   uuid NOT NULL, 
  PRIMARY KEY (ID));
CREATE TABLE Tag_Value (
  ID    uuid DEFAULT uuid_generate_v4() NOT NULL, 
  ""Tag"" text NOT NULL, 
  Value text NOT NULL, 
  PRIMARY KEY (ID));
CREATE TABLE ""User"" (
  ID       uuid NOT NULL, 
  UserName text NOT NULL UNIQUE, 
  Email    text UNIQUE, 
  PRIMARY KEY (ID));
CREATE TABLE Vocabulary (
  ID          uuid DEFAULT uuid_generate_v4() NOT NULL, 
  Tag_ValueID uuid NOT NULL, 
  word_id     uuid NOT NULL, 
  language    text, 
  PRIMARY KEY (ID));
CREATE TABLE word_assighnment (
  ID            uuid NOT NULL, 
  user_id       uuid NOT NULL, 
  dictionary_id uuid NOT NULL, 
  word_id1      uuid NOT NULL,
  word_id2      uuid NOT NULL,
  PRIMARY KEY (ID));
CREATE TABLE words (
  ID      uuid NOT NULL, 
  name    text, 
  example text, 
  PRIMARY KEY (ID));
ALTER TABLE Vocabulary ADD CONSTRAINT FKVocabulary440486 FOREIGN KEY (Tag_ValueID) REFERENCES Tag_Value (ID);
ALTER TABLE Dictionary ADD CONSTRAINT FKDictionary939139 FOREIGN KEY (user_id) REFERENCES ""User"" (ID);
ALTER TABLE word_assighnment ADD CONSTRAINT FKword_assig914040 FOREIGN KEY (user_id) REFERENCES ""User"" (ID);
ALTER TABLE word_assighnment ADD CONSTRAINT FKword_assig514825 FOREIGN KEY (dictionary_id) REFERENCES Dictionary (ID);
ALTER TABLE word_assighnment ADD CONSTRAINT FKword_assig405610 FOREIGN KEY (word_id1) REFERENCES words (ID);
ALTER TABLE word_assighnment ADD CONSTRAINT FKword_assig405611 FOREIGN KEY (word_id2) REFERENCES words (ID);
ALTER TABLE Vocabulary ADD CONSTRAINT FKVocabulary877948 FOREIGN KEY (word_id) REFERENCES words (ID);

-- Создание пользователей
INSERT INTO ""User"" (ID, UserName, Email) VALUES
(uuid_generate_v4(), 'Alice', 'alice@example.com'),
(uuid_generate_v4(), 'Bob', 'bob@example.com');

-- Создание словаря
INSERT INTO Dictionary (ID, Name, create_at, user_id) VALUES
(uuid_generate_v4(), 'English Vocabulary', NOW(), (SELECT ID FROM ""User"" WHERE UserName = 'Alice'));

-- Добавление слов
INSERT INTO words (ID, name, example) VALUES
(uuid_generate_v4(), 'run', 'I run every morning.'),
(uuid_generate_v4(), 'jump', 'He can jump very high.'),
(uuid_generate_v4(), 'read', 'She likes to read books.');

-- Добавление тегов и их значений
INSERT INTO Tag_Value (ID, ""Tag"", Value) VALUES
(uuid_generate_v4(), 'time', 'past'), -- Например, время: настоящее
(uuid_generate_v4(), 'time', 'present'), -- Прошедшее
(uuid_generate_v4(), 'number', 'plural'), -- Множественное число
(uuid_generate_v4(), 'number', 'sinsle'); -- Множественное число

-- Связывание слов с тегами и значениями в таблице Vocabulary
INSERT INTO Vocabulary (ID, Tag_ValueID, word_id, language) VALUES
(uuid_generate_v4(), (SELECT ID FROM Tag_Value WHERE ""Tag"" = 'time' AND Value = 'past'), (SELECT ID FROM words WHERE name = 'run'), 'English'),
(uuid_generate_v4(), (SELECT ID FROM Tag_Value WHERE ""Tag"" = 'number' AND Value = 'plural'), (SELECT ID FROM words WHERE name = 'run'), 'English'),
(uuid_generate_v4(), (SELECT ID FROM Tag_Value WHERE ""Tag"" = 'time' AND Value = 'present'), (SELECT ID FROM words WHERE name = 'jump'), 'English'),
(uuid_generate_v4(), (SELECT ID FROM Tag_Value WHERE ""Tag"" = 'number' AND Value = 'plural'), (SELECT ID FROM words WHERE name = 'read'), 'English');

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
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("Подключение к PostgreSQL успешно установлено.");
                
                using (var checkCommand = new NpgsqlCommand($"SELECT 1 FROM pg_database WHERE datname = '{databaseName}'", connection))
                {
                    var result = checkCommand.ExecuteScalar();
                    if (result == null)
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
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
}