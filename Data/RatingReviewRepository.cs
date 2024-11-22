using System.Data.SqlClient;
using ECSTASYJEWELS.Models;

namespace ECSTASYJEWELS
{
    public class RatingReviewRepository
    {
        private readonly string _connectionString;

        public RatingReviewRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<bool> CanUserRateProduct(decimal User_ID, int Product_ID)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var query = @"
                SELECT 
                    CASE 
                        WHEN MAX(CASE WHEN os.Status = 'Delivered' THEN 1 ELSE 0 END) = 1 THEN 'True'
                        ELSE 'False'
                    END AS Is_Delivered
                FROM 
                    Orders ord
                INNER JOIN 
                    Order_Items oi ON oi.Order_ID = ord.Order_ID
                LEFT JOIN 
                    Order_Status os ON os.Order_ID = ord.Order_ID
                WHERE 
                    ord.User_ID = @User_ID 
                    AND oi.Product_ID = @Product_ID
                GROUP BY 
                    oi.Product_ID";

                    using (var command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@User_ID", User_ID);
                        command.Parameters.AddWithValue("@Product_ID", Product_ID);

                        var result = await command.ExecuteScalarAsync();
                        return result != null && result.ToString() == "True";
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log exception (consider using a logging framework)
                throw new Exception("Database error occurred while checking rating eligibility." + ex);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while checking rating eligibility." + ex);
            }
        }

        public async Task<int> AddRatingReview(Rating_Reviews reviewData)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var query = @"
                INSERT INTO Rating_Reviews (Product_ID, User_ID, Rating, Review_Text, Review_Date)
                OUTPUT INSERTED.Review_ID
                VALUES (@Product_ID, @User_ID, @Rating, @Review_Text, @Review_Date)";

                    using (var command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Product_ID", reviewData.Product_ID);
                        command.Parameters.AddWithValue("@User_ID", reviewData.User_ID);
                        command.Parameters.AddWithValue("@Rating", reviewData.Rating);
                        command.Parameters.AddWithValue("@Review_Text", reviewData.Review_Text);
                        command.Parameters.AddWithValue("@Review_Date", DateTime.Now);

                        var result = await command.ExecuteScalarAsync();

                        // Null check before casting
                        if (result == null)
                        {
                            throw new InvalidOperationException("Failed to insert review. The result is null.");
                        }

                        return (int)result;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error
                throw new Exception("Error saving review data.", ex);
            }
        }

        public async Task AddReviewImage(ReviewImage reviewImage)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var query = @"
                INSERT INTO ReviewImage (Review_ID, Product_ID, Image)
                VALUES (@Review_ID, @Product_ID, @Image)";

                    using (var command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Review_ID", reviewImage.Review_ID);
                        command.Parameters.AddWithValue("@Product_ID", reviewImage.Product_ID);
                        command.Parameters.AddWithValue("@Image", reviewImage.Image);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error
                throw new Exception("Error saving review image.", ex);
            }
        }

    }
}