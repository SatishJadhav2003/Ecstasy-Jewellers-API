using System.Data.SqlClient;
using Dapper;
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
                            WHEN MAX(CASE WHEN os.Status = 'Delivered' THEN 1 ELSE 0 END) = 1 THEN 
                                CASE 
                                    WHEN EXISTS (
                                        SELECT 1 
                                        FROM Rating_Reviews rr
                                        WHERE rr.User_ID = ord.User_ID
                                        AND rr.Product_ID = oi.Product_ID
                                    ) THEN 'False'
                                    ELSE 'True'
                                END
                            ELSE 'False'
                        END AS Can_Rate
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
                        oi.Product_ID,ord.User_ID;
                    ";

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

                    // Insert review query
                    var insertReviewQuery = @"
                INSERT INTO Rating_Reviews (Product_ID, User_ID, Rating, Review_Text, Review_Date)
                OUTPUT INSERTED.Review_ID
                VALUES (@Product_ID, @User_ID, @Rating, @Review_Text, @Review_Date)";

                    using (var command = new SqlCommand(insertReviewQuery, conn))
                    {
                        command.Parameters.AddWithValue("@Product_ID", reviewData.Product_ID);
                        command.Parameters.AddWithValue("@User_ID", reviewData.User_ID);
                        command.Parameters.AddWithValue("@Rating", reviewData.Rating);
                        command.Parameters.AddWithValue("@Review_Text", reviewData.Review_Text);
                        command.Parameters.AddWithValue("@Review_Date", DateTime.Now);

                        var result = await command.ExecuteScalarAsync();

                        // Check for null result
                        if (result == null)
                        {
                            throw new InvalidOperationException("Failed to insert review. The result is null.");
                        }

                        var reviewId = (int)result;

                        // Update product's rating, total ratings, and total reviews
                        var updateProductQuery = @"
                    UPDATE Products
                    SET 
                        Rating = (SELECT AVG(Rating) FROM Rating_Reviews WHERE Product_ID = @Product_ID),
                        Total_Ratings = (SELECT COUNT(*) FROM Rating_Reviews WHERE Product_ID = @Product_ID),
                        Total_Reviews = (SELECT COUNT(*) FROM Rating_Reviews WHERE Product_ID = @Product_ID)
                    WHERE Product_ID = @Product_ID";

                        using (var updateCommand = new SqlCommand(updateProductQuery, conn))
                        {
                            updateCommand.Parameters.AddWithValue("@Product_ID", reviewData.Product_ID);

                            await updateCommand.ExecuteNonQueryAsync();
                        }

                        return reviewId;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error
                throw new Exception("Error saving review data and updating product information.", ex);
            }
        }

        public async Task AddReviewImage(Review_Images reviewImage)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var query = @"
                INSERT INTO Review_Images (Review_ID, Product_ID, Image)
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

        public async Task<IEnumerable<Review_Images>> GetReviewImagesByProduct(int productId)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var query = @"
                SELECT RR_Image_ID, Review_ID, Product_ID, Image
                FROM Review_Images
                WHERE Product_ID = @Product_ID";

                    using (var command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Product_ID", productId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            var images = new List<Review_Images>();
                            while (await reader.ReadAsync())
                            {
                                images.Add(new Review_Images
                                {
                                    RR_Image_ID = reader.GetInt32(0),
                                    Review_ID = reader.GetInt32(1),
                                    Product_ID = reader.GetInt32(2),
                                    Image = reader.GetString(3)
                                });
                            }
                            return images;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error
                throw new Exception("Error retrieving images by Product ID.", ex);
            }
        }
        public async Task<Review_Images> GetReviewImageById(int imageId)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var query = @"
                SELECT RR_Image_ID, Review_ID, Product_ID, Image
                FROM Review_Images
                WHERE RR_Image_ID = @Image_ID";

                    using (var command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Image_ID", imageId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new Review_Images
                                {
                                    RR_Image_ID = reader.GetInt32(0),
                                    Review_ID = reader.GetInt32(1),
                                    Product_ID = reader.GetInt32(2),
                                    Image = reader.GetString(3)
                                };
                            }
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error
                throw new Exception("Error retrieving image by Image ID.", ex);
            }
        }

        public async Task<IEnumerable<ReviewWithImages>> GetReviewsAndRatingsByProduct(int productId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    
                    SELECT 
                        rr.Review_ID,
                        rr.Product_ID,
                        rr.User_ID,
                        rr.Rating,
                        rr.Review_Text,
                        rr.Review_Date,
                        ri.Image,
                        ri.RR_Image_ID,
                        us.First_Name AS First_Name,
                        us.Last_Name AS Last_Name
                    FROM 
                        Rating_Reviews rr
                    LEFT JOIN 
                        Review_Images ri ON rr.Review_ID = ri.Review_ID
                    LEFT JOIN 
                        Users us ON us.User_ID = rr.User_ID
                    WHERE 
                        rr.Product_ID = @ProductId";

                var reviewDict = new Dictionary<int, ReviewWithImages>();

                var result = await connection.QueryAsync<ReviewWithImages, string, ReviewWithImages>(
                    query,
                    (review, image) =>
                    {
                        if (!reviewDict.TryGetValue(review.Review_ID, out var reviewEntry))
                        {
                            reviewEntry = review;
                            reviewEntry.Images = new List<string>();
                            reviewDict.Add(review.Review_ID, reviewEntry);
                        }

                        if (!string.IsNullOrEmpty(image))
                        {
                            reviewEntry.Images.Add(image);
                        }

                        return reviewEntry;
                    },
                    new { ProductId = productId },
                    splitOn: "Image"
                );


                return reviewDict.Values;
            }
        }


    }

    public class ReviewWithImages
    {
        public int Review_ID { get; set; }
        public int Product_ID { get; set; }
        public int User_ID { get; set; }
        public int Rating { get; set; }
        public string Review_Text { get; set; } = string.Empty;
        public DateTime Review_Date { get; set; }

        public List<string> Images { get; set; } = new List<string>();
        public string First_Name { get; set; } = string.Empty;
        public string Last_Name { get; set; } = string.Empty;
    }
}