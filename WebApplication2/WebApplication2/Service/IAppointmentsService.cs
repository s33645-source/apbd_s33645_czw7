using WebApplication2.DTOs;
namespace WebApplication2.Service;

public interface IAppointmentsService
{
    Task<AppointmentListDto> GetAllAppointmentsAsync();
    Task<AppointmentDetailsDto?> GetAppointmentByIdAsync(int id);
    Task<int> CreateAppointmentAsync(CreateAppointmentRequestDto dto);
    Task<bool> UpdateAppointmentAsync(int id, UpdateAppointmentRequestDto dto);
    Task<bool> DeleteAppointmentAsync(int id);
}