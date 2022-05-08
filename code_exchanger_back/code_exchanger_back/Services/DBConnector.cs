using code_exchanger_back.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Collections.Generic;
using System.Data.Common;
using code_exchanger_back.Settings;

namespace code_exchanger_back.Services
{
    public class DBConnector
    {
        private NpgsqlConnection dbConnection;

        public DBConnector()
        {
            DataBaseContext mainDataBase = new DataBaseContext(new DbContextOptions<DataBaseContext>());
            mainDataBase.Database.OpenConnection();
            dbConnection = (Npgsql.NpgsqlConnection)mainDataBase.Database.GetDbConnection();
        }

        private string ReadContentText(string link)
        {
            try
            {
                return System.IO.File.ReadAllText($"{Constants.ContentPath}\\{link}");
            }
            catch
            {
                return null;
            }
        }

        private bool WriteContentText(string link, string content)
        {
            try
            {
                System.IO.File.WriteAllText($"{Constants.ContentPath}\\{link}", content);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void DeleteContentText(string link)
        {
            System.IO.File.Delete($"{Constants.ContentPath}\\{link}");
        }

        private string ParseDate(System.DateTime moment)
        {
            return $"{moment.Year.ToString("D4")}-{moment.Month.ToString("D2")}-{moment.Day.ToString("D2")}";
        }

        public Content GetContent(string link)
        {
            string command = $"SELECT * FROM \"Content\" WHERE \"link\"='{link}'";
            lock (dbConnection)
            {
                var reader = (new NpgsqlCommand(command, dbConnection)).ExecuteReader();
                Content result = new();
                if (reader.Read())
                {
                    result = new Content()
                    {
                        code = ReadContentText(reader.GetString(0)),
                        creation_time = reader.GetProviderSpecificValue(1),
                        authorID = reader.GetInt64(2),
                        language = reader.GetByte(3),
                        password = reader.GetString(4),
                        ID = reader.GetInt64(5),
                        link = reader.GetString(6),
                    };
                }
                else
                {
                    reader.Close();
                    return null;
                }
                reader.Close();
                return result;
            }
        }

        public User GetUserByID(long id)
        {
            string command = $"SELECT * FROM \"Users\" WHERE \"ID\"='{id}'";
            lock (dbConnection)
            {
                var reader = (new NpgsqlCommand(command, dbConnection)).ExecuteReader();
                if (reader.Read())
                {
                    User result = new User()
                    {
                        ID = reader.GetInt64(0),
                        username = reader.GetString(1),
                        password = reader.GetString(2)
                    };
                    reader.Close();
                    return result;
                }
                reader.Close();
                return null;
            }
        }

        public User GetUserByUserName(string username)
        {
            string command = $"SELECT * FROM \"Users\" WHERE \"username\"='{username}'";
            lock (dbConnection)
            {
                var reader = (new NpgsqlCommand(command, dbConnection)).ExecuteReader();
                if (reader.Read())
                {
                    User result = new User()
                    {
                        ID = reader.GetInt64(0),
                        username = reader.GetString(1),
                        password = reader.GetString(2)
                    };
                    reader.Close();
                    return result;
                }
                reader.Close();
                return null;
            }
        }

        public string CreateRecord(string content, long id, long authorID, int language, string password)
        {
            string guid = System.Guid.NewGuid().ToString();
            string command = $"INSERT INTO \"Content\" (\"code\", \"creation_time\", \"authorID\", \"language\", \"password\", \"ID\", \"link\")" +
                $"VALUES('{guid}', '{ParseDate(System.DateTime.Now)}', {authorID}, {language}, '{password}', {id}, '{guid}')";
            WriteContentText(guid, content);
            lock (dbConnection)
            {
                var reader = (new NpgsqlCommand(command, dbConnection)).ExecuteReader();
                reader.Close();
                return guid;
            }
        }

        public void CreateUser(long id, string username, string password)
        {
            string command = $"INSERT INTO \"Users\" (\"ID\", \"username\", \"password\") VALUES ({id}, '{username}', '{password}')";
            lock (dbConnection)
            {
                var reader = (new NpgsqlCommand(command, dbConnection)).ExecuteReader();
                reader.Close();
            }
        }

        public int GetAmountUsers()
        {
            string command = "SELECT MAX(\"ID\") FROM \"Users\"";
            lock (dbConnection)
            {
                var reader = (new NpgsqlCommand(command, dbConnection)).ExecuteReader();
                reader.Read();
                if (reader.IsDBNull(0))
                {
                    reader.Close();
                    return 0;
                }
                int result = reader.GetInt32(0);
                reader.Close();
                return result;
            }
        }

        public Content[] GetContentByUserID(long id)
        {
            string command = $"SELECT * FROM \"Content\" WHERE \"authorID\"='{id}'";
            lock (dbConnection)
            {
                var reader = (new NpgsqlCommand(command, dbConnection)).ExecuteReader();
                List<Content> result = new List<Content>();
                while (reader.Read())
                {
                    result.Add(new Content()
                    {
                        code = ReadContentText(reader.GetString(0)),
                        creation_time = reader.GetProviderSpecificValue(1),
                        authorID = reader.GetInt64(2),
                        language = reader.GetByte(3),
                        password = reader.GetString(4),
                        ID = reader.GetInt64(5),
                        link = reader.GetString(6),
                    });
                }
                reader.Close();
                return result.ToArray();
            }
        }

        public void DeleteContent(string link)
        {
            DeleteContentText(link);
            string command = $"DELETE FROM \"Content\" WHERE \"link\" = {link}";
            lock (dbConnection)
            {
                var reader = (new NpgsqlCommand(command, dbConnection)).ExecuteReader();
                while (reader.Read());
                reader.Close();
            }
        }

        public long GetMaxContentID()
        {
            string command = "SELECT MAX(\"ID\") FROM \"Content\"";
            lock (dbConnection)
            {
                var reader = (new NpgsqlCommand(command, dbConnection)).ExecuteReader();
                reader.Read();
                if (reader.IsDBNull(0))
                {
                    reader.Close();
                    return 0;
                }
                long ans = reader.GetInt64(0);
                reader.Close();
                return ans;
            }
        }

        public void UpdateRecord(string link, string new_content)
        {
            WriteContentText(link, new_content);
            string command = $"UPDATE \"Content\" SET \"creation_time\"='{ParseDate(System.DateTime.Now)}' WHERE \"link\" = '{link}'";
            lock (dbConnection)
            {
                var reader = (new NpgsqlCommand(command, dbConnection)).ExecuteReader();
                while (reader.Read());
                reader.Close();
            }
        }

        public void DeleteUser(string username)
        {
            string command = $"DELETE FROM \"Users\" WHERE \"username\" = '{username}'";
            lock (dbConnection)
            {
                var reader = (new NpgsqlCommand(command, dbConnection)).ExecuteReader();
                while (reader.Read());
                reader.Close();
            }
        }

        public void DeleteContentByAuthor(long authorId)
        {
            foreach (Content c in GetContentByUserID(authorId)) DeleteContentText(c.link);
            string command = $"DELETE FROM \"Content\" WHERE \"authorID\" = {authorId}";
            lock (dbConnection)
            {
                var reader = (new NpgsqlCommand(command, dbConnection)).ExecuteReader();
                while (reader.Read());
                reader.Close();
            }
        }

        public void UpdatePassword(string username, string password)
        {
            string command = $"UPDATE \"Users\" SET \"password\" = '{password}' WHERE \"username\" = '{username}'";
            lock (dbConnection)
            {
                var reader = (new NpgsqlCommand(command, dbConnection)).ExecuteReader();
                while (reader.Read());
                reader.Close();
            }
        }

        public void DeleteOldContent(System.DateTime border)
        {
            string command = $"DELETE FROM \"Content\" WHERE \"creation_time\" <= '{ParseDate(border)}'";
            lock (dbConnection)
            {
                var reader = (new NpgsqlCommand(command, dbConnection)).ExecuteReader();
                while (reader.Read());
                reader.Close();
            }
        }
    }
}
