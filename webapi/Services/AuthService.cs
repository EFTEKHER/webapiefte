using System;
using Microsoft.EntityFrameworkCore;
using webapi.Data;
using webapi.Dtos;
using webapi.IService;
using webapi.Entities;
namespace webapi.Services;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

public class AuthService:IAuthService
{
 private readonly AppDbContext _context;
 private readonly IConfiguration _configuration;
 public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }
    private (string Token, DateTime ExpiresAt) GetJwtToken(User user)
    {
        var claims=new[]
        {
            new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
            new Claim(ClaimTypes.Name,user.Name ?? string.Empty),
            new Claim(ClaimTypes.Email,user.Email ?? string.Empty),
        };
        var jwtKey = _configuration["JWT:Key"]
            ?? throw new InvalidOperationException("JWT:Key is missing in appsettings.json");
        var key= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials=new SigningCredentials(key,SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddMinutes(15);
        var token= new JwtSecurityToken(
            issuer:_configuration["JWT:Issuer"],
            audience:_configuration["JWT:Audience"],
            claims:claims,
            expires:expiresAt,
            signingCredentials:credentials
        );
        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }

    private static Tuple<int,TokenDto> Result(int code, string message)
    {
        return new Tuple<int,TokenDto>(code, new TokenDto { Message = message });
    }

    private Tuple<int,TokenDto> LoginSuccess(User user)
    {
        var (token, expiresAt) = GetJwtToken(user);
        return new Tuple<int,TokenDto>(2, new TokenDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            Message = "Login successful"
        });
    }

    public async Task<Tuple<int,TokenDto>> LoginUser(UserDto dto)
    {
        try
        {
            if(dto == null)
            {
                return Result(1, "Invalid user data");
            }
            var existingUser= await _context.AccountUsers.FirstOrDefaultAsync(x=>x.Email==dto.Email);
            if (existingUser == null)
            {
                return Result(0, "User not found.please register");
            }
            if (string.IsNullOrEmpty(existingUser.Password))
            {
                return Result(1, "Invalid password");
            }
            var passwordHaser= new PasswordHasher<string>();
            var verifyPassword=passwordHaser.VerifyHashedPassword(dto.Email,existingUser.Password,dto.Password);

            switch (verifyPassword)
            {
                case PasswordVerificationResult.Success:
                    return LoginSuccess(existingUser);

                case PasswordVerificationResult.SuccessRehashNeeded:
                    // Password is correct, but the stored hash uses old settings: upgrade it
                    existingUser.Password=PasswordHasing(dto);
                    await _context.SaveChangesAsync();
                    return LoginSuccess(existingUser);

                default:
                    return Result(1, "Invalid password");
            }
        }
        catch(Exception)
        {
            return Result(3, "An error occurred while processing the request");
        }
    }
    public async Task <Tuple<int,string>> RegisterUser(UserDto dto)
    {
        try
        {
            var existingUser=await _context.AccountUsers.AnyAsync(x=> x.Email==dto.Email);
            if(existingUser)
            {
                return new Tuple<int,string>(0, "User already exists");
            }
            // Additional registration logic would go here
            _context.AccountUsers.Add(new User
            {
                Id=Guid.NewGuid(),
                Name=dto.Name,
                Email=dto.Email,
                Password=PasswordHasing(dto),
            });
            await _context.SaveChangesAsync();
            return new Tuple<int,string>(1, "User registered successfully");
        }
        catch (Exception)
        {
            throw;
        }

        
    } 
    private string PasswordHasing(UserDto dto)
        {
            var passwordHaser=new PasswordHasher<string>();
            var hash= passwordHaser.HashPassword(dto.Email, dto.Password);
            return hash;
        }   
}
