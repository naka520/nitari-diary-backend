using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using nitari_diary_backend.Service;
using nitari_diary_backend.Entity;
using System.Threading.Tasks;
using Azure.Data.Tables;
using System.Net;
using System;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;



namespace nitari_diary_backend
{
  public class NitariBackend
  {
    private readonly ILogger<NitariBackend> _logger;

    public NitariBackend(ILogger<NitariBackend> log)
    {
      _logger = log;
    }

    // Specific day's token metrics
    [FunctionName("GetDiaryAll")]
    [OpenApiOperation(operationId: "GetDiaryAll", tags: new[] { "Diary" }, Description = "Get Diary All Entity of Login User")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
    public static async Task<IActionResult> GetDailySpecificUser(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "diary/all")] HttpRequest req,
    [Table("DailyEntity"/*, Connection = "MyStorage"*/)] TableClient tableClient, ILogger log)
    {
      log.LogInformation($"/diary/all");

      //await foreach (var entity in tableClient.QueryAsync<TableEntity>(filter: $"PartitionKey eq 'TokenMetrics' and TimePeriod eq 'Day' and RowKey eq '{date}-{tokenName}'"))
      //{
      //metrics.Add(new TokenMetric
      //{
      //  TimePeriod = entity.GetString("TimePeriod"),
      //  TokenName = entity.GetString("TokenName"),
      //  TotalAmount = entity.GetDouble("TotalAmount"),
      //  Date = entity.RowKey.Substring(0, 8)
      //});
      //}

      //return new OkObjectResult(metrics);
      return new OkObjectResult("metrics");
    }

    // Specific day's Diary
    [FunctionName("PostDiary")]
    [OpenApiOperation(operationId: "PostDiary", tags: new[] { "Diary" }, Description = "Post Diary")]
    [OpenApiParameter(name: "title", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "title")]
    [OpenApiParameter(name: "description", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "description")]
    [OpenApiParameter(name: "date", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The specific date in the format YYYYMMDD")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
    public static async Task<IActionResult> PostDailySpecificUser(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "api/diary")] HttpRequest req,
    [Table("DailyEntity"/*, Connection = "MyStorage"*/)] TableClient tableClient,
    string title,
    string description,
    string date,
    ILogger log)
    {
      log.LogInformation($"[POST] /daily called with title={req.Query["title"]}");

      AuthService authService = new AuthService();
      //string authHeader = req.Headers["Authorization"];
      //string token = authHeader.Replace("Bearer ", "");
      var user = authService.GetCurrentUserId(req.Headers["Authorization"]);

      log.LogInformation($"[POST] /daily called with user={user}");

      //DiaryEntity diaryEntity = new DiaryEntity
      //{
      //  PartitionKey = user,
      //  RowKey = date,
      //  UserId = user,
      //  Title = title,
      //  Description = description,
      //  Type = "Diary",
      //  CreatedAt = DateTime.Now.ToString("yyyyMMddHHmmss")
      //};

      //return new OkObjectResult(metrics);
      return new OkObjectResult("metrics");
    }

    //[FunctionName("Auth")]
    //public static async Task<IActionResult> Auth(
    //    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
    //    ILogger log)
    //{
    //  log.LogInformation("C# HTTP trigger function processed a request.");

    //  string token = req.Headers["Authorization"].FirstOrDefault();

    //  if (string.IsNullOrEmpty(token))
    //  {
    //    return new UnauthorizedResult();
    //  }

    //  try
    //  {
    //    var handler = new JwtSecurityTokenHandler();
    //    var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

    //    var channelId = jsonToken?.Claims
    //        .First(claim => claim.Type == "iss")
    //        .Value;

    //    if (channelId != Environment.GetEnvironmentVariable("LINE_CHANNEL_ID"))
    //    {
    //      return new UnauthorizedResult();
    //    }
    //  }
    //  catch (Exception)
    //  {
    //    return new UnauthorizedResult();
    //  }

    //  return new OkObjectResult("This is a protected endpoint.");
    //}
  }
}

