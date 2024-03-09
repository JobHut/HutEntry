using System.Net;
using HutEntry.Data.Dtos;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using HutEntry.Data;
using HutEntry.Models;

namespace HutEntry
{
    public class UserRegister
    {
        private readonly ILogger _logger;
        private readonly UserDbContext _userDbContext;
        private readonly JsonSerializerOptions json;

        public UserRegister(ILoggerFactory loggerFactory, UserDbContext userDbContext)
        {
            _logger = loggerFactory.CreateLogger<UserRegister>();
            _userDbContext = userDbContext;
            json = new()
            {
                PropertyNameCaseInsensitive = true
            };
        }

        [Function("user-register")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request."); 
            
            CreateUserDto createUserDto;
            try
            {
                string requestBody =  await new StreamReader(req.Body).ReadToEndAsync();
                createUserDto = JsonSerializer.Deserialize<CreateUserDto>(
                    requestBody, json ) ?? throw new InvalidOperationException();

                if (createUserDto == null)
                {
                    var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    badRequestResponse.WriteString("Invalid user data provided.");
                    return badRequestResponse;
                }

                if (string.IsNullOrEmpty(createUserDto.Username) || string.IsNullOrEmpty(createUserDto.Password))
                {
                    var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    badRequestResponse.WriteString("Invalid user data provided.");
                    return badRequestResponse;
                }

                _userDbContext.Users.Add(new User
                {
                    UserName = createUserDto.Username,
                    PasswordHash = createUserDto.Password
                });
                await _userDbContext.SaveChangesAsync();
            }
            catch (JsonException)
            {
                var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                badRequestResponse.WriteString("Error parsing request body.");
                return badRequestResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json");

            // Supondo que você faça algum processamento aqui e retorne o resultado...
            response.WriteString($"User {createUserDto.Username} registered successfully!");

            return response;
        }
    }
}
