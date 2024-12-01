using System.Data.SqlClient;
using ECSTASYJEWELS.Models;

namespace ECSTASYJEWELS.Data
{
    public class CommonRepository
    {
        private readonly string _connectionString;
        public CommonRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        
    }
}