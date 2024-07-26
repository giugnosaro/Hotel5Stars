using System.Collections.Generic;
using System.Threading.Tasks;
using Hotel5Stars.Models.Dto;

namespace Hotel5Stars.Services
{
    public interface ICustomerService
    {
        Task<List<Customer>> GetAllCustomersAsync(); 
        Task<bool> AddCustomerAsync(Customer customer); 
    }
}
