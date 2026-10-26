using System;
using webapi.Dtos;

namespace webapi.IService;

public interface IEmployeeService
{
       Task<Tuple<int,List<EmployeeDto>>> GetAllEmployeesAsync();
     Task<Tuple<int,string>>CreateEmployee(EmployeeDto dto);
     Task<Tuple<int,string>>UpdateEmployee(Guid id, EmployeeDto dto);
     Task<Tuple<int, string>> DeleteEmployee(Guid id);
     Task<Tuple<int, EmployeeDto?>> GetEmployeeById(Guid id);
}
