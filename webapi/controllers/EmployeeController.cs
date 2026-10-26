using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webapi.Dtos;
using webapi.GenericResponse;
using webapi.IService;

namespace webapi.controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController(IEmployeeService employeeService) : ControllerBase
    {
        [HttpGet("GetAllEmployees")]
        public async Task<IActionResult> GetAllEmployees()
        {
            try
            {
                var result= await employeeService.GetAllEmployeesAsync();
                if (!result.Item2.Any())
                {
                    return Ok(
                        ResponseResult<List<EmployeeDto>>.Failure(null,"No employees found")
                    );
                }
                return Ok(ResponseResult<List<EmployeeDto>>.Success(result.Item2,"Employees Found"));
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing the request");
            }
        }
        [HttpPost("CreateEmployee")]
        public async Task<IActionResult> CreateEmploye([FromBody] EmployeeDto dto)
        {
            try
            {
                var result= await employeeService.CreateEmployee(dto);
                if(result.Item1==0)
                {
                    return BadRequest(ResponseResult<string>.Failure(null,result.Item2));
                }
                return Ok(ResponseResult<string>.Success(null,"Employee created successfully"));
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing the request");
            }
        }

        [HttpPut("UpdateEmployee/{id:guid}")]
        public async Task<IActionResult> UpdateEmployee(Guid id, [FromBody] EmployeeDto dto)
        {
            var result = await employeeService.UpdateEmployee(id, dto);
            if (result.Item1 == 0)
            {
                return NotFound(ResponseResult<string>.Failure(null, result.Item2));
            }
            if (result.Item1 == 2)
            {
                return BadRequest(ResponseResult<string>.Failure(null, result.Item2));
            }
            if (result.Item1 == 3)
            {
                return StatusCode(500, ResponseResult<string>.Failure(null, result.Item2));
            }
            return Ok(ResponseResult<string>.Success(null, result.Item2));
        }

        // Supports DeleteEmployee?id=... as well as DeleteEmployee/{id}
        [HttpDelete("DeleteEmployee")]
        public Task<IActionResult> DeleteEmployeeByQuery([FromQuery] Guid id) => DeleteEmployee(id);

        [HttpDelete("DeleteEmployee/{id:guid}")]
        public async Task <IActionResult> DeleteEmployee(Guid id)
        {
            try
            {
                var result=await employeeService.DeleteEmployee(id);
                if (result.Item1 == 0)
                {
                    return Ok(ResponseResult<string>.Failure(null,result.Item2));
                }
                return Ok(ResponseResult<string>.Success(null, result.Item2));
                
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Supports GetEmployeeById?id=... as well as GetEmployeeById/{id}
        [HttpGet("GetEmployeeById")]
        public Task<IActionResult> GetEmployeeByIdQuery([FromQuery] Guid id) => GetEmployeeById(id);

        [HttpGet("GetEmployeeById/{id:guid}")]
        public async Task<IActionResult> GetEmployeeById(Guid id)
        {
            var result = await employeeService.GetEmployeeById(id);
            if (result.Item1 == 0)
            {
                return NotFound(ResponseResult<EmployeeDto>.Failure(null, "Employee not found"));
            }
            if (result.Item1 == 3)
            {
                return StatusCode(500, ResponseResult<EmployeeDto>.Failure(null, "An error occurred while processing the request"));
            }
            return Ok(ResponseResult<EmployeeDto>.Success(result.Item2, "Employee found"));
        }
    }
}
