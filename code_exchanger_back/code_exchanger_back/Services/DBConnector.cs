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
                    password = (byte[])reader.GetProviderSpecificValue(4),
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

        public User GetUserByID(int id)
        {
            string command = $"SELECT * FROM \"Users\" WHERE \"ID\"='{id}'";
            var reader = (new NpgsqlCommand(command, dbConnection)).ExecuteReader();
            if (reader.Read())
            {
                User result = new User()
                {
                    ID = reader.GetInt64(0),
                    username = reader.GetString(1),
                    password = (byte[])reader.GetProviderSpecificValue(2)
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
                    password = (byte[])reader.GetProviderSpecificValue(2)
                };
                reader.Close();
                return result;
            }
            reader.Close();
            return null;
        }

        public string CreateRecord(string content, int id, int authorID, int language, byte[] password)
        {
            string date = $"{System.DateTime.Now.Year.ToString("D4")}-" +
                $"{System.DateTime.Now.Month.ToString("D2")}-" +
                $"{System.DateTime.Now.Day.ToString("D2")}";
            string guid = System.Guid.NewGuid().ToString();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (password != null && password.Length > 0)
            {
                sb.Append($"{{{password[0]}");
                for (int i = 1; i < password.Length; i++)
                    sb.Append($", {password[i]}");
                sb.Append('}');
            }
            else
            {
                sb.Append("{}");
            }
            string command = $"INSERT INTO \"Content\" (\"code\", \"creation_time\", \"authorID\", \"language\", \"password\", \"ID\", \"link\")" +
                $"VALUES('{content}', '{date}', {authorID}, {language}, '{sb}', {id}, '{guid}')";
            var reader = (new NpgsqlCommand(command, dbConnection)).ExecuteReader();
            reader.Close();
            return guid;
        }

        public void CreateUser(int id, string username, byte[] password)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder($"{{{password[0]}");
            for (int i = 1; i < password.Length; i++)
                sb.Append($", {password[i]}");
            sb.Append('}');
            string command = $"INSERT INTO \"Users\" (\"ID\", \"username\", \"password\") VALUES ({id}, '{username}', '{sb}')";
            var reader = (new NpgsqlCommand(command, dbConnection)).ExecuteReader();
            reader.Close();
        }

        public int GetAmountUsers()
        {
            string command = "SELECT COUNT(*) FROM \"Users\"";
            var reader = (new NpgsqlCommand(command, dbConnection)).ExecuteReader();
            reader.Read();
            int result = reader.GetInt32(0);
            reader.Close();
            return result;
        }
    }
}
