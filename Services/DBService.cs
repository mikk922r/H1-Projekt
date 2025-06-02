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

        public async Task<List<Users>> GetAllUsersAsync()
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand("SELECT id, name, email, phone_code, phone_number FROM users ORDER BY name", conn);
            using var rdr = await cmd.ExecuteReaderAsync();

            var list = new List<Users>();
            while (await rdr.ReadAsync())
            {
                list.Add(new Users
                {
                    Id = rdr.GetInt32(0),
                    Name = rdr.GetString(1),
                    Email = rdr.GetString(2),
                    PhoneCode = rdr.GetInt32(3),
                    PhoneNumber = rdr.GetString(4)
                });
            }
            return list;
        }

        public async Task<Users?> GetUserByEmailAsync(string email)
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand(
                "SELECT id, name, email, phone_code, phone_number FROM users WHERE email=@e", conn);
            cmd.Parameters.AddWithValue("e", email);
            using var rdr = await cmd.ExecuteReaderAsync();
            if (!await rdr.ReadAsync()) return null;

            return new Users
            {
                Id = rdr.GetInt32(0),
                Name = rdr.GetString(1),
                Email = rdr.GetString(2),
                PhoneCode = rdr.GetInt32(3),
                PhoneNumber = rdr.GetString(4)
            };
        }

        public async Task<int> AddUserAsync(Users user)
        {
            const string sql = @"
                INSERT INTO users (name, email, phone_code, phone_number)
                VALUES (@name, @email, @code, @phone)
                RETURNING id;";
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("name", user.Name);
            cmd.Parameters.AddWithValue("email", user.Email);
            cmd.Parameters.AddWithValue("code", user.PhoneCode);
            cmd.Parameters.AddWithValue("phone", user.PhoneNumber);
            return (int)await cmd.ExecuteScalarAsync();
        }

        public async Task<List<Brand>> GetAllBrandsAsync()
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand("SELECT id, name FROM brands ORDER BY name", conn);
            using var rdr = await cmd.ExecuteReaderAsync();

            var list = new List<Brand>();
            while (await rdr.ReadAsync())
            {
                list.Add(new Brand
                {
                    Id = rdr.GetInt32(0),
                    Name = rdr.GetString(1)
                });
            }
            return list;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand("SELECT id, name, parent_category_id FROM categories ORDER BY name", conn);
            using var rdr = await cmd.ExecuteReaderAsync();

            var list = new List<Category>();
            while (await rdr.ReadAsync())
            {
                list.Add(new Category
                {
                    Id = rdr.GetInt32(0),
                    Name = rdr.GetString(1),
                });
            }
            return list;
        }

        public async Task<int> AddProductAsync(Products prod)
        {
            const string sql = @"
                INSERT INTO products
                  (name, description, price, color, size, quantity, used,
                   brand_id, category_id, user_id)
                VALUES
                  (@name, @desc, @price, @color, @size, @qty, @used,
                   @brand, @cat, @user)
                RETURNING id;
            ";

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("name", prod.Name);
            cmd.Parameters.AddWithValue("desc", prod.Description);
            cmd.Parameters.AddWithValue("price", prod.Price);
            cmd.Parameters.AddWithValue("color", (object?)prod.Color ?? DBNull.Value);
            cmd.Parameters.AddWithValue("size", (object?)prod.Size ?? DBNull.Value);
            cmd.Parameters.AddWithValue("qty", prod.Quantity);
            cmd.Parameters.AddWithValue("used", prod.Used);
            cmd.Parameters.AddWithValue("brand", prod.BrandId);
            cmd.Parameters.AddWithValue("cat", prod.CategoryId);
            cmd.Parameters.AddWithValue("user", prod.UserId);

            return (int)await cmd.ExecuteScalarAsync();
        }
    }
}
