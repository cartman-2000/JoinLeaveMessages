using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;

namespace JoinLeaveMessages
{
    public class JoinLeaveMessages : RocketPlugin<JoinLeaveMessagesConfig>
    {
        JoinLeaveMessages Instance;

        protected override void Load()
        {
            Instance = this;
            U.Events.OnPlayerConnected += Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected += Events_OnPlayerDisconnected;
        }

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList
                {
                    { "connect_message", "{0} connected to the server." },
                    { "disconnect_message", "{0} disconnected from the server." }
                };
            }
        }

        private void Events_OnPlayerConnected(UnturnedPlayer player)
        {
            if (Instance.Configuration.Instance.Enable)
            {
                UnturnedChat.Say(Translate("connect_message", player.CharacterName));
            }
        }

        private void Events_OnPlayerDisconnected(UnturnedPlayer player)
        {
            if (Instance.Configuration.Instance.Enable)
            {
                UnturnedChat.Say(Translate("disconnect_message", player.CharacterName));
            }
        }
    }
}
