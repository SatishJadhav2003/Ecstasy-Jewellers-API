using System.Data.SqlClient;
using ECSTASYJEWELS.Controllers;
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

        public async Task<IEnumerable<Product>> GetAllProductsByCategory(decimal Category_ID)
        {
            var products = new List<Product>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var command = new SqlCommand(
                        "SELECT Product_ID, Product_Name, Description, Price, Weight, Dimensions, Stock_Quantity, Rating, Total_Ratings, Total_Reviews, " +
                        "(SELECT img.Image_URL FROM Product_Images img WHERE img.Product_ID = prod.Product_ID AND img.Is_Primary = 1) as Product_Image " +
                        "FROM Products prod WHERE Is_Active = 1 and Category_ID = @Category_ID", conn);

                    // Use parameterized query to avoid SQL injection
                    command.Parameters.AddWithValue("@Category_ID", Category_ID);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            products.Add(new Product
                            {
                                Product_ID = reader["Product_ID"] == DBNull.Value ? 0 : (int)reader["Product_ID"],
                                Product_Name = reader["Product_Name"]?.ToString() ?? "",
                                Description = reader["Description"]?.ToString() ?? "",
                                Product_Image = reader["Product_Image"]?.ToString() ?? "",
                                Price = reader["Price"] == DBNull.Value ? 0.0m : (decimal)reader["Price"],
                                Weight = reader["Weight"] == DBNull.Value ? 0.0m : (decimal)reader["Weight"],
                                Stock_Quantity = reader["Stock_Quantity"] == DBNull.Value ? 0 : (int)reader["Stock_Quantity"],
                                Rating = reader["Rating"] == DBNull.Value ? 0.0m : (decimal)reader["Rating"],
                                Total_Ratings = reader["Total_Ratings"] == DBNull.Value ? 0 : (int)reader["Total_Ratings"],
                                Total_Reviews = reader["Total_Reviews"] == DBNull.Value ? 0 : (int)reader["Total_Reviews"]
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

        public async Task<ProductData[]> GetProductById(int Product_ID)
        {
            var product = new List<ProductData>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var command = new SqlCommand(
                        "SELECT PROD.Product_ID,PROD.Rating,PROD.Total_Ratings,PROD.Total_Reviews, PROD.Category_ID, PROD.Product_Name, PROD.Description, PROD.Price, PROD.Weight, PROD.Stock_Quantity, " +
                        "DIM.Dimension_ID, DIM.Title, DIM.Dim_Desc, " +
                        "(SELECT img.Image_URL FROM Product_Images img WHERE img.Product_ID = PROD.Product_ID AND img.Is_Primary = 1) as Product_Image " +
                        "FROM Products AS PROD " +
                        "LEFT JOIN Dimensions as DIM ON DIM.Product_ID = PROD.Product_ID " +
                        "WHERE PROD.Is_Active = 1 AND PROD.Product_ID = @Product_ID", conn);

                    command.Parameters.AddWithValue("@Product_ID", Product_ID);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            product.Add(new ProductData
                            {
                                Product_ID = (int)reader["Product_ID"],
                                Category_ID = (int)reader["Category_ID"],
                                Dimension_ID = reader["Dimension_ID"] != DBNull.Value ? (int)reader["Dimension_ID"] : 0,  // Handle nullable values
                                Product_Name = reader["Product_Name"].ToString() ?? "",
                                Product_Image = reader["Product_Image"].ToString() ?? "",
                                Description = reader["Description"].ToString() ?? "",
                                Price = (decimal)reader["Price"],
                                Weight = (decimal)reader["Weight"],
                                Title = reader["Title"].ToString() ?? "",
                                Dim_Desc = reader["Dim_Desc"].ToString() ?? "",
                                Stock_Quantity = (int)reader["Stock_Quantity"],
                                Rating = reader["Rating"] == DBNull.Value ? 0.0m : (decimal)reader["Rating"],
                                Total_Ratings = reader["Total_Ratings"] == DBNull.Value ? 0 : (int)reader["Total_Ratings"],
                                Total_Reviews = reader["Total_Reviews"] == DBNull.Value ? 0 : (int)reader["Total_Reviews"]
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log exception (consider using a logging framework)
                throw new Exception("Database error occurred while retrieving the product." + ex);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while retrieving the product." + ex);
            }

            return product.ToArray();  // Convert the list to an array before returning
        }

        public async Task<IEnumerable<Product>> GeatSearchProducts(string query)
        {
            var products = new List<Product>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var command = new SqlCommand(
                        "SELECT TOP 20 Product_ID, Product_Name, Description, Price, Weight, Dimensions, Stock_Quantity, Rating, Total_Ratings, Total_Reviews, " +
                        "(SELECT img.Image_URL FROM Product_Images img WHERE img.Product_ID = prod.Product_ID AND img.Is_Primary = 1) as Product_Image " +
                        "FROM Products prod WHERE Is_Active = 1 and Product_Name like @Query or Description like @Query order by Rating Desc", conn);

                    // Use parameterized query to avoid SQL injection
                    command.Parameters.AddWithValue("@Query", $"%{query}%");

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            products.Add(new Product
                            {
                                Product_ID = reader["Product_ID"] == DBNull.Value ? 0 : (int)reader["Product_ID"],
                                Product_Name = reader["Product_Name"]?.ToString() ?? "",
                                Description = reader["Description"]?.ToString() ?? "",
                                Product_Image = reader["Product_Image"]?.ToString() ?? "",
                                Price = reader["Price"] == DBNull.Value ? 0.0m : (decimal)reader["Price"],
                                Weight = reader["Weight"] == DBNull.Value ? 0.0m : (decimal)reader["Weight"],
                                Stock_Quantity = reader["Stock_Quantity"] == DBNull.Value ? 0 : (int)reader["Stock_Quantity"],
                                Rating = reader["Rating"] == DBNull.Value ? 0.0m : (decimal)reader["Rating"],
                                Total_Ratings = reader["Total_Ratings"] == DBNull.Value ? 0 : (int)reader["Total_Ratings"],
                                Total_Reviews = reader["Total_Reviews"] == DBNull.Value ? 0 : (int)reader["Total_Reviews"]
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


        public async Task<IEnumerable<Product>> GetFilteredProducts(FilterData data)
        {
            var products = new List<Product>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    // Base query
                    var query = @"
                SELECT TOP 20 
                    Product_ID, Product_Name, Description, Price, Weight, Stock_Quantity, Rating, Total_Ratings, Total_Reviews,
                    (SELECT img.Image_URL 
                     FROM Product_Images img 
                     WHERE img.Product_ID = prod.Product_ID AND img.Is_Primary = 1) AS Product_Image
                FROM Products prod
                WHERE Is_Active = 1 ";

                    // Initialize parameters
                    var parameters = new List<SqlParameter>();

                    // Handle Category filter
                    if (data.Category.Any())
                    {
                        var categoryConditions = new List<string>();
                        int categoryIndex = 0;

                        foreach (var category in data.Category)
                        {
                            var paramName = $"@Category{categoryIndex}";
                            categoryConditions.Add(paramName);
                            parameters.Add(new SqlParameter(paramName, category));
                            categoryIndex++;
                        }

                        query += $"AND prod.Category_ID IN ({string.Join(",", categoryConditions)}) ";
                    }

                    // Handle Metal filter
                    if (data.Metal.Any())
                    {
                        var metalConditions = new List<string>();
                        int metalIndex = 0;

                        foreach (var metal in data.Metal)
                        {
                            var paramName = $"@Metal{metalIndex}";
                            metalConditions.Add(paramName);
                            parameters.Add(new SqlParameter(paramName, metal));
                            metalIndex++;
                        }

                        query += $"AND prod.Metal_ID IN ({string.Join(",", metalConditions)}) ";
                    }


                    // Handle Price filter
                    if (data.Price.Any())
                    {
                        var priceConditions = new List<string>();
                        int priceIndex = 0;

                        var priceRanges = new Dictionary<int, (int Min, int Max)>
                            {
                                { 0, (0, 25000) },
                                { 25000, (25000, 50000) },
                                { 50000, (50000, 100000) },
                                { 100000, (100000, int.MaxValue) }
                            };

                        foreach (var price in data.Price)
                        {
                            if (priceRanges.TryGetValue(price, out var range))
                            {
                                var minParam = $"@PriceMin{priceIndex}";
                                var maxParam = $"@PriceMax{priceIndex}";
                                priceConditions.Add($"Price BETWEEN {minParam} AND {maxParam}");
                                parameters.Add(new SqlParameter(minParam, range.Min));
                                parameters.Add(new SqlParameter(maxParam, range.Max));
                                priceIndex++;
                            }
                        }

                        // Combine conditions with OR
                        if (priceConditions.Any())
                        {
                            query += $"AND ({string.Join(" OR ", priceConditions)}) ";
                        }
                    }

                    // Handle Gender filter
                    if (data.Gender.Any())
                    {
                        var genderConditions = new List<string>();
                        int genderIndex = 0;

                        foreach (var gender in data.Gender)
                        {
                            var paramName = $"@Gender{genderIndex}";
                            genderConditions.Add($"prod.Description LIKE {paramName}");
                            parameters.Add(new SqlParameter(paramName, $"%{gender}%"));
                            genderIndex++;
                        }

                        // Combine conditions with OR
                        query += $"AND ({string.Join(" OR ", genderConditions)}) ";
                    }


                    // Order by Rating
                    query += "ORDER BY Rating DESC;";
                    

                    // Prepare and execute the command
                    var command = new SqlCommand(query, conn);
                    command.Parameters.AddRange(parameters.ToArray());

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            products.Add(new Product
                            {
                                Product_ID = reader["Product_ID"] as int? ?? 0,
                                Product_Name = reader["Product_Name"]?.ToString() ?? "",
                                Description = reader["Description"]?.ToString() ?? "",
                                Product_Image = reader["Product_Image"]?.ToString() ?? "",
                                Price = reader["Price"] as decimal? ?? 0,
                                Weight = reader["Weight"] as decimal? ?? 0,
                                Stock_Quantity = reader["Stock_Quantity"] as int? ?? 0,
                                Rating = reader["Rating"] as decimal? ?? 0,
                                Total_Ratings = reader["Total_Ratings"] as int? ?? 0,
                                Total_Reviews = reader["Total_Reviews"] as int? ?? 0
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("Database error occurred while retrieving products.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving products.", ex);
            }

            return products;
        }

    }

    public class ProductData
    {
        public int Product_ID { get; set; }
        public string Product_Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Category_ID { get; set; }
        public decimal Price { get; set; }
        public int Stock_Quantity { get; set; }
        public int Total_Ratings { get; set; }
        public int Total_Reviews { get; set; }
        public decimal Rating { get; set; }
        public decimal Weight { get; set; }
        public string Product_Image { get; set; } = string.Empty;

        public bool Is_Active { get; set; }
        public DateTime Date_Added { get; set; }
        public DateTime? Updated_Date { get; set; }
        public int Dimension_ID { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Dim_Desc { get; set; } = string.Empty;
    }

}

