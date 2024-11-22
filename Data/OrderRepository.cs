using System.Data.SqlClient;
using ECSTASYJEWELS.Models;

namespace ECSTASYJEWELS
{
    public class OrderRepository
    {
        private readonly string _connectionString;

        public OrderRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> AddOrderAsync(Order order, List<Order_Items> orderItems)
        {
            int newOrderId = 0;

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    using (var transaction = conn.BeginTransaction())  // Use transaction to ensure both inserts succeed
                    {
                        try
                        {
                            // Insert into Order table
                            var insertOrderQuery = @"
                    INSERT INTO Orders (Address_ID, User_ID, Order_Date, Status, Total_Amount, Shipping_Address, Billing_Address, Payment_Status)
                    VALUES (@Address_ID, @User_ID, @Order_Date, @Status, @Total_Amount, @Shipping_Address, @Billing_Address, @Payment_Status);
                    SELECT SCOPE_IDENTITY();";  // Returns the newly inserted Order_ID

                            using (var command = new SqlCommand(insertOrderQuery, conn, transaction))
                            {
                                // Set parameters for Order insertion
                                command.Parameters.AddWithValue("@Address_ID", order.Address_ID);
                                command.Parameters.AddWithValue("@User_ID", order.User_ID);
                                command.Parameters.AddWithValue("@Order_Date", order.Order_Date);
                                command.Parameters.AddWithValue("@Status", order.Status);
                                command.Parameters.AddWithValue("@Total_Amount", order.Total_Amount);
                                command.Parameters.AddWithValue("@Shipping_Address", order.Shipping_Address);
                                command.Parameters.AddWithValue("@Billing_Address", order.Billing_Address);
                                command.Parameters.AddWithValue("@Payment_Status", order.Payment_Status);

                                // Execute and get new Order_ID
                                var result = await command.ExecuteScalarAsync();
                                if (result != null)
                                {
                                    newOrderId = Convert.ToInt32(result);
                                }
                            }

                            // Insert into Order_Items table for each order item
                            var insertOrderItemsQuery = @"
                    INSERT INTO Order_Items (Order_ID, Product_ID, Quantity, Unit_Price)
                    VALUES (@Order_ID, @Product_ID, @Quantity, @Unit_Price);";

                            foreach (var item in orderItems)
                            {
                                using (var command = new SqlCommand(insertOrderItemsQuery, conn, transaction))
                                {
                                    // Set parameters for each Order_Item
                                    command.Parameters.AddWithValue("@Order_ID", newOrderId);  // Reference the new Order_ID
                                    command.Parameters.AddWithValue("@Product_ID", item.Product_ID);
                                    command.Parameters.AddWithValue("@Quantity", item.Quantity);
                                    command.Parameters.AddWithValue("@Unit_Price", item.Unit_Price);

                                    // Execute the query for each order item
                                    await command.ExecuteNonQueryAsync();
                                }
                            }

                            // Insert initial status (Placed) into Order_Status table
                            var insertOrderStatusQuery = @"
                    INSERT INTO Order_Status (Order_ID, Status, Status_Timestamp, Location)
                    VALUES (@Order_ID, @Status, @Status_Timestamp, @Location);";

                            using (var command = new SqlCommand(insertOrderStatusQuery, conn, transaction))
                            {
                                // Set parameters for Order_Status insertion
                                command.Parameters.AddWithValue("@Order_ID", newOrderId);  // Reference the new Order_ID
                                command.Parameters.AddWithValue("@Status", "Placed");
                                command.Parameters.AddWithValue("@Status_Timestamp", DateTime.Now);  // Current timestamp
                                command.Parameters.AddWithValue("@Location", "");  // Optional: Leave empty if no location is available

                                // Execute the query for order status
                                await command.ExecuteNonQueryAsync();
                            }

                            // Delete products from Cart after placing the order
                            var deleteCartQuery = @"DELETE FROM Cart WHERE User_ID = @User_ID AND Product_ID IN (@Product_ID);";

                            foreach (var item in orderItems)
                            {
                                using (var command = new SqlCommand(deleteCartQuery, conn, transaction))
                                {
                                    // Remove each item from the user's cart
                                    command.Parameters.AddWithValue("@User_ID", order.User_ID);
                                    command.Parameters.AddWithValue("@Product_ID", item.Product_ID);

                                    // Execute the query to delete the cart item
                                    await command.ExecuteNonQueryAsync();
                                }
                            }

                            // Commit transaction if all operations succeed
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            // Rollback transaction if an error occurs
                            transaction.Rollback();
                            throw new Exception("An error occurred while adding the Order and Order Items." + ex.Message);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log exception
                throw new Exception("Database error occurred while adding the Order." + ex.Message);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while adding the Order." + ex.Message);
            }

            return newOrderId;  // Return the new Order_ID
        }

        public async Task<IEnumerable<OrderInputOutput>> GetUserOrderItems(decimal User_ID)
        {
            var orderItems = new List<OrderInputOutput>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    // Updated SQL query
                    var query = @"
                SELECT
                    ord.Order_ID,
                    ord.Payment_Status,
                    ord.Total_Amount,
                    oi.Quantity,
                    oi.Product_ID,
                    prod.Product_Name,
                    prod.[Description],
                    oi.Unit_Price,
                    oi.Order_Item_ID,
                    pi.Image_URL AS Product_Image,
                    addr.City AS City_Name,
                    
                    CASE 
                        WHEN MAX(CASE WHEN os.Status = 'Delivered' THEN 1 ELSE 0 END) = 1 THEN 'True'
                        ELSE 'False'
                    END AS Is_Delivered,
                    
                    MAX(CASE WHEN os.Status = 'Delivered' THEN os.Status_Timestamp END) AS Delivery_Date,
                    MAX(CASE WHEN os.Status = 'Placed' THEN os.Status_Timestamp END) AS Order_Placed_Date

                FROM
                    Orders ord
                INNER JOIN 
                    Order_Items oi ON oi.Order_ID = ord.Order_ID
                INNER JOIN 
                    Products prod ON prod.Product_ID = oi.Product_ID
                INNER JOIN 
                    Product_Images pi ON pi.Product_ID = oi.Product_ID AND pi.Is_Primary = 1
                LEFT JOIN 
                    Order_Status os ON os.Order_ID = ord.Order_ID
                INNER JOIN 
                    Addresses addr ON addr.Address_ID = ord.Address_ID
                
                WHERE
                    ord.User_ID = @User_ID
                
                GROUP BY
                    ord.Order_ID, 
                    ord.Payment_Status,
                    ord.Total_Amount,
                    oi.Quantity,
                    oi.Product_ID,
                    prod.Product_Name,
                    prod.[Description],
                    oi.Unit_Price,
                    pi.Image_URL,
                    addr.City,
                    oi.Order_Item_ID";

                    using (var command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@User_ID", User_ID);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                orderItems.Add(new OrderInputOutput
                                {
                                    Order_ID = (int)reader["Order_ID"],
                                    Order_Item_ID = (int)reader["Order_Item_ID"],
                                    Product_ID = (int)reader["Product_ID"],
                                    Quantity = (int)reader["Quantity"],
                                    Payment_Status = reader["Payment_Status"].ToString() ?? "",
                                    Product_Name = reader["Product_Name"].ToString() ?? "",
                                    Description = reader["Description"].ToString() ?? "",
                                    Total_Amount = (decimal)reader["Total_Amount"],
                                    Unit_Price = (decimal)reader["Unit_Price"],
                                    Product_Image = reader["Product_Image"].ToString() ?? "",
                                    City_Name = reader["City_Name"].ToString() ?? "",

                                    // Is_Delivered and Date fields
                                    Is_Delivered = reader["Is_Delivered"].ToString() == "True",
                                    Delivery_Date = reader["Delivery_Date"] != DBNull.Value ? (DateTime?)reader["Delivery_Date"] : null,
                                    Order_Placed_Date = reader["Order_Placed_Date"] != DBNull.Value ? (DateTime?)reader["Order_Placed_Date"] : null,
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log exception (consider using a logging framework)
                throw new Exception("Database error occurred while retrieving order items." + ex);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while retrieving order items." + ex);
            }

            return orderItems;
        }
        public async Task<bool> RemoveFromOrder(int Order_ID)
        {
            bool isDeleted = false;  // This will store the result of the deletion (true if successful)

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var query = @"DELETE FROM Order WHERE Order_ID = @Order_ID";  // Query to delete the Order item by Order_ID

                    using (var command = new SqlCommand(query, conn))
                    {
                        // Add parameter to avoid SQL injection
                        command.Parameters.AddWithValue("@Order_ID", Order_ID);

                        // Execute the delete query
                        var result = await command.ExecuteNonQueryAsync();

                        // If at least one row is affected, consider the delete successful
                        isDeleted = result > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log exception (consider using a logging framework)
                throw new Exception("Database error occurred while deleting the Order item." + ex.Message);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while deleting the Order item." + ex.Message);
            }

            return isDeleted;  // Return whether the deletion was successful
        }


        public async Task<IEnumerable<Order_Description>> GetOrderDescription(int Order_Item_ID)
        {
            var orderDescriptions = new List<Order_Description>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    // New SQL query to get order description details
                    var query = @"
                SELECT 
                    pi.Image_URL AS Product_Image,
                    os.[Status],
                    os.Status_Timestamp,
                    os.[Location],
                    prod.Product_Name,
                    prod.[Description],
                    ord.Total_Amount,
                    ord.Order_ID,
                    oi.Unit_Price,
                    oi.Quantity
                FROM 
                    Orders ord
                INNER JOIN 
                    Order_Items oi ON oi.Order_ID = ord.Order_ID
                INNER JOIN 
                    Order_Status os ON os.Order_ID = ord.Order_ID
                INNER JOIN 
                    Products prod ON prod.Product_ID = oi.Product_ID
                INNER JOIN 
                    Product_Images pi ON pi.Product_ID = oi.Product_ID
                WHERE 
                    pi.Is_Primary = 1 AND oi.Order_Item_ID = @Order_Item_ID
                ORDER BY 
                    os.Status_Timestamp asc";

                    using (var command = new SqlCommand(query, conn))
                    {
                        command.Parameters.AddWithValue("@Order_Item_ID", Order_Item_ID);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                orderDescriptions.Add(new Order_Description
                                {
                                    Order_ID = (int)reader["Order_ID"],
                                    Order_Item_ID = Order_Item_ID,
                                    Product_Image = reader["Product_Image"].ToString() ?? "",
                                    Status = reader["Status"].ToString() ?? "",
                                    Status_Timestamp = (DateTime)reader["Status_Timestamp"],
                                    Location = reader["Location"].ToString() ?? "",
                                    Product_Name = reader["Product_Name"].ToString() ?? "",
                                    Description = reader["Description"].ToString() ?? "",
                                    Total_Amount = (decimal)reader["Total_Amount"],
                                    Unit_Price = (decimal)reader["Unit_Price"],
                                    Quantity = (int)reader["Quantity"]
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log exception (consider using a logging framework)
                throw new Exception("Database error occurred while retrieving order descriptions." + ex);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while retrieving order descriptions." + ex);
            }

            return orderDescriptions;
        }


    }


    public class OrderInputOutput
    {
        public int Order_ID { get; set; }
        public int Order_Item_ID {get;set;}
        public int Product_ID { get; set; }
        public int Quantity { get; set; }
        public string Payment_Status { get; set; } = "";
        public string Product_Name { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Total_Amount { get; set; }
        public decimal Unit_Price { get; set; }
        public string Product_Image { get; set; } = "";
        public string City_Name { get; set; } = ""; // New field for City

        public bool Is_Delivered { get; set; }  // New field for delivery status
        public DateTime? Delivery_Date { get; set; }  // New field for Delivery date
        public DateTime? Order_Placed_Date { get; set; }  // New field for Order placed date
    }

    public class Order_Description
    {
        public int Order_ID { get; set; }
        public int Order_Item_ID { get; set; }
        public string Product_Image { get; set; } = "";
        public string Status { get; set; } = "";
        public DateTime Status_Timestamp { get; set; }
        public string Location { get; set; } = "";
        public string Product_Name { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Total_Amount { get; set; }
        public decimal Unit_Price { get; set; }
        public int Quantity { get; set; }
    }


}