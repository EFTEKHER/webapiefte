using System;
using webapi.Dtos;

namespace webapi.IService;

public interface IAuthService
{
 Task<Tuple<int,string>> LoginUser(UserDto dto);
 Task<Tuple<int,string>> RegisterUser(UserDto dto);
}
