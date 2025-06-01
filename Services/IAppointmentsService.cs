using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorOfficina.Data.Dtos;

namespace BlazorOfficina.Services
{
    public interface IAppointmentService
    {
        Task<int> GetRegisteredVehiclesCountAsync(string userId);
        Task<int> GetCompletedRepairsCountAsync(string userId);
        Task<int> GetInProgressRepairsCountAsync(string userId);
        Task<DateTime?> GetNextAppointmentDateAsync(string userId);
        Task<List<RiparazioneDto>> GetOngoingRepairsAsync(string userId);
        Task<List<AppuntamentoDto>> GetUpcomingAppointmentsAsync(string userId, int count);
        Task CancelAppointmentAsync(int appointmentId);
        Task<int> GetTodaysCountForMechanicAsync(string mechanicId);
        Task<List<AppuntamentoDto>> GetAppointmentsForDateAsync(DateTime date);
        Task<IEnumerable<MechanicAppointmentDto>> GetByMechanicAsync(string mechanicId);
        Task CancelAppointmentAsync(int appointmentId, string userId);
        Task<int> GetCurrentAppointmentsCountForMechanicAsync(string mechanicId);
        Task<int> GetCurrentAppointmentsCountForCustomerAsync(string userId);
        Task<int> GetCompletedAppointmentsCountForCustomerAsync(string userId);
    }
}