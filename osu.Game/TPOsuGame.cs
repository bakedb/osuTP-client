// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Configuration;
using osu.Game.Online;
using osu.Game.Online.API;

namespace osu.Game
{
    /// <summary>
    /// An osu! game instance that connects to our TP server.
    /// </summary>
    public partial class TPOsuGame : OsuGame
    {
        private readonly string[] args;

        public TPOsuGame(string[] args)
        {
            this.args = args;
        }

        public override EndpointConfiguration CreateEndpoints() => new TPEndpointConfiguration();

        protected override void LoadComplete()
        {
            base.LoadComplete();

            // Set up our TP API access after base initialization
            if (API is not TPAPIAccess)
            {
                System.Console.WriteLine("TPOsuGame: Replacing API with TPAPIAccess");
                API = new TPAPIAccess(LocalConfig);
                System.Console.WriteLine($"TPOsuGame: New API state after replacement: {API.State.Value}");
            }
        }
    }
}
