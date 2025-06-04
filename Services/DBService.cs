using Npgsql;
using Projekt.Models;

namespace Projekt.Services
{
    public class DBService
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
                await using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);

                await conn.OpenAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        #region Users

        public async Task<List<User>> GetAllUsersAsync()
        {
            await using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, name, email, phone_code, phone_number FROM users ORDER BY name", conn);
            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            List<User> list = new List<User>();

            while (await reader.ReadAsync())
            {
                list.Add(new User
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Email = reader.GetString(2),
                    PhoneCode = reader.GetInt32(3),
                    PhoneNumber = reader.GetString(4),
                });
            }

            return list;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            await using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM users WHERE email = @email", conn);

            cmd.Parameters.AddWithValue("email", email);

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return null;
            }

            return new User
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Email = reader.GetString(2),
                PhoneCode = reader.GetInt32(3),
                PhoneNumber = reader.GetString(4),
                Password = reader.GetString(5)
            };
        }

        public async Task<int?> AddUserAsync(User user)
        {
            const string sql = @"
                INSERT INTO users (name, email, phone_code, phone_number, password)
                VALUES (@name, @email, @phone_code, @phone_number, @password)
                RETURNING id;
            ";

            await using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("name", user.Name);
            cmd.Parameters.AddWithValue("email", user.Email);
            cmd.Parameters.AddWithValue("phone_code", user.PhoneCode);
            cmd.Parameters.AddWithValue("phone_number", user.PhoneNumber);
            cmd.Parameters.AddWithValue("password", user.Password);

            object? result = await cmd.ExecuteScalarAsync();

            int? id = result is not null ? (int)result : null;

            return id;
        }

        #endregion

        #region Brands

        public async Task<List<Brand>> GetAllBrandsAsync()
        {
            await using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, name FROM brands ORDER BY name", conn);
            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            List<Brand> list = new List<Brand>();

            while (await reader.ReadAsync())
            {
                list.Add(new Brand
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                });
            }

            return list;
        }

        #endregion

        #region Categories

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            await using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using NpgsqlCommand cmd = new NpgsqlCommand("SELECT id, name, parent_category_id FROM categories ORDER BY name", conn);
            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            List<Category> list = new List<Category>();

            while (await reader.ReadAsync())
            {
                list.Add(new Category
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    ParentCategoryId = reader.IsDBNull(2) ? null : reader.GetInt32(2)
                });
            }

            return list;
        }

        #endregion

        #region Products

        public async Task<List<Products>> GetAllProductsAsync()
        {
            const string sql = @"
        SELECT 
            p.id,
            p.name,
            p.description,
            p.price,
            p.color,
            p.size,
            p.quantity,
            p.used,
            p.brand_id,
            p.category_id,
            p.user_id,
            p.image,
            b.name    AS brand_name,
            c.name    AS category_name,
            u.name    AS user_name
        FROM products p
        JOIN brands     b ON b.id = p.brand_id
        JOIN categories c ON c.id = p.category_id
        LEFT JOIN users  u ON u.id = p.user_id
        ORDER BY p.name;
    ";

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new NpgsqlCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            var list = new List<Products>();

            while (await reader.ReadAsync())
            {
                list.Add(new Products
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.GetString(2),
                    Price = reader.GetDecimal(3),
                    Color = reader.IsDBNull(4) ? null : reader.GetString(4),
                    Size = reader.IsDBNull(5) ? null : reader.GetString(5),
                    Quantity = reader.GetInt32(6),
                    Used = reader.GetBoolean(7),
                    BrandId = reader.GetInt32(8),
                    CategoryId = reader.GetInt32(9),
                    UserId = reader.GetInt32(10),
                    Image = reader.IsDBNull(11) ? null : reader.GetString(11),
                    BrandName = reader.GetString(12),
                    CategoryName = reader.GetString(13),
                    UserName = reader.IsDBNull(14) ? null : reader.GetString(14)
                });
            }

            return list;
        }


        public async Task<int?> AddProductAsync(Products prod)
        {
            const string sql = @"
                INSERT INTO products (name, description, price, color, size, quantity, used, brand_id, category_id, user_id, image)
                VALUES (@name, @desc, @price, @color, @size, @qty, @used, @brand, @cat, @user, @image)
                RETURNING id;
            ";

            await using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("name", prod.Name);
            cmd.Parameters.AddWithValue("desc", prod.Description ?? "");
            cmd.Parameters.AddWithValue("price", prod.Price);
            cmd.Parameters.AddWithValue("color", (object?)prod.Color ?? DBNull.Value);
            cmd.Parameters.AddWithValue("size", (object?)prod.Size ?? DBNull.Value);
            cmd.Parameters.AddWithValue("qty", prod.Quantity);
            cmd.Parameters.AddWithValue("used", prod.Used);
            cmd.Parameters.AddWithValue("brand", prod.BrandId);
            cmd.Parameters.AddWithValue("cat", prod.CategoryId);
            cmd.Parameters.AddWithValue("user", prod.UserId);

            if (string.IsNullOrWhiteSpace(prod.Image))
            {
                cmd.Parameters.AddWithValue("image", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("image", prod.Image);
            }

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            object? result = await cmd.ExecuteScalarAsync();

            int? id = result is not null ? (int)result : null;

            return id;
        }

        public async Task<Products?> GetProductByIdAsync(int id)
        {
            const string sql = @"
                SELECT 
                    p.id,
                    p.name,
                    p.description,
                    p.price,
                    p.color,
                    p.size,
                    p.quantity,
                    p.used,
                    p.brand_id,
                    p.category_id,
                    p.user_id,
                    p.image,
                    b.name    AS brand_name,
                    c.name    AS category_name,
                    u.name    AS user_name
                FROM products p
                JOIN brands     b ON b.id = p.brand_id
                JOIN categories c ON c.id = p.category_id
                LEFT JOIN users  u ON u.id = p.user_id
                WHERE p.id = @id;
            ";

            await using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("id", id);

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return null;
            }

            return new Products
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.GetString(2),
                Price = reader.GetDecimal(3),
                Color = reader.IsDBNull(4) ? null : reader.GetString(4),
                Size = reader.IsDBNull(5) ? null : reader.GetString(5),
                Quantity = reader.GetInt32(6),
                Used = reader.GetBoolean(7),
                BrandId = reader.GetInt32(8),
                CategoryId = reader.GetInt32(9),
                UserId = reader.GetInt32(10),
                Image = reader.IsDBNull(11) ? null : reader.GetString(11),
                BrandName = reader.GetString(12),
                CategoryName = reader.GetString(13),
                UserName = reader.IsDBNull(14) ? null : reader.GetString(14)
            };
        }

        public async Task<bool> UpdateProductAsync(Products prod)
        {
            const string sql = @"
                UPDATE products SET 
                    name        = @name,
                    description = @description,
                    price       = @price,
                    color       = @color,
                    size        = @size,
                    quantity    = @quantity,
                    used        = @used,
                    brand_id    = @brand,
                    category_id = @category
                 WHERE id = @id;
            ";

            await using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("name", prod.Name);
            cmd.Parameters.AddWithValue("description", prod.Description ?? string.Empty);
            cmd.Parameters.AddWithValue("price", prod.Price);
            cmd.Parameters.AddWithValue("color", (object?)prod.Color ?? DBNull.Value);
            cmd.Parameters.AddWithValue("size", (object?)prod.Size ?? DBNull.Value);
            cmd.Parameters.AddWithValue("quantity", prod.Quantity);
            cmd.Parameters.AddWithValue("used", prod.Used);
            cmd.Parameters.AddWithValue("brand", prod.BrandId);
            cmd.Parameters.AddWithValue("category", prod.CategoryId);
            cmd.Parameters.AddWithValue("id", prod.Id);

            int rowsAffected = await cmd.ExecuteNonQueryAsync();

            return rowsAffected == 1;
        }

        #endregion
    }
}
