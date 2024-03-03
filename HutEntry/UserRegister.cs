using System.Net;
using HutEntry.Data.Dtos;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace HutEntry
{
    public class UserRegister
    {
        private readonly ILogger _logger;

        public UserRegister(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<UserRegister>();
        }

        [Function("UserRegister")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            JsonSerializerOptions json = new()
            {
                PropertyNameCaseInsensitive = true
            };

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
