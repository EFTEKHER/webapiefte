using System;
using Microsoft.EntityFrameworkCore;
using webapi.Data;
using webapi.Dtos;
using webapi.IService;
using webapi.Entities;
namespace webapi.Services;

public class EmployeeService(AppDbContext _context):IEmployeeService
{
    public async Task<Tuple<int,List<EmployeeDto>>> GetAllEmployeesAsync()
    {
        //read
        var employees = await _context.Employees.AsNoTracking().Select(x=>new EmployeeDto
        {
            Id = x.Id,
            Name = x.Name,
            CreatedDate = x.CreatedDate,
            LastModified = x.LastModified,
            DOB = x.DOB,
            Position = x.Position,
            Department = x.Department,
            EmailAddress = x.EmailAddress
        }).ToListAsync();
        return new Tuple<int,List<EmployeeDto>>(1, employees);

    }
    public async Task  <Tuple<int,string>>CreateEmployee(EmployeeDto dto)
    {
        var existingUser=await _context.Employees.AnyAsync(x=>x.EmailAddress==dto.EmailAddress);
        if (existingUser)
        {
            return new Tuple<int, string>(0, "Employee with this email already exists");
        }
        // Create new employee
        var employee = new Employee
        {
            Id=Guid.NewGuid(),
            Name = dto.Name,
            CreatedDate = DateTime.UtcNow,
            DOB = dto.DOB,
            Position = dto.Position,
            Department = dto.Department,
            EmailAddress = dto.EmailAddress
        };
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        return new Tuple<int, string>(1, "Employee created successfully");
    }
    public async Task<Tuple<int,string>> UpdateEmployee(Guid id, EmployeeDto dto)
    {
        try
        {
            var existing = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id);
            if (existing == null)
            {
                return new Tuple<int, string>(0, "Employee not found");
            }

            // Email must stay unique across employees
            var emailTaken = await _context.Employees.AnyAsync(x => x.EmailAddress == dto.EmailAddress && x.Id != id);
            if (emailTaken)
            {
                return new Tuple<int, string>(2, "Another employee already uses this email");
            }

            existing.Name = dto.Name??existing.Name;
            existing.DOB = dto.DOB??existing.DOB;
            existing.Position = string.IsNullOrWhiteSpace(dto.Position) ? existing.Position : dto.Position;
            existing.Department = string.IsNullOrWhiteSpace(dto.Department) ? existing.Department : dto.Department;
            existing.EmailAddress = string.IsNullOrWhiteSpace(dto.EmailAddress) ? existing.EmailAddress : dto.EmailAddress;
            existing.LastModified = DateTime.UtcNow;
            _context.Employees.Update(existing);
            await _context.SaveChangesAsync();
            return new Tuple<int, string>(1, "Employee updated successfully");
        }
        catch (Exception)
        {
            return new Tuple<int, string>(3, "An error occurred while processing the request");
        }

    }

    public async Task<Tuple<int, string>> DeleteEmployee(Guid id)
    {
        var data= await _context.Employees.FirstOrDefaultAsync(x => x.Id == id);

        if (data == null)
        {
            return new Tuple<int, string>(0, "Employee not found");
        }

        _context.Employees.Remove(data);
        await _context.SaveChangesAsync();
        return new Tuple<int, string>(1, "Employee deleted successfully");
    }
    public async Task<Tuple<int, EmployeeDto?>> GetEmployeeById(Guid id)
    {
        try
        {
            var employee = await _context.Employees
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new EmployeeDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    CreatedDate = x.CreatedDate,
                    LastModified = x.LastModified,
                    DOB = x.DOB,
                    Position = x.Position,
                    Department = x.Department,
                    EmailAddress = x.EmailAddress
                })
                .FirstOrDefaultAsync();

            if (employee == null)
            {
                return new Tuple<int, EmployeeDto?>(0, null);
            }
            return new Tuple<int, EmployeeDto?>(1, employee);
        }
        catch (Exception)
        {
            return new Tuple<int, EmployeeDto?>(3, null);
        }
    }
}
