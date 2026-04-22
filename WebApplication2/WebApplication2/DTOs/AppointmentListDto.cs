namespace WebApplication2.DTOs;

public class AppointmentListDto
{
    public List<AppointmentDetailsDto> AppointmentDetails { get; set; } = new();
}