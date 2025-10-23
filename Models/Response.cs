using System;

namespace ContractmanagementemailLambda.Models
{
    public class Response
    {
        public ResponseItem? Data { get; set; }
        public ErrorResponse? Error { get; set; }

        public Response()
        {
            Data = null;
            Error = null;
        }

        public class ResponseItem
        {
            public int Id { get; set; }
            public string? ContractName { get; set; }
            public string? Email { get; set; }

            public ResponseItem()
            {
                ContractName = null!;
                Email = null!;
            }
        }

        public class ErrorResponse
        {
            public string? Code { get; set; }
            public string? Message { get; set; }
            public string? Details { get; set; }

            public ErrorResponse()
            {
                Code = null!;
                Message = null!;
                Details = null!;
            }
        }
    }
}
