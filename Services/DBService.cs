using Microsoft.Extensions.Configuration;
using Npgsql;
using Projekt.Models;

namespace Projekt.Services
{
    public partial class DBService
    {
        private readonly string _connectionString;

        public DBService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    
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

        // Get All Users
        public async Task<List<Users>> GetAllUsersAsync()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new NpgsqlCommand("SELECT * FROM users", connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    var users = new List<Users>();

                    while (await reader.ReadAsync())
                    {
                        users.Add(
                          new Users
                          {
                              Id = reader.GetInt32(reader.GetOrdinal("id")),
                              Name = reader.GetString(reader.GetOrdinal("name")),
                              Email = reader.GetString(reader.GetOrdinal("email")),
                              PhoneCode = reader.GetInt32(reader.GetOrdinal("phone_code")),
                              PhoneNumber = reader.GetString(reader.GetOrdinal("phone_number")),
                          }
                      );
                    }

                    return users;
                }
            }
        }
    }
}
