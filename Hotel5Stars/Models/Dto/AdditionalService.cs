namespace Hotel5Stars.Models.Dto
{
    public class AdditionalService
    {
        public int AdditionalServiceId { get; set; }
        public string? Name { get; set; } // Modificato per essere nullable
        public DateTime Date { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public int ReservationId { get; set; }
        public Reservation? Reservation { get; set; } // Modificato per essere nullable
    }
}
