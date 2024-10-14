using System.Data.SqlClient;
using ECSTASYJEWELS.Models;

namespace ECSTASYJEWELS.Data
{
    public class BannerRepository
    {
        private readonly string _connectionString;
        public BannerRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<EJ_Banner>> GetAllActiveBanners()
        {
            var banners = new List<EJ_Banner>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var command = new SqlCommand("SELECT Banner_ID, Category_ID, Banner_Name,Banner_Action_Name, Banner_Image FROM EJ_Banner WHERE Is_Active = 1", conn);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            banners.Add(new EJ_Banner
                            {
                                Banner_ID = (decimal)reader["Banner_ID"],
                                Category_ID = (decimal)reader["Category_ID"],
                                Banner_Name = reader["Banner_Name"].ToString() ?? "",
                                Banner_Image = reader["Banner_Image"].ToString() ?? "",
                                Banner_Action_Name = reader["Banner_Action_Name"].ToString() ?? "",
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log exception (consider using a logging framework)
                throw new Exception("Database error occurred while retrieving Banners.", ex);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while retrieving Banners."+ ex);
            }
            return banners;
        }

    }
}