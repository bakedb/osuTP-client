// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Scoring;

namespace osu.Game.Online.API.Requests
{
    public class TPGetLeaderboardRequest : APIRequest<List<TPLeaderboardEntry>>
    {
        private readonly int beatmapId;
        private readonly int limit;

        public TPGetLeaderboardRequest(int beatmapId, int limit = 50)
        {
            this.beatmapId = beatmapId;
            this.limit = limit;
        }

        protected override string Target => $"api/leaderboards/beatmap/{beatmapId}?limit={limit}";
    }

    public class TPLeaderboardEntry
    {
        [JsonProperty("rank")]
        public int Rank { get; set; }

        [JsonProperty("user_id")]
        public int UserId { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; } = string.Empty;

        [JsonProperty("final_value")]
        public double FinalValue { get; set; }

        [JsonProperty("timestamp")]
        public System.DateTime Timestamp { get; set; }

        public APIUserScoreAggregate WithPosition()
        {
            return new APIUserScoreAggregate
            {
                User = new APIUser
                {
                    Id = UserId,
                    Username = Username
                },
                TotalScore = (long)FinalValue,
                Accuracy = (float)(FinalValue / 1000000), // Rough approximation
                Position = Rank
            };
        }
    }
}
