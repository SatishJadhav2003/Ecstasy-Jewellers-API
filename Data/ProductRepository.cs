using System.Data.SqlClient;
using ECSTASYJEWELS.Models;

namespace ECSTASYJEWELS.Data
{
    public class ProductRepository
    {
        private readonly string _connectionString;

        public ProductRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<EJ_Product>> GetAllProductsByCategory(decimal Category_ID)
        {
            var products = new List<EJ_Product>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var command = new SqlCommand("SELECT Product_ID, Product_Name, Product_Desc, Price, Rating, Price, Weight, Caret, Making_Charges, Other_Charges, (SELECT img.Image_Path FROM EJ_Product_Images img WHERE img.Product_ID = prod.Product_ID AND img.Is_Thumbnail = 1) as Product_Image FROM EJ_Product prod WHERE Is_Active = 1 and Category_ID="+Category_ID, conn);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            products.Add(new EJ_Product
                            {
                                Product_ID = (decimal)reader["Product_ID"],
                                Product_Name = reader["Product_Name"].ToString()??"",
                                Product_Desc = reader["Product_Desc"].ToString()??"",
                                Product_Image = reader["Product_Image"].ToString()??"",
                                Price = (decimal)reader["Price"],
                                Making_Charges = (decimal)reader["Making_Charges"],
                                Other_Charges = (decimal)reader["Other_Charges"],
                                Caret = reader["Caret"].ToString() ?? "",
                                Weight = reader["Weight"].ToString() ?? "",
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log exception (consider using a logging framework)
                throw new Exception("Database error occurred while retrieving products.", ex);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while retrieving products.", ex);
            }

            return products;
        }
    }
}

