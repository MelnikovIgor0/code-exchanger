using code_exchanger_back.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Collections.Generic;
using System.Data.Common;

namespace code_exchanger_back.Services
{
    public class DBConnector
    {
        private DataBaseContext mainDataBase;
        private NpgsqlConnection dbConnection;

        public DBConnector()
        {
            mainDataBase = new DataBaseContext(new DbContextOptions<DataBaseContext>());
            mainDataBase.Database.OpenConnection();
            dbConnection = (Npgsql.NpgsqlConnection)mainDataBase.Database.GetDbConnection();
        }

        public Content GetContent(string link)
        {
            string command = $"SELECT * FROM \"Content\" WHERE \"link\"='{link}'";
            var reader = (new NpgsqlCommand(command, dbConnection)).ExecuteReader();
            Content result = new();
            if (reader.Read())
            {
                result = new Content()
                {
                    code = reader.GetString(0),
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

        public User GetUserByID(long id)
        {
            string command = $"SELECT * FROM \"Users\" WHERE \"ID\"='{id}'";
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

        public User GetUserByUserName(string username)
        {
            string command = $"SELECT * FROM \"Users\" WHERE \"username\"='{username}'";
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

        public string CreateRecord(string content, long id, long authorID, int language, string password)
        {
            string date = $"{System.DateTime.Now.Year.ToString("D4")}-" +
                $"{System.DateTime.Now.Month.ToString("D2")}-" +
                $"{System.DateTime.Now.Day.ToString("D2")}";
            string guid = System.Guid.NewGuid().ToString();
            string command = $"INSERT INTO \"Content\" (\"code\", \"creation_time\", \"authorID\", \"language\", \"password\", \"ID\", \"link\")" +
                $"VALUES('{content}', '{date}', {authorID}, {language}, '{password}', {id}, '{guid}')";
            var reader = (new NpgsqlCommand(command, dbConnection)).ExecuteReader();
            reader.Close();
            return guid;
        }

        public void CreateUser(long id, string username, string password)
        {
            string command = $"INSERT INTO \"Users\" (\"ID\", \"username\", \"password\") VALUES ({id}, '{username}', '{password}')";
            var reader = (new NpgsqlCommand(command, dbConnection)).ExecuteReader();
            reader.Close();
        }

        public int GetAmountUsers()
        {
            string command = "SELECT MAX(\"ID\") FROM \"Users\"";
            var reader = (new NpgsqlCommand(command, dbConnection)).ExecuteReader();
            reader.Read();
            int result = reader.GetInt32(0);
            reader.Close();
            return result;
        }

        public Content[] GetContentByUserID(long id)
        {
            string command = $"SELECT * FROM \"Content\" WHERE \"authorID\"='{id}'";
            var reader = (new NpgsqlCommand(command, dbConnection)).ExecuteReader();
            List<Content> result = new List<Content>();
            while (reader.Read())
            {
                result.Add(new Content()
                {
                    code = reader.GetString(0),
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

        public void DeleteContent(long id)
        {
            string command = $"DELETE FROM \"Content\" WHERE \"ID\" = {id}";
            var reader = (new NpgsqlCommand(command, dbConnection)).ExecuteReader();
            while (reader.Read());
            reader.Close();
        }

        public long GetMaxContentID()
        {
            string command = "SELECT MAX(\"ID\") FROM \"Content\"";
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

        public void UpdateRecord(string link, string new_content)
        {
            string date = $"{System.DateTime.Now.Year.ToString("D4")}-" +
                $"{System.DateTime.Now.Month.ToString("D2")}-" +
                $"{System.DateTime.Now.Day.ToString("D2")}";
            string command = $"UPDATE \"Content\" SET \"code\" = '{new_content}', \"creation_time\"='{date}' WHERE \"link\" = '{link}'";
            var reader = (new NpgsqlCommand(command, dbConnection)).ExecuteReader();
            while (reader.Read());
            reader.Close();
        }

        public void DeleteUser(string username)
        {
            string command = $"DELETE FROM \"Users\" WHERE \"username\" = '{username}'";
            var reader = (new NpgsqlCommand(command, dbConnection)).ExecuteReader();
            while (reader.Read());
            reader.Close();
        }

        public void DeleteContentByAuthor(long authorId)
        {
            string command = $"DELETE FROM \"Content\" WHERE \"authorID\" = {authorId}";
            var reader = (new NpgsqlCommand(command, dbConnection)).ExecuteReader();
            while (reader.Read());
            reader.Close();
        }

        public void UpdatePassword(string username, string password)
        {
            string command = $"UPDATE \"Users\" SET \"password\" = '{password}' WHERE \"username\" = '{username}'";
            var reader = (new NpgsqlCommand(command, dbConnection)).ExecuteReader();
            while (reader.Read());
            reader.Close();
        }
    }
}
