// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using osu.Framework.IO.Network;
using osu.Game.Online.API;
using osu.Game.Online.Rooms;
using osu.Game.Rulesets.Scoring;
using osu.Game.Scoring;

namespace osu.Game.Online.API.Requests
{
    public class TPSubmitScoreRequest : APIRequest<MultiplayerScore>
    {
        private readonly ScoreInfo scoreInfo;
        private readonly int beatmapId;
        private readonly int userId;

        public TPSubmitScoreRequest(ScoreInfo scoreInfo, int beatmapId, int userId)
        {
            this.scoreInfo = scoreInfo;
            this.beatmapId = beatmapId;
            this.userId = userId;
        }

        protected override WebRequest CreateWebRequest()
        {
            var req = base.CreateWebRequest();

            req.ContentType = "application/json";
            req.Method = HttpMethod.Post;
            req.Timeout = 30000;

            var scoreSubmission = new
            {
                user_id = userId,
                beatmap_id = beatmapId,
                raw_score = scoreInfo.TotalScore,
                accuracy = scoreInfo.Accuracy,
                count_300 = scoreInfo.Statistics.TryGetValue(HitResult.Great, out var count300) ? count300 : 0,
                count_100 = scoreInfo.Statistics.TryGetValue(HitResult.Good, out var count100) ? count100 : 0,
                count_50 = scoreInfo.Statistics.TryGetValue(HitResult.Meh, out var count50) ? count50 : 0,
                count_miss = scoreInfo.Statistics.TryGetValue(HitResult.Miss, out var countMiss) ? countMiss : 0,
                mods = string.Join(",", scoreInfo.Mods.Select(m => m.Acronym))
            };

            System.Console.WriteLine($"TPSubmitScoreRequest: Submitting to {Target} - {JsonConvert.SerializeObject(scoreSubmission)}");
            req.AddRaw(JsonConvert.SerializeObject(scoreSubmission));
            return req;
        }

        protected override string Target => "api/scores/submit";

        protected override void PostProcess()
        {
            base.PostProcess();

            if (CompletionState == APIRequestCompletionState.Completed)
            {
                System.Console.WriteLine($"TPSubmitScoreRequest: Score submission successful for user {userId}, beatmap {beatmapId}");
            }
            else
            {
                System.Console.WriteLine($"TPSubmitScoreRequest: Score submission failed for user {userId}, beatmap {beatmapId}");
            }
        }
    }
}
