namespace GetaccountsLambda;

using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Microsoft.Extensions.Configuration;
using System.IO;
using GetaccountsLambda.Services;
using GetaccountsLambda.Models;
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
public class Function
{
    private readonly Service _service;

    public Function()
    {
        // Build configuration from appsettings.json and environment variables.
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables();

        var config = builder.Build();
        _service = new Service(config);
    }

    // Lambda handler must be named "getaccounts"
    public async Task<Response> getaccounts(Request? request, ILambdaContext context)
    {
        // Request may be null or unused; delegate to service
        return await _service.GetLastAccountAsync(context);
    }
}
