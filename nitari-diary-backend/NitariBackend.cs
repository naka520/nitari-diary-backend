using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using nitari_diary_backend.Entity;
using nitari_diary_backend.Response;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Data.Tables;
using System.Net;
using nitari_diary_backend.Service;
using Azure;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace nitari_diary_backend
{
  public class NitariBackend
  {

    // Specific day's token metrics
    [FunctionName("GetDiaryAll")]
    [OpenApiOperation(operationId: "GetDiaryAll", tags: new[] { "Diary" }, Description = "Get Diary All Entity of Login User")]
    [OpenApiParameter(name: "userId", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The specific userId")]
    [OpenApiParameter(name: "Authorization", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "accessToken")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<DiaryResponse>), Description = "The OK response")]
    public static async Task<IActionResult> GetDailySpecificUser(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "diary/all")] HttpRequest req,
    [Table("DailyEntity", Connection = "MyStorage")] TableClient tableClient, ILogger log)
    {
      log.LogInformation($"/diary/all called");
      string userId = req.Query["userId"];

      // API Authrization
      string token = req.Headers["Authorization"].FirstOrDefault();

      var result = await AuthService.VerifyAccessToken(token);
      if (!result)
      {
        return new UnauthorizedResult();
      }

      // Get Diary All Entity of Login User
      var diaries = tableClient.Query<DiaryEntity>().Where(x => x.PartitionKey == userId).ToList();

      List<DiaryResponse> diaryResponses = new List<DiaryResponse>();

      foreach (var diary in diaries)
      {
        DiaryResponse diaryResponse = new DiaryResponse
        {
          UserId = diary.UserId,
          Date = diary.Date,
          Title = diary.Title,
          Description= diary.Description,
          CreatedAt = diary.CreatedAt,
        };
        diaryResponses.Add(diaryResponse);
      };
      return new OkObjectResult(diaryResponses);
    }

    // Funcitons Args
    public class DiaryRequest
    {
      public string UserId { get; set; }
      public List<TagDiary> TagDiaries { get; set; }
      public string Title { get; set; }
      public string Description { get; set; }

      public string Date { get; set; }
    }

    public class TagDiary
    {
      public string activity { get; set; }
      public string feeling { get; set; }
    }

    [FunctionName("PostDiary")]
    [OpenApiOperation(operationId: "PostDiary", tags: new[] { "Diary" }, Description = "Post Diary")]
    [OpenApiRequestBody("application/json", typeof(DiaryRequest), Required = true, Description = "Diary data")]
    [OpenApiParameter(name: "Authorization", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "accessToken")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response")]
    public static async Task<IActionResult> PostDailySpecificUser(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "diary")] HttpRequest req,
    [Table("DailyEntity", Connection = "MyStorage")] TableClient tableClient,
    ILogger log)
    {
      log.LogInformation($"[POST] /daily called");

      // API Authrization
      string token = req.Headers["Authorization"].FirstOrDefault();

      var result = await AuthService.VerifyAccessToken(token);
      if (!result)
      {
        return new UnauthorizedResult();
      }

      //Create DiaryEntity
      DiaryEntity diaryEntity = new DiaryEntity
      {
        PartitionKey = "",
        RowKey = "",
        // ather data members .....
      };

      // Insert DiaryEntity
      try
      {
        await tableClient.UpdateEntityAsync(diaryEntity, ETag.All, TableUpdateMode.Replace);
      }
      catch (RequestFailedException ex) when (ex.Status == (int)HttpStatusCode.NotFound)
      {
        await tableClient.AddEntityAsync(diaryEntity);
      }

      DiaryResponse diaryResponse = new DiaryResponse
      {
        UserId = diaryEntity.UserId,
        Date = diaryEntity.Date,
        Title = diaryEntity.Title,
        Description = diaryEntity.Description,
        CreatedAt = diaryEntity.CreatedAt,
      };

      return new OkObjectResult(diaryResponse);
    }

    //[FunctionName("Auth")]
    //[OpenApiOperation(operationId: "PostDiary", tags: new[] { "Diary" }, Description = "Post Diary")]
    //[OpenApiParameter(name: "Authorization", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "accessToken")]
    //public static async Task<IActionResult> Auth(
    //    [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
    //    ILogger log)
    //{
    //  log.LogInformation("C# HTTP trigger function processed a request.");

    //  // API Authrization
    //  string token = req.Headers["Authorization"].FirstOrDefault();
    //  var result = await AuthService.VerifyAccessToken(token);
    //  if (!result)
    //  {
    //    return new UnauthorizedResult();
    //  }

    //  return new OkObjectResult(result);
    //}
  }
}

