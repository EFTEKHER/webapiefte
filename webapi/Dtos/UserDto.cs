using System;
using System.ComponentModel.DataAnnotations;

namespace webapi.Dtos;

public class UserDto
{
  public Guid Id{ get; set; }= Guid.NewGuid();
  public  string? Name { get; set; }
  [Required]
  [EmailAddress]
  public string Email { get; set; } = string.Empty;
  [Required]
  public string Password{ get; set; } = string.Empty;

}
