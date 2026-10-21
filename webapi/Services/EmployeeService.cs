using System;
using Microsoft.EntityFrameworkCore;
using webapi.Data;
using webapi.Dtos;
using webapi.IService;

namespace webapi.Services;

public class EmployeeService(AppDbContext _context):IEmployeeService
{
    public async Task<Tuple<int,List<EmployeeDto>>> GetAllEmployeesAsync()
    {
        var employees = await _context.Employees.Select(x=>new EmployeeDto
        {
            Id = x.Id,
            Name = x.Name,
            CreatedDate = x.CreatedDate,
            DOB = x.DOB,
            Position = x.Position,
            Department = x.Department,
            EmailAddress = x.EmailAddress
        }).ToListAsync();
        return new Tuple<int,List<EmployeeDto>>(1, employees);
    }
}
