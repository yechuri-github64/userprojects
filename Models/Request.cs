using System.Text.Json;

namespace ManageordersLambda.Models
{
    public class Request
    {
        public Request()
        {
            Action = null!;
            OrderId = null!;
            Data = null;
        }

        /// <summary>
        /// "retrieve" or "update"
        /// </summary>
        public string? Action { get; set; }

        /// <summary>
        /// Salesforce Order Id (e.g., 18-character Id)
        /// </summary>
        public string? OrderId { get; set; }

        /// <summary>
        /// JSON payload for update operation
        /// </summary>
        public JsonElement? Data { get; set; }
    }
}
