using System;

namespace KailashcontractslambdaLambda.Models
{
    public class Response
    {
        public int? Id { get; set; }
        public string? ContractName { get; set; }
        public string? Email { get; set; }
        public ErrorDetail? Error { get; set; }

        public Response() { }
    }

    public class ErrorDetail
    {
        public string? Code { get; set; }
        public string? Message { get; set; }

        public ErrorDetail() { }
    }
}
