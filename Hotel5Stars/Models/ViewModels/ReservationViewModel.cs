using Hotel5Stars.Models.Dto;

namespace Hotel5Stars.Models.ViewModels
{
    public class ReservationViewModel
    {
        public int ReservationId { get; set; }
        public string CustomerLastName { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal Deposit { get; set; }
        public decimal Rate { get; set; }
        public string Details { get; set; } = string.Empty;
        public List<AdditionalService> AdditionalServices { get; set; }
    }
}
