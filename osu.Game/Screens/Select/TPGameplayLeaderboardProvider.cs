// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Game.Beatmaps;
using osu.Game.Online.API;
using osu.Game.Screens.Select.Leaderboards;

namespace osu.Game.Screens.Select
{
    /// <summary>
    /// Simple leaderboard provider for TP server - disabled for now.
    /// Web panel will be added later for viewing scores.
    /// </summary>
    public class TPGameplayLeaderboardProvider : IGameplayLeaderboardProvider
    {
        [Resolved]
        private IAPIProvider api { get; set; }

        public IBindableList<GameplayLeaderboardScore> Scores { get; } = new BindableList<GameplayLeaderboardScore>();

        public Task<GameplayLeaderboardScore[]> GetScoresAsync(BeatmapInfo beatmapInfo, CancellationToken cancellationToken = default)
        {
            // Leaderboard disabled - scores are submitted directly to TP server
            // Web panel will be added later for viewing scores
            System.Console.WriteLine($"TPGameplayLeaderboardProvider: Leaderboard disabled for beatmap {beatmapInfo.OnlineID} - use web panel instead");
            return Task.FromResult(Array.Empty<GameplayLeaderboardScore>());
        }
    }
}
