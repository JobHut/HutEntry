using System.Net;
using HutEntry.Data.Dtos;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using HutEntry.Data;
using HutEntry.Models;
using Microsoft.AspNetCore.Identity;
using AutoMapper;

namespace HutEntry
{
    public class UserLogin
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly JsonSerializerOptions json;

        public UserLogin(
            ILoggerFactory loggerFactory, 
            UserManager<User> userManager, 
            IMapper mapper)
        {
            _logger = loggerFactory.CreateLogger<UserLogin>();
            _userManager = userManager;
            json = new()
            {
                PropertyNameCaseInsensitive = true
            };
            _mapper = mapper;
        }

        [Function("user-login")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            CreateUserDto createUserDto;
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                createUserDto = JsonSerializer.Deserialize<CreateUserDto>(
                    requestBody, json) ?? throw new InvalidOperationException();

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

                var user = _mapper.Map<User>(createUserDto);

                var result = await _userManager.CreateAsync(user, createUserDto.Password);

                if (!result.Succeeded)
                {
                    throw new Exception($"Error to register user!, {result.Errors}");                    
                }

                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "application/json");

                response.WriteString($"User {user.UserName} registered successfully!");

                return response;
            }
            catch (JsonException)
            {
                var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                badRequestResponse.WriteString("Error parsing request body.");
                return badRequestResponse;
            }            
        }
    }
}
