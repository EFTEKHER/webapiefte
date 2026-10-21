using System;
using Microsoft.EntityFrameworkCore;
using webapi.Data;
using webapi.Dtos;
using webapi.IService;
using webapi.Entities;
namespace webapi.Services;
using Microsoft.AspNetCore.Identity;
public class AuthService:IAuthService
{
 private readonly AppDbContext _context;
 public AuthService(AppDbContext context)
    {
        _context = context;
    }
 public async Task<Tuple<int,string>> LoginUser(UserDto dto)
    {
        try
        {
            if(dto == null)
            {
                return  new Tuple<int, string>(0, "Invalid user data");
            }
            var existingUser= await _context.AccountUsers.FirstOrDefaultAsync(x=>x.Email==dto.Email);
            if (existingUser == null)
            {
                return new Tuple<int,string>(0, "User not found.please login");
            }
            // if(existingUser.Password != dto.Password)
            // {
            //     return new Tuple<int,string>(1, "Invalid password");
            // }
            if (string.IsNullOrEmpty(existingUser.Password))
            {
                return new Tuple<int,string>(1, "Invalid password");
            }
            var  passwordHaser= new PasswordHasher<string>();
            var verifyPassword=passwordHaser.VerifyHashedPassword(dto.Email,existingUser.Password,dto.Password);
            if (verifyPassword == PasswordVerificationResult.Success)
            {
                return new Tuple<int,string>(2, "Login successful");
            }
            else if (verifyPassword == PasswordVerificationResult.SuccessRehashNeeded)
            {
                existingUser.Password=PasswordHasing(dto);
                _context.AccountUsers.Update(existingUser);
                await _context.SaveChangesAsync();
                return new Tuple<int,string>(1,"Password updated successfully");
            }
            else if(verifyPassword == PasswordVerificationResult.Failed)
            {
                return new Tuple<int,string>(1, "Invalid password");
            }
            return new Tuple<int,string>(0, "An unexpected error occurred");
        }
        catch(Exception)
        {
            return new Tuple<int,string>(3, "An error occurred while processing the request");
            
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
