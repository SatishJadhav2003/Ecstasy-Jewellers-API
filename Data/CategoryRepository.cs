using System.Data.SqlClient;
using ECSTASYJEWELS.Models;

namespace ECSTASYJEWELS
{
    public class CategoryRepository
    {
        private readonly string _connectionString;

        public CategoryRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Category>> GetAllActiveCategory()
        {
            var category = new List<Category>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var command = new SqlCommand("SELECT Category_ID, Category_Name, Category_Image FROM Category WHERE Is_Active = 1", conn);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            category.Add(new Category
                            {
                                Category_ID = (int)reader["Category_ID"],
                                Category_Name = reader["Category_Name"].ToString()??"",
                                Category_Image = reader["Category_Image"].ToString() ??""
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log exception (consider using a logging framework)
                throw new Exception("Database error occurred while retrieving Category.", ex);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while retrieving Category.", ex);
            }
            return category;
        }
        
        public async Task<IEnumerable<Category>> GetCategoryByID(int Category_ID)
        {
            var category = new List<Category>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var command = new SqlCommand("SELECT Category_ID, Category_Name, Category_Image FROM Category WHERE Category_ID = "+Category_ID, conn);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            category.Add(new Category
                            {
                                Category_ID = (int)reader["Category_ID"],
                                Category_Name = reader["Category_Name"].ToString()??"",
                                Category_Image = reader["Category_Image"].ToString() ??""
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log exception (consider using a logging framework)
                throw new Exception("Database error occurred while retrieving Category.", ex);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while retrieving Category.", ex);
            }
            return category;
        }
       
        
        public async Task<bool> AddCategory(Category category)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var command = new SqlCommand("INSERT INTO Category (Category_Name, Category_Image, Is_Active) VALUES (@Name, @Image, @IsActive)", conn);
                    command.Parameters.AddWithValue("@Name", category.Category_Name);
                    command.Parameters.AddWithValue("@Image", category.Category_Image);
                    command.Parameters.AddWithValue("@IsActive", true); // Assuming new categories are active by default

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0; // Returns true if a row was inserted
                }
            }
            catch (SqlException ex)
            {
                // Log exception (consider using a logging framework)
                throw new Exception("Database error occurred while adding Category.", ex);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while adding Category.", ex);
            }
        }
    }
}