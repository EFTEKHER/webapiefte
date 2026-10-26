using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using  webapi.Dtos;
using webapi.Entities;
using webapi.GenericResponse;
using webapi.IService;
using webapi.Services;
namespace webapi.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        //Registration

        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
    [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]UserDto dto)
        {
            try
            {
                var result=await _authService.LoginUser(dto);
                if (result.Item1 == 0)
                {
                    return NotFound(ResponseResult<TokenDto>.Failure(null, result.Item2.Message));
                }
                if (result.Item1==1)
                {
                    return BadRequest(ResponseResult<TokenDto>.Failure(null, "Invalid credentials"));
                }
                if (result.Item1 == 3)
                {
                    return StatusCode(500, ResponseResult<TokenDto>.Failure(null, result.Item2.Message));
                }
                return Ok(ResponseResult<TokenDto>.Success(result.Item2, result.Item2.Message));
            }
            catch(Exception)
            {
                throw;
            }
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody]UserDto userDto)
        {
           try{ 
            var result=await _authService.RegisterUser(userDto);
                if (result.Item1 == 0)
                {
                    return Ok(ResponseResult<string>.Failure(null, "User already exists"));
                }
           
           return Ok(ResponseResult<string>.Success(null, result.Item2));
           
           }
            catch (Exception)
            {
                throw;
            }
           
        }


    }
}
