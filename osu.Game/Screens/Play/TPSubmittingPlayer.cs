// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using osu.Game.Online.API;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Online.Rooms;
using osu.Game.Scoring;

namespace osu.Game.Screens.Play
{
    /// <summary>
    /// A player instance which submits scores to our TP server using direct HTTP requests.
    /// </summary>
    public partial class TPSubmittingPlayer : SubmittingPlayer
    {
        private static readonly HttpClient httpClient = new HttpClient();

        protected TPSubmittingPlayer([CanBeNull] PlayerConfiguration configuration = null)
            : base(configuration)
        {
            System.Console.WriteLine("TPSubmittingPlayer: Player created - ready for direct score submission");
        }

        protected override APIRequest<APIScoreToken> CreateTokenRequest()
        {
            // TP server doesn't use tokens - skip entirely
            System.Console.WriteLine("TPSubmittingPlayer: Skipping token system for TP server");
            return null;
        }

        protected override APIRequest<MultiplayerScore> CreateSubmissionRequest(Score score, long token)
        {
            // TP server uses direct HTTP - skip osu! submission system
            System.Console.WriteLine("TPSubmittingPlayer: Skipping osu! submission system for TP server");
            return null;
        }

        protected override bool ShouldExitOnTokenRetrievalFailure(Exception exception)
        {
            // Never exit on token failure - TP server doesn't use tokens
            System.Console.WriteLine("TPSubmittingPlayer: Bypassing token failure check for TP server");
            return false;
        }

        protected override async Task PrepareScoreForResultsAsync(Score score)
        {
            await base.PrepareScoreForResultsAsync(score).ConfigureAwait(false);

            score.ScoreInfo.Date = DateTimeOffset.Now;

            // Submit score directly to TP server using HTTP
            System.Console.WriteLine($"TPSubmittingPlayer: Submitting score directly to TP server - beatmap {score.ScoreInfo.BeatmapInfo.OnlineID}, score {score.ScoreInfo.TotalScore}");

            await SubmitScoreDirectly(score).ConfigureAwait(false);
        }

        private async Task SubmitScoreDirectly(Score score)
        {
            try
            {
                var scoreData = new
                {
                    beatmap_id = score.ScoreInfo.BeatmapInfo.OnlineID,
                    user_id = 1, // Fixed user ID for TP server
                    username = "TPUser", // Fixed username for TP server
                    raw_score = score.ScoreInfo.TotalScore,
                    accuracy = score.ScoreInfo.Accuracy,
                    max_combo = score.ScoreInfo.MaxCombo,
                    mods = string.Join(",", score.ScoreInfo.Mods.Select(m => m.Acronym)),
                    rank = score.ScoreInfo.Rank.ToString(),
                    count_300 = 0,
                    count_100 = 0,
                    count_50 = 0,
                    count_miss = 0,
                    timestamp = DateTimeOffset.Now.ToUnixTimeSeconds()
                };

                var json = JsonConvert.SerializeObject(scoreData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                System.Console.WriteLine($"TPSubmittingPlayer: Sending to TP server: {json}");

                var response = await httpClient.PostAsync("http://localhost:8000/api/scores/submit", content).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    System.Console.WriteLine($"TPSubmittingPlayer: ✅ Score submission successful! Response: {responseContent}");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    System.Console.WriteLine($"TPSubmittingPlayer: ❌ Score submission failed: {response.StatusCode} - {errorContent}");
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine($"TPSubmittingPlayer: ⚠️ Score submission error: {e.Message}");
            }
        }
    }
}
