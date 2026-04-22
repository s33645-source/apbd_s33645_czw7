using Microsoft.AspNetCore.Mvc;
using WebApplication2.DTOs;
using WebApplication2.Service;

namespace WebApplication2.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AppointmentController : ControllerBase
{
    private readonly IAppointmentsService _appointmentsService;

    public AppointmentController(IAppointmentsService appointmentsService)
    {
        _appointmentsService = appointmentsService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var appointments = await _appointmentsService.GetAllAppointmentsAsync();
        return Ok(appointments);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var appointment = await _appointmentsService.GetAppointmentByIdAsync(id);
        if (appointment is null)
            return NotFound(new ErrorResponseDto { Message = $"Appointment with id {id} not found." });
        return Ok(appointment);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAppointmentRequestDto dto)
    {
        int newId = await _appointmentsService.CreateAppointmentAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = newId }, new { IdAppointment = newId });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAppointmentRequestDto dto)
    {
        bool updated = await _appointmentsService.UpdateAppointmentAsync(id, dto);
        if (!updated)
            return NotFound(new ErrorResponseDto { Message = $"Appointment with id {id} not found." });
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        bool deleted = await _appointmentsService.DeleteAppointmentAsync(id);
        if (!deleted)
            return NotFound(new ErrorResponseDto { Message = $"Appointment with id {id} not found." });
        return NoContent();
    }
}