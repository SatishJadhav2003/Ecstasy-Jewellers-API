using ECSTASYJEWELS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace ECSTASYJEWELS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BannerController : ControllerBase
    {
        private readonly string _connectionString;


        public BannerController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        // Create Banner
        [HttpPost]
        public OperationResult AddBanner(EJ_Banner banner)
        {
            var result = new OperationResult();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(
                        "INSERT INTO EJ_Banner (Banner_Name, Banner_Image, Is_Active, Is_Edited, Is_Deleted, Inserted_Date) " +
                        "VALUES (@Banner_Name, @Banner_Image, @Is_Active, 0, 0, @Inserted_Date)", conn);

                    // Add parameters
                    cmd.Parameters.AddWithValue("@Banner_Name", banner.Banner_Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Banner_Image", banner.Banner_Image ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Is_Active", banner.Is_Active);
                    cmd.Parameters.AddWithValue("@Inserted_Date", DateTime.Now);

                    cmd.ExecuteNonQuery();

                    result.Success = true;
                    result.Message = "Banner added successfully.";
                }
            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"SQL Error: {sqlEx.Message}");
                result.Success = false;
                result.Message = "SQL error occurred while adding the banner.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
                result.Success = false;
                result.Message = "An unexpected error occurred.";
            }

            return result;
        }



        // Get All Banners
        [HttpGet]
        public async IAsyncEnumerable<EJ_Banner> GetAllBanners()
        {
            List<EJ_Banner> banners = new List<EJ_Banner>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    SqlCommand cmd = new SqlCommand(
                        "SELECT Banner_ID, Category_ID, Banner_Name, Banner_Image FROM EJ_Banner WHERE Is_Active = 1",
                        conn);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            banners.Add(new EJ_Banner
                            {
                                Banner_ID = reader["Banner_ID"] != DBNull.Value ? (decimal)reader["Banner_ID"] : 0,
                                Category_ID = reader["Category_ID"] != DBNull.Value ? (decimal)reader["Category_ID"] : 0,
                                Banner_Name = reader["Banner_Name"] != DBNull.Value ? reader["Banner_Name"].ToString() : string.Empty,
                                Banner_Image = reader["Banner_Image"] != DBNull.Value ? reader["Banner_Image"].ToString() : string.Empty,
                            });
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Log SQL-specific error
                Console.WriteLine($"SQL Error: {sqlEx.Message}");
                yield break;  // Stop the iteration in case of error
            }
            catch (Exception ex)
            {
                // Log general exception
                Console.WriteLine($"Error: {ex.Message}");
                yield break;  // Stop the iteration in case of error
            }

            // Yield the banners after the try-catch block
            foreach (var banner in banners)
            {
                yield return banner;
            }
        }

    }
}
