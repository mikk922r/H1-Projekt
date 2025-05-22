using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Projekt.Models;

namespace Projekt.Services
{
    public partial class DBService
    {
        private readonly string _connectionString;

        public DBService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                await using var conn = new NpgsqlConnection(_connectionString);
                await conn.OpenAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

       

        public async Task<List<Users>> GetAllUsersAsync()
        {
            var list = new List<Users>();
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            const string sql = "SELECT id, name, email, phone_code, phone_number FROM users;";
            await using var cmd = new NpgsqlCommand(sql, conn);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new Users
                {
                    ID = reader.GetInt32(reader.GetOrdinal("id")),
                    name = reader.GetString(reader.GetOrdinal("name")),
                    email = reader.GetString(reader.GetOrdinal("email")),
                    phone_code = reader.GetInt32(reader.GetOrdinal("phone_code")),
                    phone_number = reader.GetString(reader.GetOrdinal("phone_number"))
                });
            }

            return list;
        }

     
    }
}
