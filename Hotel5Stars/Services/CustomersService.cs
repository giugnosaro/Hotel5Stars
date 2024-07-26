using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Hotel5Stars.Models.Dto;

namespace Hotel5Stars.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly string _connectionString;

        public CustomerService(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            var customers = new List<Customer>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = "SELECT * FROM Customers";
                using (var cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            customers.Add(new Customer
                            {
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                FiscalCode = reader.IsDBNull(reader.GetOrdinal("FiscalCode")) ? null : reader.GetString(reader.GetOrdinal("FiscalCode")),
                                LastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString(reader.GetOrdinal("LastName")),
                                FirstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? null : reader.GetString(reader.GetOrdinal("FirstName")),
                                City = reader.IsDBNull(reader.GetOrdinal("BirthCity")) ? null : reader.GetString(reader.GetOrdinal("BirthCity")),
                                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                                Phone = reader.IsDBNull(reader.GetOrdinal("HomePhone")) ? null : reader.GetString(reader.GetOrdinal("HomePhone")),
                                Mobile = reader.IsDBNull(reader.GetOrdinal("MobilePhone")) ? null : reader.GetString(reader.GetOrdinal("MobilePhone"))
                            });
                        }
                    }
                }
            }
            return customers;
        }

        public async Task<bool> AddCustomerAsync(Customer customer)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = @"
            INSERT INTO Customers (FiscalCode, LastName, FirstName, BirthCity, Email, HomePhone, MobilePhone)
            VALUES (@FiscalCode, @LastName, @FirstName, @BirthCity, @Email, @HomePhone, @MobilePhone)";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FiscalCode", (object)customer.FiscalCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastName", (object)customer.LastName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@FirstName", (object)customer.FirstName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@BirthCity", (object)customer.City ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", (object)customer.Email ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@HomePhone", (object)customer.Phone ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MobilePhone", (object)customer.Mobile ?? DBNull.Value);

                    conn.Open();
                    int result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }


    }
}
