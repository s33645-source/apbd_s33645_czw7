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
        string query = """
                       SELECT a.IdAppointment, a.AppointmentDate, a.Status, a.Reason, a.InternalNotes, a.CreatedAt, p.FirstName +' '+ p.LastName AS PatientFullName, p.Email, p.Phone, d.FirstName +' '+ d.LastName AS DoctorFullName, d.LicenseNumber
                       from Appointments a INNER JOIN Patients p ON a.IdPatient = p.IdPatient
                       INNER JOIN Doctors d ON a.IdDoctor = a.IdDoctor
                       """;
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(query, connection);
        var appointmentListDto = new AppointmentListDto()
        {
            AppointmentDetails = new List<AppointmentDetailsDto>()
        };
        var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var details = new AppointmentDetailsDto()
            {
                IdAppointment = reader.GetInt32(reader.GetOrdinal("IdAppointment")),
                AppointmentDate = reader.GetDateTime(reader.GetOrdinal("AppointmentDate")),
                Status = reader.GetString(reader.GetOrdinal("Status")),
                Reason = reader.GetString(reader.GetOrdinal("Reason")),
                InternalNotes = reader.IsDBNull(reader.GetOrdinal("InternalNotes"))
                    ? null
                    : reader.GetString(reader.GetOrdinal("InternalNotes")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                PatientFullName = reader.GetString(reader.GetOrdinal("PatientFullName")),
                PatientEmail = reader.GetString(reader.GetOrdinal("Email")),
                PatientPhone = reader.GetString(reader.GetOrdinal("Phone")),
                DoctorFullName = reader.GetString(reader.GetOrdinal("DoctorFullName")),
                DoctorLicenseNumber = reader.GetString(reader.GetOrdinal("LicenseNumber"))

            };
            appointmentListDto.AppointmentDetails.Add(details);
        }

        return appointmentListDto;
    }

    public async Task<AppointmentDetailsDto?> GetAppointmentByIdAsync(int id)
    {
        string query = """
                       SELECT a.IdAppointment, a.AppointmentDate, a.Status, a.Reason, a.InternalNotes, a.CreatedAt, p.FirstName +' '+ p.LastName AS PatientFullName, p.Email, p.Phone,
                       d.FirstName +' '+ d.LastName AS DoctorFullName, d.LicenseNumber
                       from Appointments a INNER JOIN Patients p on a.IdPatient = p.IdPatient
                       INNER JOIN Doctors d ON d.IdDoctor = a.IdDoctor
                       where a.IdAppointment=@IdAppointment
                       """;
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@IdAppointment", id);
        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return null;
        return new AppointmentDetailsDto
        {
            IdAppointment = reader.GetInt32(reader.GetOrdinal("IdAppointment")),
            AppointmentDate = reader.GetDateTime(reader.GetOrdinal("AppointmentDate")),
            Status = reader.GetString(reader.GetOrdinal("Status")),
            Reason = reader.GetString(reader.GetOrdinal("Reason")),
            InternalNotes = reader.IsDBNull(reader.GetOrdinal("InternalNotes"))
                ? null
                : reader.GetString(reader.GetOrdinal("InternalNotes")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
            PatientFullName = reader.GetString(reader.GetOrdinal("PatientFullName")),
            PatientEmail = reader.GetString(reader.GetOrdinal("Email")),
            PatientPhone = reader.GetString(reader.GetOrdinal("Phone")),
            DoctorFullName = reader.GetString(reader.GetOrdinal("DoctorFullName")),
            DoctorLicenseNumber = reader.GetString(reader.GetOrdinal("LicenseNumber"))
        };
    }

    public async Task<int> CreateAppointmentAsync(CreateAppointmentRequestDto dto)
    {
        string query = """
                       INSERT INTO Appointments (IdPatient,IdDoctor, AppointmentDate, Status, Reason, CreatedAt)
                       OUTPUT INSERTED.Id
                       VALUES (@IdPatient, @IdDoctor, @AppointmentDate, 'Scheduled',@Reason, GETDATE())
                       """;
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@IdPatient", dto.IdPatient);
        command.Parameters.AddWithValue("@IdDoctor", dto.IdDoctor);
        command.Parameters.AddWithValue("@AppointmentDate", dto.AppointmentDate);
        command.Parameters.AddWithValue("@Reason", dto.Reason);
        var newId = await command.ExecuteScalarAsync();
        return Convert.ToInt32(newId);
    }

    public async Task<bool> UpdateAppointmentAsync(int id, UpdateAppointmentRequestDto dto)
    {
        string query = """
                       UPDATE Appointments SET
                                               IdPatient=@IdPatient,
                                               IdDoctor=@IdDoctor,
                                               AppointmentDate=@AppointmentDate,
                                               Status=@Status,
                                               Reason=@Reason,
                                               InternalNotes=@InternalNotes,
                                               Where IdAppointment=@IdAppointment
                       """;
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@IdAppointment", id);
        command.Parameters.AddWithValue("@IdPatient", dto.IdPatient);
        command.Parameters.AddWithValue("@IdDoctor", dto.IdDoctor);
        command.Parameters.AddWithValue("@AppointmentDate", dto.AppointmentDate);
        command.Parameters.AddWithValue("@Status", dto.Status);
        command.Parameters.AddWithValue("@Reason", dto.Reason);
        command.Parameters.AddWithValue("@InternalNotes", dto.InternalNotes ?? (object)DBNull.Value);
        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    public async Task<bool> DeleteAppointmentAsync(int id)
    {
        string query = """
                       DELETE FROM  Appointments WHERE IdAppointment = @IdAppointment
                       """;
        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@IdAppointment", id);
        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }
}
