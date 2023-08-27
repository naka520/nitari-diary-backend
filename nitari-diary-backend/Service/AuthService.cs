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
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

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

    public static async Task<bool> VerifyAccessToken(string accessToken)
    {
      string apiUrl = "https://api.line.me/oauth2/v2.1/verify";

      using (HttpClient client = new HttpClient())
      {
        var uriBuilder = new UriBuilder(apiUrl);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["access_token"] = accessToken;
        uriBuilder.Query = query.ToString();
        var url = uriBuilder.ToString();

        HttpResponseMessage response = await client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
          string message = await response.Content.ReadAsStringAsync();
          var tokenInfo = JsonConvert.DeserializeObject<TokenInfo>(message);
          return tokenInfo.ExpiresIn > 0;
        }
        else
        {
          string message = $"Error: {response.Content.ReadAsStringAsync()}";
          return false;
        }
      }
    }

    public class TokenInfo
    {
      [JsonProperty("scope")]
      public string Scope { get; set; }

      [JsonProperty("client_id")]
      public string ClientId { get; set; }

      [JsonProperty("expires_in")]
      public int ExpiresIn { get; set; }
    }
  }
}
