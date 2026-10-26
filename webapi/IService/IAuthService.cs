using System;
using webapi.Dtos;
using webapi.Entities;

namespace webapi.IService;

public interface IAuthService
{
 Task<Tuple<int,TokenDto>> LoginUser(UserDto dto);
 Task<Tuple<int,string>> RegisterUser(UserDto dto);
}
