namespace RandomNumberGenerator.Models
{
 public class ErrorResponse
 {
 public ErrorDetail Error { get; set; } = new ErrorDetail();
 }

 public class ErrorDetail
 {
 public string Code { get; set; } = "internal_error";
 public string Message { get; set; } = string.Empty;
 public object Details { get; set; }
 }
}
