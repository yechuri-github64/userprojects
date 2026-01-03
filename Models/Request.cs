using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TestLambda.Models
{
    public class Request
    {
        public Request()
        {
            TeamName = string.Empty;
            Players = new List<PlayerGoal>();
        }

        public string TeamName { get; set; }

        [JsonConverter(typeof(ExactCaseJsonStringEnumConverter))]
        public TeamCategory Category { get; set; } = TeamCategory.Professional;

        public List<PlayerGoal> Players { get; set; }
    }

    public class PlayerGoal
    {
        public PlayerGoal()
        {
            PlayerId = 0;
            Goals = 0;
        }

        public int PlayerId { get; set; }
        public int Goals { get; set; }
    }
}
