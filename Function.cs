using Amazon.Lambda.Core;
using System;
using NumberLambda.Models;
using NumberLambda.Services;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace NumberLambda
{
    public class Function
    {
        private readonly Service _service = new Service();

        /// <summary>
        /// Lambda handler named "number" that returns a random integer between 5 and 100 inclusive.
        /// Accepts a Request object and returns a Response object. Any errors are logged and returned
        /// in a structured ErrorResponse inside the Response.
        /// </summary>
        public Response number(Request request, ILambdaContext context)
        {
            try
            {
                // Log the incoming request
                context.Logger.LogLine("Received request: " + System.Text.Json.JsonSerializer.Serialize(request));

                // Generate number
                var value = _service.GenerateRandomNumber();

                var response = new Response
                {
                    Number = value,
                    Message = "Random number generated successfully.",
                    Error = null
                };

                context.Logger.LogLine("Response: " + System.Text.Json.JsonSerializer.Serialize(response));
                return response;
            }
            catch (Exception ex)
            {
                // Log exception
                context.Logger.LogLine("Error: " + ex.ToString());

                // Return structured error
                return new Response
                {
                    Number = null,
                    Message = null,
                    Error = new ErrorResponse
                    {
                        Code = "InternalError",
                        Message = "An internal error occurred while generating the number.",
                        Details = ex.Message
                    }
                };
            }
        }
    }
}
