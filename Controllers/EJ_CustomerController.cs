// Controllers/CustomerController.cs
using System.Data;
using System.Data.SqlClient;
using ECSTASYJEWELS.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECSTASYJEWELS.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly string _connectionString;


        public CustomerController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        // Get All Customer
        [HttpGet]
        public async IAsyncEnumerable<EJ_Customer> GetActiveCustomersAsync()
        {
            List<EJ_Customer> customers = new List<EJ_Customer>();

            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    SqlCommand cmd = new SqlCommand("SELECT Cust_ID, Cust_Name, Cust_Email FROM EJ_Customer WHERE Is_Active = 1", conn);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            customers.Add(new EJ_Customer
                            {
                                Cust_ID = reader["Cust_ID"] != DBNull.Value ? (decimal)reader["Cust_ID"] : 0,
                                Cust_Name = reader["Cust_Name"] != DBNull.Value ? reader["Cust_Name"].ToString() : string.Empty,
                                Cust_Email = reader["Cust_Email"] != DBNull.Value ? reader["Cust_Email"].ToString() : string.Empty,
                            });
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Log SQL-specific exception (use a logging framework in real-world scenarios)
                Console.WriteLine($"SQL Error: {sqlEx.Message}");
                yield break;  // Stop the iteration in case of error
            }
            catch (Exception ex)
            {
                // Log general exception
                Console.WriteLine($"Error: {ex.Message}");
                yield break;  // Stop the iteration in case of error
            }

            // Yield the customers after the try-catch block
            foreach (var customer in customers)
            {
                yield return customer;
            }
        }

        [HttpPost]
        public ActionResult<EJ_Customer> AddCustomer(EJ_Customer Customer)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    string sql = @"INSERT INTO EJ_Customer 
                                    (Cust_Name, Cust_Mobile, Cust_Email, Cust_Password, Is_Active, Is_Edited, Is_Deleted, Inserted_Date) 
                                    OUTPUT INSERTED.Cust_ID, INSERTED.Cust_Name, INSERTED.Cust_Mobile, INSERTED.Cust_Email, 
                                        INSERTED.Cust_Password
                                    VALUES (@Cust_Name, @Cust_Mobile, @Cust_Email, @Cust_Password, @Is_Active, @Is_Edited, @Is_Deleted, @Inserted_Date)";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        // Add parameters
                        cmd.Parameters.Add(new SqlParameter("@Cust_Name", SqlDbType.NVarChar, 100) { Value = Customer.Cust_Name });
                        cmd.Parameters.Add(new SqlParameter("@Cust_Mobile", SqlDbType.NVarChar, 15) { Value = Customer.Cust_Mobile });
                        cmd.Parameters.Add(new SqlParameter("@Cust_Email", SqlDbType.NVarChar, 100) { Value = Customer.Cust_Email });
                        cmd.Parameters.Add(new SqlParameter("@Cust_Password", SqlDbType.NVarChar, 100) { Value = Customer.Cust_Password });
                        cmd.Parameters.Add(new SqlParameter("@Inserted_Date", SqlDbType.DateTime) { Value = DateTime.Now });
                        cmd.Parameters.Add(new SqlParameter("@Is_Active", SqlDbType.Bit) { Value = true });
                        cmd.Parameters.Add(new SqlParameter("@Is_Edited", SqlDbType.Bit) { Value = false });
                        cmd.Parameters.Add(new SqlParameter("@Is_Deleted", SqlDbType.Bit) { Value = false });

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return Ok(new EJ_Customer
                                {
                                    Cust_ID = reader.GetDecimal(0),
                                    Cust_Name = reader.GetString(1),
                                    Cust_Mobile = reader.GetDecimal(2),  // Assuming Cust_Mobile is decimal in the database
                                    Cust_Email = reader.GetString(3),
                                    Cust_Password = reader.GetString(4),
                                });
                            }
                        }
                    }
                }

                return BadRequest("Customer could not be added."); // Fallback if insert fails for some reason
            }
            catch (SqlException sqlEx)
            {
                // Return a detailed error response with a status code (500 Internal Server Error)
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Database Error",
                    Detail = sqlEx.Message,
                    Status = StatusCodes.Status500InternalServerError
                });
            }
            catch (Exception ex)
            {
                // Return a detailed error response for a general exception
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "An error occurred",
                    Detail = ex.Message,
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }
    }
}
