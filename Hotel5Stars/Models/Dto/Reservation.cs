namespace Hotel5Stars.Models.Dto
{
    public class Reservation
    {
        public int ReservationId { get; set; }
        public int CustomerId { get; set; }
        public int RoomId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal Deposit { get; set; }
        public decimal Rate { get; set; }
        public string Details { get; set; } = string.Empty;
    }
}
