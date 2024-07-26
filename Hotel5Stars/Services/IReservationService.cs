using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Hotel5Stars.Models.Dto;
using Hotel5Stars.Models.ViewModels;

public interface IReservationService
{
    Task<IEnumerable<ReservationViewModel>> GetAllReservationsAsync();
    Task<IEnumerable<SelectListItem>> GetCustomersAsync();
    Task<IEnumerable<SelectListItem>> GetRoomsAsync();
    Task AddReservationAsync(Reservation reservation);
    Task<IEnumerable<Reservation>> GetReservationsByFiscalCodeAsync(string fiscalCode);
    Task<int> GetFullBoardCountAsync();
    Task<IEnumerable<AdditionalService>> GetAdditionalServicesByReservationIdAsync(int reservationId);
    Task<ReservationViewModel> GetReservationByIdAsync(int id);
    Task<bool> AddAdditionalServiceAsync(AdditionalService service);
}
