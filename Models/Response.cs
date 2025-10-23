namespace GetaccountsLambda.Models;

#nullable enable
using System;

public class Response
{
    public AccountModel? Account { get; init; }
    public ErrorModel? Error { get; init; }

    public Response()
    {
        Account = null;
        Error = null;
    }

    public class AccountModel
    {
        public int Id { get; init; }
        public string? Name { get; init; }
        public string? Email { get; init; }
        public DateTime? CreatedAt { get; init; }

        public AccountModel()
        {
            Id = 0;
            Name = null;
            Email = null;
            CreatedAt = null;
        }
    }

    public class ErrorModel
    {
        public string? Code { get; init; }
        public string? Message { get; init; }
        public string? Details { get; init; }

        public ErrorModel()
        {
            Code = null;
            Message = null;
            Details = null;
        }
    }
}
