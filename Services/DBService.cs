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

        public async Task<List<Product>> GetAllProductsAsync(int category = 0, int subCategory = 0)
        {
            string sql = @$"
                SELECT p.* FROM products_with_extra p
                WHERE 
                    (@category = 0 OR 
                    (@sub_category = 0 AND p.category_id IN (SELECT id FROM categories WHERE parent_category_id = @category)) OR
                    (@sub_category > 0 AND p.category_id = @sub_category))
                ORDER BY name;
            ";

            await using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("category", category);
            cmd.Parameters.AddWithValue("sub_category", subCategory);

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            List<ProductColor> colors = await GetProductColorsAsync();
            List<ProductSize> sizes = await GetProductSizesAsync();
            List<Review> reviews = await GetAllReviewsAsync();

            List<Product> list = new List<Product>();

            while (await reader.ReadAsync())
            {
                int productId = reader.GetInt32(0);

                list.Add(new Product
                {
                    Id = productId,
                    Name = reader.GetString(1),
                    Description = reader.GetString(2),
                    Price = reader.GetDecimal(3),
                    Colors = colors.Where(c => c.ProductId == productId).Select(c => c.Color).ToList(),
                    Sizes = sizes.Where(c => c.ProductId == productId).Select(c => c.Size).ToList(),
                    Image = reader.IsDBNull(4) ? null : reader.GetString(4),
                    Reviews = reviews.Where(r => r.ProductId == productId).ToList(),
                    BrandId = reader.GetInt32(5),
                    CategoryId = reader.GetInt32(6),
                    UserId = reader.GetInt32(7),
                    BrandName = reader.GetString(8),
                    CategoryName = reader.GetString(9),
                    UserName = reader.IsDBNull(10) ? null : reader.GetString(10)
                });
            }

            return list;
        }


        public async Task<int?> AddProductAsync(Product product)
        {
            const string sql = @"
                INSERT INTO products (name, description, price, image, brand_id, category_id, user_id)
                VALUES (@name, @description, @price, @image, @brand, @category, @user)
                RETURNING id;
            ";

            await using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("name", product.Name);
            cmd.Parameters.AddWithValue("description", product.Description);
            cmd.Parameters.AddWithValue("price", product.Price);
            cmd.Parameters.AddWithValue("brand", product.BrandId);
            cmd.Parameters.AddWithValue("category", product.CategoryId);
            cmd.Parameters.AddWithValue("user", product.UserId);
            cmd.Parameters.AddWithValue("image", string.IsNullOrWhiteSpace(product.Image) ? DBNull.Value : product.Image);

            object? result = await cmd.ExecuteScalarAsync();

            int? id = result is not null ? (int)result : null;

            if (id.HasValue)
            {
                await ReplaceProductColorsAsync(id.Value, product.Colors);
                await ReplaceProductSizesAsync(id.Value, product.Sizes);
            }

            return id;
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            await using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM products_with_extra WHERE id = @id;", conn);

            cmd.Parameters.AddWithValue("id", id);

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return null;
            }

            List<ProductColor> colors = await GetProductColorsAsync(id);
            List<ProductSize> sizes = await GetProductSizesAsync(id);
            List<Review> reviews = await GetAllReviewsAsync(id);

            return new Product
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.GetString(2),
                Price = reader.GetDecimal(3),
                Colors = colors.Select(c => c.Color).ToList(),
                Sizes = sizes.Select(s => s.Size).ToList(),
                Image = reader.IsDBNull(4) ? null : reader.GetString(4),
                Reviews = reviews,
                BrandId = reader.GetInt32(5),
                CategoryId = reader.GetInt32(6),
                UserId = reader.GetInt32(7),
                BrandName = reader.GetString(8),
                CategoryName = reader.GetString(9),
                UserName = reader.IsDBNull(10) ? null : reader.GetString(10)
            };
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            const string sql = @"
                UPDATE products SET 
                    name        = @name,
                    description = @description,
                    price       = @price,
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
            cmd.Parameters.AddWithValue("brand", product.BrandId);
            cmd.Parameters.AddWithValue("category", product.CategoryId);
            cmd.Parameters.AddWithValue("id", product.Id);

            int rowsAffected = await cmd.ExecuteNonQueryAsync();

            bool colorsReplaced = await ReplaceProductColorsAsync(product.Id, product.Colors);

            if (!colorsReplaced)
            {
                return false;
            }

            bool sizesReplaced = await ReplaceProductSizesAsync(product.Id, product.Sizes);

            if (!sizesReplaced)
            {
                return false;
            }

            return rowsAffected == 1;
        }

        #endregion

        #region Products - Colors

        private async Task<List<ProductColor>> GetProductColorsAsync(int productId = 0)
        {
            await using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM products_colors WHERE (@product_id = 0 OR product_id = @product_id)", conn);

            cmd.Parameters.AddWithValue("product_id", productId);

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            List<ProductColor> list = new List<ProductColor>();

            while (await reader.ReadAsync())
            {
                list.Add(new ProductColor()
                {
                    ProductId = reader.GetInt32(1),
                    Color = ColorHelper.GetColorByValue(reader.GetInt32(2))
                });
            }

            return list;
        }

        private async Task<bool> ReplaceProductColorsAsync(int productId, List<Colors> colors)
        {
            await using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            await using NpgsqlTransaction transaction = await conn.BeginTransactionAsync();

            try
            {
                using (NpgsqlCommand deleteCmd = new NpgsqlCommand("DELETE FROM products_colors WHERE product_id = @product_id", conn))
                {
                    deleteCmd.Parameters.AddWithValue("product_id", productId);

                    await deleteCmd.ExecuteNonQueryAsync();
                }

                int insertedCount = 0;

                foreach (Colors color in colors)
                {
                    using NpgsqlCommand insertCmd = new NpgsqlCommand("INSERT INTO products_colors (product_id, color) VALUES (@product_id, @color)", conn);

                    insertCmd.Parameters.AddWithValue("product_id", productId);
                    insertCmd.Parameters.AddWithValue("color", ColorHelper.GetColorValue(color));

                    insertedCount += await insertCmd.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();

                return insertedCount == colors.Count;
            }
            catch
            {
                await transaction.RollbackAsync();

                throw;
            }
        }

        #endregion

        #region Products - Sizes

        private async Task<List<ProductSize>> GetProductSizesAsync(int productId = 0)
        {
            await using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM products_sizes WHERE (@product_id = 0 OR product_id = @product_id)", conn);

            cmd.Parameters.AddWithValue("product_id", productId);

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            List<ProductSize> list = new List<ProductSize>();

            while (await reader.ReadAsync())
            {
                list.Add(new ProductSize()
                {
                    ProductId = reader.GetInt32(1),
                    Size = reader.GetString(2)
                });
            }

            return list;
        }

        private async Task<bool> ReplaceProductSizesAsync(int productId, List<string> sizes)
        {
            await using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            await using NpgsqlTransaction transaction = await conn.BeginTransactionAsync();

            try
            {
                using (NpgsqlCommand deleteCmd = new NpgsqlCommand("DELETE FROM products_sizes WHERE product_id = @product_id", conn))
                {
                    deleteCmd.Parameters.AddWithValue("product_id", productId);

                    await deleteCmd.ExecuteNonQueryAsync();
                }

                int insertedCount = 0;

                foreach (string size in sizes)
                {
                    using NpgsqlCommand insertCmd = new NpgsqlCommand("INSERT INTO products_sizes (product_id, size) VALUES (@product_id, @size)", conn);

                    insertCmd.Parameters.AddWithValue("product_id", productId);
                    insertCmd.Parameters.AddWithValue("size", size);

                    insertedCount += await insertCmd.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();

                return insertedCount == sizes.Count;
            }
            catch
            {
                await transaction.RollbackAsync();

                throw;
            }
        }

        #endregion

        #region Reviews

        public async Task<List<Review>> GetAllReviewsAsync(int productId = 0)
        {
            await using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM reviews_with_extra WHERE (@product_id = 0 OR product_id = @product_id) ORDER BY created_at DESC", conn);

            cmd.Parameters.AddWithValue("product_id", productId);

            using NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();

            List<Review> list = new List<Review>();

            while (await reader.ReadAsync())
            {
                list.Add(new Review
                {
                    Id = reader.GetInt32(0),
                    ProductId = reader.GetInt32(1),
                    UserId = reader.GetInt32(2),
                    Title = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    Message = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    Rating = reader.GetInt32(5),
                    CreatedAt = reader.GetDateTime(6),
                    UserName = reader.IsDBNull(7) ? string.Empty : reader.GetString(7)
                });
            }

            return list;
        }

        public async Task<int?> AddReviewAsync(Review review)
        {
            const string sql = @"
                INSERT INTO reviews (product_id, user_id, title, message, rating)
                VALUES (@product_id, @user_id, @title, @message, @rating)
                RETURNING id;
            ";

            await using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using NpgsqlCommand cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("product_id", review.ProductId);
            cmd.Parameters.AddWithValue("user_id", review.UserId);
            cmd.Parameters.AddWithValue("title", string.IsNullOrWhiteSpace(review.Title) ? DBNull.Value : review.Title);
            cmd.Parameters.AddWithValue("message", string.IsNullOrWhiteSpace(review.Message) ? DBNull.Value : review.Message);
            cmd.Parameters.AddWithValue("rating", review.Rating);

            object? result = await cmd.ExecuteScalarAsync();

            int? id = result is not null ? (int)result : null;

            return id;
        }

        public async Task<bool> DeleteReviewAsync(int id)
        {
            await using NpgsqlConnection conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            using NpgsqlCommand cmd = new NpgsqlCommand("DELETE FROM reviews WHERE id = @id;", conn);

            cmd.Parameters.AddWithValue("id", id);

            int rowsAffected = await cmd.ExecuteNonQueryAsync();

            return rowsAffected == 1;
        }

        #endregion
    }
}
