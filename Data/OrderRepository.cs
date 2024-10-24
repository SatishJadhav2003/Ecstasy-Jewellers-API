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
            var OrderItems = new List<OrderInputOutput>();
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var command = new SqlCommand("SELECT Order_ID, Products.Product_ID,Products.Product_Name,Products.Description,Products.Price  " +
                    ",(select Image_URL from Product_Images where Product_Images.Product_ID=Products.Product_ID and Product_Images.Is_Primary=1 ) as Product_Image " +
                    "FROM Order inner join Products on Products.Product_ID =Order.Product_ID " +
                    "WHERE User_ID = @User_ID", conn);
                    command.Parameters.AddWithValue("@User_ID", User_ID);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            OrderItems.Add(new OrderInputOutput
                            {
                                Order_ID = (int)reader["Order_ID"],
                                Product_ID = (int)reader["Product_ID"],

                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log exception (consider using a logging framework)
                throw new Exception("Database error occurred while retrieving Order Items." + ex);
            }
            catch (Exception ex)
            {
                // Log exception
                throw new Exception("An error occurred while retrieving Order Items." + ex);
            }
            return OrderItems;
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


    }

    public class OrderInputOutput
    {
        public int Order_ID { get; set; }
        public int Address_ID { get; set; }
        public int User_ID { get; set; }
        public DateTime Order_Date { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Total_Amount { get; set; }
        public string Shipping_Address { get; set; } = string.Empty;
        public string Billing_Address { get; set; } = string.Empty;
        public string Payment_Status { get; set; } = string.Empty;
        public int Order_Item_ID { get; set; }
        public int Product_ID { get; set; }
        public int Quantity { get; set; }
        public decimal Unit_Price { get; set; }
        public decimal Total_Price => Quantity * Unit_Price;
    }
}