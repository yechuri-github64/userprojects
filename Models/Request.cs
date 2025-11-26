using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace CreaterailcardLambda.Models
{
    public class Request
    {
        [JsonPropertyName("railcardType")]
        public RailcardType RailcardType { get; set; }

        [JsonPropertyName("railcardValidFrom")]
        public DateTime RailcardValidFrom { get; set; }

        [JsonPropertyName("railcardValidTo")]
        public DateTime RailcardValidTo { get; set; }

        [JsonPropertyName("railcardName")]
        public string? RailcardName { get; set; }

        [JsonPropertyName("railcardNumber")]
        public string RailcardNumber { get; set; } = null!;

        [JsonPropertyName("railcardRequestedDate")]
        public DateTime RailcardRequestedDate { get; set; }

        [JsonPropertyName("railcardTransactionReference")]
        public string RailcardTransactionReference { get; set; } = null!;

        [JsonPropertyName("railcardUsableTo")]
        public DateTime? RailcardUsableTo { get; set; }

        [JsonPropertyName("cardholders")]
        public List<Cardholder> Cardholders { get; set; } = new List<Cardholder>();
    }

    public class Cardholder
    {
        [JsonPropertyName("cardholderTitle")]
        public string CardholderTitle { get; set; } = null!;

        [JsonPropertyName("cardholderForename")]
        public string CardholderForename { get; set; } = null!;

        [JsonPropertyName("cardholderSurname")]
        public string CardholderSurname { get; set; } = null!;

        [JsonPropertyName("cardholderType")]
        public CardholderType CardholderType { get; set; }

        [JsonPropertyName("cardholderPhotoName")]
        public string CardholderPhotoName { get; set; } = null!;

        [JsonPropertyName("cardholderPhotoRRSKey")]
        public string? CardholderPhotoRRSKey { get; set; }

        [JsonPropertyName("cardholderPhotoURL")]
        public string? CardholderPhotoURL { get; set; }

        [JsonPropertyName("cardholderPhotoKey")]
        public string? CardholderPhotoKey { get; set; }
    }
}
