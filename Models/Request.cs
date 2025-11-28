using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text.Json.Serialization;

namespace TestfromdocspostgresqlLambda.Models
{
    public class Request
    {
        [JsonConverter(typeof(ExactCaseEnumConverter<RailcardType>))]
        public RailcardType railcardType { get; set; }

        public DateTime railcardValidFrom { get; set; }
        public DateTime railcardValidTo { get; set; }

        public string? railcardName { get; set; }

        public string railcardNumber { get; set; } = null!;

        public DateTime railcardRequestedDate { get; set; }

        public string railcardTransactionReference { get; set; } = null!;

        public DateTime? railcardUsableTo { get; set; }

        public List<Cardholder> cardholders { get; set; } = new List<Cardholder>();
    }

    public class Cardholder
    {
        public string cardholderTitle { get; set; } = null!;
        public string cardholderForename { get; set; } = null!;
        public string cardholderSurname { get; set; } = null!;

        [JsonConverter(typeof(ExactCaseEnumConverter<CardholderType>))]
        public CardholderType cardholderType { get; set; }

        public string cardholderPhotoName { get; set; } = null!;

        // One of these three must be provided
        public string? cardholderPhotoRRSKey { get; set; }
        public string? cardholderPhotoURL { get; set; }
        public string? cardholderPhotoKey { get; set; }
    }
}
