using Npgsql;
using Projekt.Helpers;
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

        public async Task<List<Product>> GetAllProductsAsync()
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
            p.image,
            p.brand_id,
            p.category_id,
            p.user_id,
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

            var list = new List<Product>();

            while (await reader.ReadAsync())
            {
                list.Add(new Product
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Description = reader.GetString(2),
                    Price = reader.GetDecimal(3),
                    Color = ColorHelper.GetColorByValue(reader.GetInt32(4)),
                    Size = reader.GetString(5),
                    Quantity = reader.GetInt32(6),
                    Used = reader.GetBoolean(7),
                    Image = reader.IsDBNull(8) ? null : reader.GetString(8),
                    BrandId = reader.GetInt32(9),
                    CategoryId = reader.GetInt32(10),
                    UserId = reader.GetInt32(11),
                    BrandName = reader.GetString(12),
                    CategoryName = reader.GetString(13),
                    UserName = reader.IsDBNull(14) ? null : reader.GetString(14)
                });
            }

            return list;
        }


        public async Task<int?> AddProductAsync(Product product)
        {
            const string sql = @"
                INSERT INTO products (name, description, price, color, size, quantity, used, image, brand_id, category_id, user_id)
                VALUES (@name, @description, @price, @color, @size, @quantity, @used, @image, @brand, @category, @user)
                RETURNING id;
            ";

            await using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("name", product.Name);
            cmd.Parameters.AddWithValue("description", product.Description ?? "");
            cmd.Parameters.AddWithValue("price", product.Price);
            cmd.Parameters.AddWithValue("color", ColorHelper.GetColorValue(product.Color));
            cmd.Parameters.AddWithValue("size", product.Size);
            cmd.Parameters.AddWithValue("quantity", product.Quantity);
            cmd.Parameters.AddWithValue("used", product.Used);
            cmd.Parameters.AddWithValue("brand", product.BrandId);
            cmd.Parameters.AddWithValue("category", product.CategoryId);
            cmd.Parameters.AddWithValue("user", product.UserId);
            cmd.Parameters.AddWithValue("image", string.IsNullOrWhiteSpace(product.Image) ? DBNull.Value : product.Image);

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            object? result = await cmd.ExecuteScalarAsync();

            int? id = result is not null ? (int)result : null;

            return id;
        }

        public async Task<Product?> GetProductByIdAsync(int id)
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
                    p.image,
                    p.brand_id,
                    p.category_id,
                    p.user_id,
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

            return new Product
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.GetString(2),
                Price = reader.GetDecimal(3),
                Color = ColorHelper.GetColorByValue(reader.GetInt32(4)),
                Size = reader.GetString(5),
                Quantity = reader.GetInt32(6),
                Used = reader.GetBoolean(7),
                Image = reader.IsDBNull(8) ? null : reader.GetString(8),
                BrandId = reader.GetInt32(9),
                CategoryId = reader.GetInt32(10),
                UserId = reader.GetInt32(11),
                BrandName = reader.GetString(12),
                CategoryName = reader.GetString(13),
                UserName = reader.IsDBNull(14) ? null : reader.GetString(14)
            };
        }

        public async Task<bool> UpdateProductAsync(Product product)
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

            cmd.Parameters.AddWithValue("name", product.Name);
            cmd.Parameters.AddWithValue("description", product.Description ?? string.Empty);
            cmd.Parameters.AddWithValue("price", product.Price);
            cmd.Parameters.AddWithValue("color", ColorHelper.GetColorValue(product.Color));
            cmd.Parameters.AddWithValue("size", product.Size);
            cmd.Parameters.AddWithValue("quantity", product.Quantity);
            cmd.Parameters.AddWithValue("used", product.Used);
            cmd.Parameters.AddWithValue("brand", product.BrandId);
            cmd.Parameters.AddWithValue("category", product.CategoryId);
            cmd.Parameters.AddWithValue("id", product.Id);

            int rowsAffected = await cmd.ExecuteNonQueryAsync();

            return rowsAffected == 1;
        }

        #endregion
    }
}
