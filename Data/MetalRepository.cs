using System.Data.SqlClient;
using ECSTASYJEWELS.Models;

namespace ECSTASYJEWELS.Data
{
    public class MetalRepository
    {
        private readonly string _connectionString;
        public MetalRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Metal>> GetAllMetal()
        {
            var Metal = new List<Metal>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var command = new SqlCommand("SELECT Metal_ID,Metal_Name FROM Metal", conn);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Metal.Add(new Metal
                            {
                                Metal_ID = (int)reader["Metal_ID"],
                                Metal_Name = reader["Metal_Name"].ToString()??"",
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log exception (consider using a logging framework)
                throw new Exception("Database error occurred while retrieving Metal.", ex);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while retrieving Metal."+ ex);
            }
            return Metal;
        }

    }
}