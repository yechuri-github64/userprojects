using System;
using Amazon.Lambda.Core;
using NumberLambda.Models;
using NumberLambda.Services;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace NumberLambda
{
    public class Function
    {
        // Handler method must be named "number"
        public Response number(Request request, ILambdaContext context)
        {
            try
            {
                int min = 5;
                int max = 100;

                // Allow optional overrides from request but clamp to 5-100
                if (request?.Min is not null) min = Math.Max(5, request.Min.Value);
                if (request?.Max is not null) max = Math.Min(100, request.Max.Value);
                if (min > max)
                {
                    min = 5;
                    max = 100;
                }

                var value = Service.GenerateRandomNumber(min, max);

                return new Response
                {
                    Success = true,
                    Number = value
                };
            }
            catch (Exception ex)
            {
                context?.Logger.LogLine($"Error in number handler: {ex}");

                return new Response
                {
                    Success = false,
                    Error = new Response.ErrorDetail
                    {
                        Code = "InternalError",
                        Message = "An unexpected error occurred while generating the number.",
                        Details = ex.Message
                    }
                };
            }
        }
    }
}