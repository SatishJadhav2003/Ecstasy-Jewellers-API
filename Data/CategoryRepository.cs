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

        public async Task<IEnumerable<EJ_Category>> GetAllActiveCategory()
        {
            var category = new List<EJ_Category>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var command = new SqlCommand("SELECT Category_ID, Category_Name, Category_Image FROM EJ_Category WHERE Is_Active = 1", conn);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            category.Add(new EJ_Category
                            {
                                Category_ID = (decimal)reader["Category_ID"],
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
    }
}