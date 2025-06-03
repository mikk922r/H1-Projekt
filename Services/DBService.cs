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

        public async Task<List<User>> GetAllUsersAsync()
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand(
                "SELECT id, name, email, phone_code, phone_number FROM users ORDER BY name", conn);
            using var rdr = await cmd.ExecuteReaderAsync();

            var list = new List<User>();
            while (await rdr.ReadAsync())
            {
                list.Add(new User
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

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            using var cmd = new NpgsqlCommand(
                "SELECT id, name, email, phone_code, phone_number FROM users WHERE email = @e", conn);
            cmd.Parameters.AddWithValue("e", email);
            using var rdr = await cmd.ExecuteReaderAsync();
            if (!await rdr.ReadAsync())
                return null;

            return new User
            {
                Id = rdr.GetInt32(0),
                Name = rdr.GetString(1),
                Email = rdr.GetString(2),
                PhoneCode = rdr.GetInt32(3),
                PhoneNumber = rdr.GetString(4)
            };
        }

        public async Task<int> AddUserAsync(User user)
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
            using var cmd = new NpgsqlCommand(
                "SELECT id, name FROM brands ORDER BY name", conn);
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
            await using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            using NpgsqlCommand cmd = new NpgsqlCommand(
                "SELECT id, name, parent_category_id FROM categories ORDER BY name", conn);
            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            var list = new List<Category>();
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

        public async Task<int> AddProductAsync(Products prod)
        {
            const string sql = @"
                INSERT INTO products
                  (name, description, price, color, size, quantity, used,
                   brand_id, category_id, user_id, image)
                VALUES
                  (@name, @desc, @price, @color, @size, @qty, @used,
                   @brand, @cat, @user, @image)
                RETURNING id;
            ";

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            await using var cmd = new NpgsqlCommand(sql, conn);
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
                cmd.Parameters.AddWithValue("image", DBNull.Value);
            else
                cmd.Parameters.AddWithValue("image", prod.Image);

            return (int)await cmd.ExecuteScalarAsync();
        }

        public async Task<List<Products>> GetAllProductsAsync()
        {
            const string sql = @"
                SELECT id,
                       name,
                       description,
                       price,
                       color,
                       size,
                       quantity,
                       used,
                       brand_id,
                       category_id,
                       user_id,
                       image
                  FROM products
                 ORDER BY name;
            ";

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            await using var cmd = new NpgsqlCommand(sql, conn);
            using var rdr = await cmd.ExecuteReaderAsync();

            var list = new List<Products>();
            while (await rdr.ReadAsync())
            {
                list.Add(new Products
                {
                    Id = rdr.GetInt32(0),
                    Name = rdr.GetString(1),
                    Description = rdr.GetString(2),
                    Price = rdr.GetDecimal(3),
                    Color = rdr.IsDBNull(4) ? null : rdr.GetString(4),
                    Size = rdr.IsDBNull(5) ? null : rdr.GetString(5),
                    Quantity = rdr.GetInt32(6),
                    Used = rdr.GetBoolean(7),
                    BrandId = rdr.GetInt32(8),
                    CategoryId = rdr.GetInt32(9),
                    UserId = rdr.GetInt32(10),
                    Image = rdr.IsDBNull(11) ? null : rdr.GetString(11),

                    // The three new name‐fields remain null here; 
                    // only the detail lookup populates them.
                    BrandName = string.Empty,
                    CategoryName = string.Empty,
                    UserName = null
                });
            }
            return list;
        }

        // ───────────────────────────────────────────────────────────────────
        // UPDATED: now returns the three new name‐fields too
        // ───────────────────────────────────────────────────────────────────
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

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("id", id);

            using var rdr = await cmd.ExecuteReaderAsync();
            if (!await rdr.ReadAsync())
                return null;

            return new Products
            {
                Id = rdr.GetInt32(0),
                Name = rdr.GetString(1),
                Description = rdr.GetString(2),
                Price = rdr.GetDecimal(3),
                Color = rdr.IsDBNull(4) ? null : rdr.GetString(4),
                Size = rdr.IsDBNull(5) ? null : rdr.GetString(5),
                Quantity = rdr.GetInt32(6),
                Used = rdr.GetBoolean(7),
                BrandId = rdr.GetInt32(8),
                CategoryId = rdr.GetInt32(9),
                UserId = rdr.GetInt32(10),
                Image = rdr.IsDBNull(11) ? null : rdr.GetString(11),
                BrandName = rdr.GetString(12),
                CategoryName = rdr.GetString(13),
                UserName = rdr.IsDBNull(14) ? null : rdr.GetString(14)
            };
        }

        public async Task<bool> UpdateProductAsync(Products prod)
        {
            const string sql = @"
                UPDATE products
                   SET name        = @name,
                       description = @desc,
                       price       = @price,
                       color       = @color,
                       size        = @size,
                       quantity    = @qty,
                       used        = @used,
                       brand_id    = @brand,
                       category_id = @cat
                 WHERE id = @id;
            ";

            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();
            await using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("name", prod.Name);
            cmd.Parameters.AddWithValue("desc", prod.Description ?? "");
            cmd.Parameters.AddWithValue("price", prod.Price);
            cmd.Parameters.AddWithValue("color", (object?)prod.Color ?? DBNull.Value);
            cmd.Parameters.AddWithValue("size", (object?)prod.Size ?? DBNull.Value);
            cmd.Parameters.AddWithValue("qty", prod.Quantity);
            cmd.Parameters.AddWithValue("used", prod.Used);
            cmd.Parameters.AddWithValue("brand", prod.BrandId);
            cmd.Parameters.AddWithValue("cat", prod.CategoryId);
            cmd.Parameters.AddWithValue("id", prod.Id);

            var rowsAffected = await cmd.ExecuteNonQueryAsync();
            return rowsAffected == 1;
        }
    }
}
