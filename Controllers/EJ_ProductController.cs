// Controllers/ProductController.cs
using System.Data.SqlClient;
using ECSTASYJEWELS.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECSTASYJEWELS.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly string _connectionString;


        public ProductController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }
        // Get All Product
        [HttpGet("{Category_ID}")]
        public async IAsyncEnumerable<EJ_Product> GetAllCategories(decimal Category_ID)
        {
            List<EJ_Product> categories = new List<EJ_Product>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    SqlCommand cmd = new SqlCommand(
                        "SELECT Product_ID, Product_Name, Product_Desc, Price, Rating, Price, Weight, Caret, Making_Charges, Other_Charges, (SELECT img.Image_Path FROM EJ_Product_Images img WHERE img.Product_ID = prod.Product_ID AND img.Is_Thumbnail = 1) as Product_Image FROM EJ_Product prod WHERE Is_Active = 1 and Category_ID="+Category_ID, conn);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            categories.Add(new EJ_Product
                            {
                                Product_ID = reader["Product_ID"] != DBNull.Value ? (decimal)reader["Product_ID"] : 0,
                                Product_Name = reader["Product_Name"] != DBNull.Value ? reader["Product_Name"].ToString() : string.Empty,
                                Product_Image = reader["Product_Image"] != DBNull.Value ? reader["Product_Image"].ToString() : string.Empty,
                                Product_Desc = reader["Product_Desc"] != DBNull.Value ? reader["Product_Desc"].ToString() : string.Empty,
                                Caret = reader["Caret"] != DBNull.Value ? reader["Caret"].ToString() : string.Empty,
                                Weight = reader["Weight"] != DBNull.Value ? reader["Weight"].ToString() : string.Empty,
                                Price = reader["Price"] != DBNull.Value ? (decimal)reader["Price"] : 0,
                                Making_Charges = reader["Making_Charges"] != DBNull.Value ? (decimal)reader["Making_Charges"] : 0,
                                Other_Charges = reader["Other_Charges"] != DBNull.Value ? (decimal)reader["Other_Charges"] : 0,
                                
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
                // Log general error
                Console.WriteLine($"Error: {ex.Message}");
                yield break;  // Stop the iteration in case of error
            }

            // Yield the categories after the try-catch block
            foreach (var Product in categories)
            {
                yield return Product;
            }
        }
        }

   
}
