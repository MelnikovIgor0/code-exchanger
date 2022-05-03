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
                    password = (byte[][])reader.GetProviderSpecificValue(4),
                    ID = reader.GetInt64(5),
                    link = reader.GetString(6),
                };
                foreach (var el in result.password)
                {
                    foreach (var e in el)
                    {
                        System.Diagnostics.Debug.WriteLine(e + " ");
                    }
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("NO CONTENT");
            }
            reader.Close();
            return result;
        }

        public string CreateRecord(string content, int id)
        {
            string date = $"{System.DateTime.Now.Year.ToString("D4")}-" +
                $"{System.DateTime.Now.Month.ToString("D2")}-" +
                $"{System.DateTime.Now.Day.ToString("D2")}";
            string guid = System.Guid.NewGuid().ToString();
            string command = $"INSERT INTO \"Content\" (\"code\", \"creation_time\", \"authorID\", \"language\", \"password\", \"ID\", \"link\")" +
                $"VALUES('{content}', '{date}', 0, 0, '{{1, 2}}', {id}, '{guid}')";
            var reader = (new NpgsqlCommand(command, dbConnection)).ExecuteReader();
            reader.Close();
            return guid;
        }
    }
}
