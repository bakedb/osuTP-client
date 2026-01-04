// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Game.Screens.Select;
using osu.Game.Screens.Select.Leaderboards;

namespace osu.Game.Screens.Play
{
    /// <summary>
    /// A solo player instance that submits scores to our TP server.
    /// </summary>
    public partial class TPSoloPlayer : TPSubmittingPlayer
    {
        [Cached(typeof(IGameplayLeaderboardProvider))]
        private readonly TPGameplayLeaderboardProvider leaderboardProvider = new TPGameplayLeaderboardProvider();

        public TPSoloPlayer([CanBeNull] PlayerConfiguration configuration = null)
            : base(configuration)
        {
            System.Console.WriteLine("TPSoloPlayer: Creating TP solo player instance");
            Configuration.ShowLeaderboard = true;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            // Don't add the leaderboard provider as internal - it's just for dependency injection
        }
    }
}
