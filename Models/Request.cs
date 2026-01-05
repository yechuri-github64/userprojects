using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using DemotestprojLambda.Models;

#nullable enable

namespace DemotestprojLambda.Models
{
    public class Request
    {
        public Request()
        {
            cardholders = new List<Cardholder>();
            travelcardName = null;
            travelcardUsableTo = null;
        }

        [JsonPropertyName("travelcardType")]
        public TravelcardType travelcardType { get; set; }

        [JsonPropertyName("travelcardValidFrom")]
        public DateTimeOffset travelcardValidFrom { get; set; }

        [JsonPropertyName("travelcardValidTo")]
        public DateTimeOffset travelcardValidTo { get; set; }

        [JsonPropertyName("travelcardName")]
        public string? travelcardName { get; set; }

        [JsonPropertyName("travelcardNumber")]
        public string travelcardNumber { get; set; } = null!;

        [JsonPropertyName("travelcardRequestedDate")]
        public DateTimeOffset travelcardRequestedDate { get; set; }

        [JsonPropertyName("travelcardTransactionReference")]
        public string travelcardTransactionReference { get; set; } = null!;

        [JsonPropertyName("travelcardUsableTo")]
        public DateTimeOffset? travelcardUsableTo { get; set; }

        [JsonPropertyName("cardholders")]
        public List<Cardholder> cardholders { get; set; }
    }

    public class Cardholder
    {
        public Cardholder()
        {
            cardholderPhotoRRSKey = null;
            cardholderPhotoURL = null;
            cardholderPhotoKey = null;
        }

        [JsonPropertyName("cardholderTitle")]
        public string cardholderTitle { get; set; } = null!;

        [JsonPropertyName("cardholderForename")]
        public string cardholderForename { get; set; } = null!;

        [JsonPropertyName("cardholderSurname")]
        public string cardholderSurname { get; set; } = null!;

        [JsonPropertyName("cardholderType")]
        public CardholderType cardholderType { get; set; }

        [JsonPropertyName("cardholderPhotoName")]
        public string cardholderPhotoName { get; set; } = null!;

        [JsonPropertyName("cardholderPhotoRRSKey")]
        public string? cardholderPhotoRRSKey { get; set; }

        [JsonPropertyName("cardholderPhotoURL")]
        public string? cardholderPhotoURL { get; set; }

        [JsonPropertyName("cardholderPhotoKey")]
        public string? cardholderPhotoKey { get; set; }
    }
}
