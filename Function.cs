using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using System.Text.Json;
using KailashcontractslambdaLambda.Models;
using KailashcontractslambdaLambda.Services;

[assembly: Amazon.Lambda.Core.LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace KailashcontractslambdaLambda
{
    public class Function
    {
        private readonly Service _service;

        public Function()
        {
            _service = new Service();
        }

        /// <summary>
        /// Lambda handler named 'kailashcontractslambda'
        /// </summary>
        /// <param name="request">Request object (not used for this operation)</param>
        /// <param name="context">Lambda context</param>
        /// <returns>Response containing the contract with the highest id, or structured error</returns>
        public async Task<Response> kailashcontractslambda(Request request, ILambdaContext context)
        {
            try
            {
                var result = await _service.GetContractWithHighestIdAsync();
                return result;
            }
            catch (Exception ex)
            {
                // Log and return structured error
                Console.Error.WriteLine(ex.ToString());
                return new Response
                {
                    Error = new ErrorDetail
                    {
                        Code = "UnhandledException",
                        Message = ex.Message
                    }
                };
            }
        }
    }
}
