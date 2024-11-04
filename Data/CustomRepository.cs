using System.Data.SqlClient;
using ECSTASYJEWELS.Models;

namespace ECSTASYJEWELS
{
    public class CustomOrderRepository
    {
        private readonly string _connectionString;

        public CustomOrderRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> AddtoCustomOrder(Custom_Order toCustom)
        {
            int response = 0;  // This will store the newly inserted Custom_ID
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var query = @"INSERT INTO Custom_Order (Category_ID, User_ID, Purity, Remark, Mobile_Number, Email, Price, Weight, File_Path)
                          VALUES (@Category_ID, @User_ID, @Purity, @Remark, @Mobile_Number, @Email, @Price, @Weight, @File_Path);
                          SELECT SCOPE_IDENTITY();";  // To return the newly inserted Custom_ID

                    using (var command = new SqlCommand(query, conn))
                    {
                        // Add parameters to avoid SQL injection
                        command.Parameters.AddWithValue("@Category_ID", toCustom.Category_ID);
                        command.Parameters.AddWithValue("@User_ID", toCustom.User_ID);
                        command.Parameters.AddWithValue("@Purity", toCustom.Purity);
                        command.Parameters.AddWithValue("@Remark", toCustom.Remark);
                        command.Parameters.AddWithValue("@Mobile_Number", Convert.ToInt64(toCustom.Mobile_Number));
                        command.Parameters.AddWithValue("@Email", toCustom.Email);
                        command.Parameters.AddWithValue("@Price", Convert.ToDecimal(toCustom.Price));
                        command.Parameters.AddWithValue("@Weight", toCustom.Weight);
                        command.Parameters.AddWithValue("@File_Path", toCustom.File_Path);

                        // Execute the query and retrieve the new Custom_ID
                        var result = await command.ExecuteScalarAsync();

                        if (result != null)
                        {
                            response = Convert.ToInt32(result);  // Assign the new Custom_ID
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Database error occurred while adding the Custom." + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the Custom." + ex.Message);
            }

            return response;  // Return the new Custom_ID
        }

    }


}