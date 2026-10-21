using System;
using webapi.Dtos;

namespace webapi.IService;

public interface IEmployeeService
{
     public  Task<Tuple<int,List<EmployeeDto>>> GetAllEmployeesAsync();
}
