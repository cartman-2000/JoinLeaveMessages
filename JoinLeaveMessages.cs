using Rocket.API.Collections;
using Rocket.Core;
using Rocket.Core.Plugins;
using Rocket.Core.Serialisation;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Linq;

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

        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected -= Events_OnPlayerDisconnected;
        }

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList
                {
                    { "connect_message", "{0} connected to the server." },
                    { "disconnect_message", "{0} disconnected from the server." },
                    { "connect_group_message", "{0}{1} connected to the server." },
                    { "disconnect_group_message", "{0}{1} disconnected to the server." }
                };
            }
        }

        private void Events_OnPlayerConnected(UnturnedPlayer player)
        {
            Message(player, true);
        }

        private void Events_OnPlayerDisconnected(UnturnedPlayer player)
        {
            Message(player, false);
        }

        private void Message(UnturnedPlayer player, bool join)
        {
            if (Instance.Configuration.Instance.Enable)
            {
                if ((R.Permissions.HasPermission(player, "jlm.group") || player.IsAdmin) && Instance.Configuration.Instance.GroupMessages)
                {
                    RocketPermissionsGroup group = R.Permissions.GetGroups(player, false).FirstOrDefault();
                    UnturnedChat.Say(Translate(join ? "connect_group_message" : "disconnect_group_message", group != null ? group.DisplayName + ": " : "", player.CharacterName));
                }
                else
                {
                    UnturnedChat.Say(Translate(join ? "connect_message" : "disconnect_message", player.CharacterName));
                }
            }
        }
    }
}
