using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Azure.Core;
using Microsoft.Extensions.Logging;
using Azure;
using System.Linq;
using System.Collections.Generic;

namespace nitari_diary_backend.Service
{
  public class AuthService
  {
    private readonly string SUPABASE_URL = "";
    private readonly string SUPABASE_KEY = "";
    private readonly ILogger<AuthService> _logger;

    public IEnumerable<Claim> GetCurrentUserId(string token)
    {
      try
      {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(SUPABASE_KEY);
        var tokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(key),
          ValidateIssuer = false,
          ValidateAudience = false
        };

        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
        var jwtToken = (JwtSecurityToken)validatedToken;

        _logger.LogInformation("GetCurrentUserId is called");

        var value = jwtToken?.Claims;

        return value;
      }
      catch (Exception ex)
      {
        _logger.LogError("Error in GetCurrentUserId: {0}", ex.Message);
        throw new RequestFailedException(401, "Invalid token");
      }
    }
  }
}
