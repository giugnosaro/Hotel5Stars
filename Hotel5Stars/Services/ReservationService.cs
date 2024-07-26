using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Hotel5Stars.Models.Dto;
using Hotel5Stars.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hotel5Stars.Services
{
    public class ReservationService : IReservationService
    {
        private readonly string _connectionString;

        public ReservationService(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<IEnumerable<ReservationViewModel>> GetAllReservationsAsync()
        {
            var reservations = new List<ReservationViewModel>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT r.ReservationId, r.CustomerId, r.RoomId, r.FromDate, r.ToDate, r.Deposit, r.Rate, r.Details,
                           c.LastName AS CustomerLastName, ro.RoomNumber
                    FROM Reservations r
                    INNER JOIN Customers c ON r.CustomerId = c.CustomerId
                    INNER JOIN Rooms ro ON r.RoomId = ro.RoomId";

                using (var cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            reservations.Add(new ReservationViewModel
                            {
                                ReservationId = (int)reader["ReservationId"],
                                CustomerLastName = reader["CustomerLastName"].ToString(),
                                RoomNumber = reader["RoomNumber"].ToString(),
                                FromDate = (DateTime)reader["FromDate"],
                                ToDate = (DateTime)reader["ToDate"],
                                Deposit = (decimal)reader["Deposit"],
                                Rate = (decimal)reader["Rate"],
                                Details = reader["Details"]?.ToString() ?? string.Empty
                            });
                        }
                    }
                }
            }
            return reservations;
        }

        public async Task<IEnumerable<SelectListItem>> GetCustomersAsync()
        {
            var customers = new List<SelectListItem>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = "SELECT CustomerId, LastName FROM Customers";
                using (var cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            customers.Add(new SelectListItem
                            {
                                Value = reader["CustomerId"].ToString(),
                                Text = reader["LastName"].ToString()
                            });
                        }
                    }
                }
            }
            return customers;
        }

        public async Task<IEnumerable<SelectListItem>> GetRoomsAsync()
        {
            var rooms = new List<SelectListItem>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = "SELECT RoomId, RoomNumber FROM Rooms";
                using (var cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            rooms.Add(new SelectListItem
                            {
                                Value = reader["RoomId"].ToString(),
                                Text = reader["RoomNumber"].ToString()
                            });
                        }
                    }
                }
            }
            return rooms;
        }

        public async Task AddReservationAsync(Reservation reservation)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = @"
                    INSERT INTO Reservations (CustomerId, RoomId, FromDate, ToDate, Deposit, Rate, Details)
                    VALUES (@CustomerId, @RoomId, @FromDate, @ToDate, @Deposit, @Rate, @Details)";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerId", reservation.CustomerId);
                    cmd.Parameters.AddWithValue("@RoomId", reservation.RoomId);
                    cmd.Parameters.AddWithValue("@FromDate", reservation.FromDate);
                    cmd.Parameters.AddWithValue("@ToDate", reservation.ToDate);
                    cmd.Parameters.AddWithValue("@Deposit", reservation.Deposit);
                    cmd.Parameters.AddWithValue("@Rate", reservation.Rate);
                    cmd.Parameters.AddWithValue("@Details", reservation.Details);

                    conn.Open();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<IEnumerable<Reservation>> GetReservationsByFiscalCodeAsync(string fiscalCode)
        {
            var reservations = new List<Reservation>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = @"
                    SELECT r.*
                    FROM Reservations r
                    INNER JOIN Customers c ON r.CustomerId = c.CustomerId
                    WHERE c.FiscalCode = @FiscalCode";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@FiscalCode", fiscalCode);
                    conn.Open();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            reservations.Add(new Reservation
                            {
                                ReservationId = (int)reader["ReservationId"],
                                CustomerId = (int)reader["CustomerId"],
                                RoomId = (int)reader["RoomId"],
                                FromDate = (DateTime)reader["FromDate"],
                                ToDate = (DateTime)reader["ToDate"],
                                Deposit = (decimal)reader["Deposit"],
                                Rate = (decimal)reader["Rate"],
                                Details = reader["Details"]?.ToString() ?? string.Empty
                            });
                        }
                    }
                }
            }
            return reservations;
        }

        public async Task<int> GetFullBoardCountAsync()
        {
            int count = 0;
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = "SELECT COUNT(*) FROM Reservations WHERE Details LIKE '%Full board%'";
                using (var cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    count = (int)(await cmd.ExecuteScalarAsync() ?? 0);
                }
            }
            return count;
        }

        public async Task<ReservationViewModel> GetReservationByIdAsync(int id)
        {
            ReservationViewModel reservation = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM Reservations WHERE ReservationId = @ReservationId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ReservationId", id);

                conn.Open();
                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                if (reader.Read())
                {
                    reservation = new ReservationViewModel
                    {
                        ReservationId = (int)reader["ReservationId"],
                        CustomerLastName = reader["CustomerLastName"].ToString(),
                        RoomNumber = reader["RoomNumber"].ToString(),
                        FromDate = (DateTime)reader["FromDate"],
                        ToDate = (DateTime)reader["ToDate"],
                        Deposit = (decimal)reader["Deposit"],
                        Rate = (decimal)reader["Rate"],
                        Details = reader["Details"].ToString()
                    };

                    reservation.AdditionalServices = (await GetAdditionalServicesByReservationIdAsync(reservation.ReservationId)).ToList();
                }
            }

            return reservation;
        }


        public async Task<IEnumerable<AdditionalService>> GetAdditionalServicesByReservationIdAsync(int reservationId)
        {
            var services = new List<AdditionalService>();
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = "SELECT * FROM AdditionalServices WHERE ReservationId = @ReservationId";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ReservationId", reservationId);
                    conn.Open();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            services.Add(new AdditionalService
                            {
                                AdditionalServiceId = (int)reader["AdditionalServiceId"],
                                Name = reader["Name"].ToString(),
                                Date = (DateTime)reader["Date"],
                                Quantity = (int)reader["Quantity"],
                                Price = (decimal)reader["Price"],
                                ReservationId = (int)reader["ReservationId"]
                            });
                        }
                    }
                }
            }
            return services;
        }

        public async Task<bool> AddAdditionalServiceAsync(AdditionalService service)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = @"
                    INSERT INTO AdditionalServices (Name, Date, Quantity, Price, ReservationId)
                    VALUES (@Name, @Date, @Quantity, @Price, @ReservationId)";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", service.Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Date", service.Date);
                    cmd.Parameters.AddWithValue("@Quantity", service.Quantity);
                    cmd.Parameters.AddWithValue("@Price", service.Price);
                    cmd.Parameters.AddWithValue("@ReservationId", service.ReservationId);

                    conn.Open();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            return true;
        }
    }
}
