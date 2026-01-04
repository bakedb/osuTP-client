// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using osu.Framework.Bindables;
using osu.Framework.Logging;
using osu.Game.Configuration;
using osu.Game.Online.API.Requests;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Scoring;
using osu.Game.Localisation;
using osu.Game.Online.Chat;
using osu.Game.Online.Notifications.WebSocket;
using osu.Game.Online;

namespace osu.Game.Online.API
{
    public class TPAPIAccess : IAPIProvider
    {
        private readonly OsuConfigManager config;
        private readonly string serverUrl = "http://localhost:8000";

        private readonly Bindable<APIState> state = new Bindable<APIState>(APIState.Online);
        private readonly Bindable<APIUser> localUser = new Bindable<APIUser>();
        private readonly LocalUserState localUserState;

        public IBindable<APIState> State => state;
        public IBindable<APIUser> LocalUser => localUser;
        public ILocalUserState LocalUserState => localUserState;
        public EndpointConfiguration Endpoints { get; } = new TPEndpointConfiguration();
        public int APIVersion => 20240103;
        public Exception LastLoginError { get; private set; }
        public SessionVerificationMethod? SessionVerificationMethod { get; private set; }

        public string ProvidedUsername { get; private set; }
        public bool IsLoggedIn => true; // Always return true for TP server - no login required

        public TPAPIAccess(OsuConfigManager config)
        {
            this.config = config;
            localUserState = new LocalUserState(this, config);

            // Set a default user for demo purposes
            localUser.Value = new APIUser
            {
                Id = 1,
                Username = "TPUser"
            };

            // Set the API to online state immediately
            state.Value = APIState.Online;
            System.Console.WriteLine("TPAPIAccess: Constructor - Setting API state to Online");

            // Auto-login for TP server
            Login("TPUser", "password");
        }

        public void Login(string username, string password)
        {
            ProvidedUsername = username;

            // For TP server, we'll create or get the user on first score submission
            // For now, just set a basic user state
            state.Value = APIState.Online;
            LastLoginError = null;

            // Create a basic user - the real user ID will come from the server
            localUser.Value = new APIUser
            {
                Id = username.GetHashCode(), // Simple hash for temporary ID
                Username = username
            };

            System.Console.WriteLine($"TPAPIAccess: Logged in as {username}");
        }

        public void Logout()
        {
            state.Value = APIState.Offline;
            localUser.Value = new APIUser();
        }

        public void Queue(APIRequest request)
        {
            // Execute requests immediately for TP server
            Task.Run(() => Perform(request));
        }

        public void Perform(APIRequest request)
        {
            try
            {
                request.AttachAPI(this);
                request.Perform();
            }
            catch (Exception e)
            {
                request.Fail(e);
            }
        }

        public Task PerformAsync(APIRequest request) =>
            Task.Factory.StartNew(() => Perform(request), TaskCreationOptions.LongRunning);

        void IAPIProvider.Schedule(Action action) => action();

        public string AccessToken => "tpserver-token";

        public Guid SessionIdentifier { get; } = Guid.NewGuid();

        public INotificationsClient NotificationsClient => null;
        public IChatClient GetChatClient() => null;
        public IHubClientConnector GetHubConnector(string clientName, string endpoint) => null;
        public Language Language => default;

        public RegistrationRequest.RegistrationRequestErrors CreateAccount(string email, string username, string password)
        {
            // Implementation for user creation on TP server
            return null;
        }

        public void AuthenticateSecondFactor(string code) { }
    }
}
