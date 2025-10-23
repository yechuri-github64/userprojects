namespace GetaccountsLambda.Models;

#nullable enable
using System;

public class Request
{
    // No inputs required for this operation. Keep placeholder properties for future extensibility.
    public string? CorrelationId { get; init; }

    public Request()
    {
        CorrelationId = null;
    }
}
