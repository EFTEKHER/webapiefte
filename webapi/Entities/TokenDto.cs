using System;

namespace webapi.Entities;

public class TokenDto
{
     public string? Token { get; set; }
     public DateTime? ExpiresAt { get; set; }
     public string Message { get; set; } = string.Empty;
}
