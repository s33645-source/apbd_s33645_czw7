using Microsoft.Data.SqlClient;
using WebApplication2.DTOs;

namespace WebApplication2.Service;

public class AppointmentsService : IAppointmentsService
{
    private readonly string _connectionString;
    public AppointmentsService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
    }
    
    public async Task<AppointmentListDto> GetAllAppointmentsAsync()
    {
        string query = "SELECT * FROM Appointments";
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        using var command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = query;
        var appointmentListDto = new AppointmentListDto()
        {
           AppointmentDetails= new List<AppointmentDetailsDto>()
        };
       var reader= await command.ExecuteReaderAsync();
       while (await reader.ReadAsync())
       {
           var details=new AppointmentDetailsDto()
           {
               IdAppointment = reader.GetInt32(reader.GetOrdinal("IdAppointment")),
               
           };
           appointmentListDto.AppointmentDetails.Add(details);
       }
       return appointmentListDto;
    }
}