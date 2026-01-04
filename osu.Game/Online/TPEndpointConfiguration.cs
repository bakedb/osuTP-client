// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Game.Online
{
    public class TPEndpointConfiguration : EndpointConfiguration
    {
        public TPEndpointConfiguration()
        {
            // Connect to our local TP server
            WebsiteUrl = APIUrl = @"http://localhost:8000";
            APIClientSecret = @"tpserver-secret";
            APIClientID = "tpserver-client";
            SpectatorUrl = $@"{APIUrl}/signalr/spectator";
            MultiplayerUrl = $@"{APIUrl}/signalr/multiplayer";
            MetadataUrl = $@"{APIUrl}/signalr/metadata";
            BeatmapSubmissionServiceUrl = $@"{APIUrl}/beatmap-submission";
        }
    }
}
