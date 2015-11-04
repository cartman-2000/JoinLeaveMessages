using Rocket.API.Collections;
using Rocket.API.Serialisation;
using Rocket.Core;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Linq;
using UnityEngine;

namespace JoinLeaveMessages
{
    public class JoinLeaveMessages : RocketPlugin<JoinLeaveMessagesConfig>
    {
        internal JoinLeaveMessages Instance;
        private Color JoinMessageColor;
        private Color LeaveMessageColor;

        protected override void Load()
        {
            Instance = this;
            if (Instance.Configuration.Instance.JoinMessageEnable)
            {
                JoinMessageColor = ParseColor(Instance.Configuration.Instance.JoinMessageColor);
                U.Events.OnPlayerConnected += Events_OnPlayerConnected;
            }
            if (Instance.Configuration.Instance.LeaveMessageEnable)
            {
                LeaveMessageColor = ParseColor(Instance.Configuration.Instance.LeaveMessageColor);
                U.Events.OnPlayerDisconnected += Events_OnPlayerDisconnected;
            }
        }

        protected override void Unload()
        {
            if (Instance.Configuration.Instance.JoinMessageEnable)
                U.Events.OnPlayerConnected -= Events_OnPlayerConnected;
            if (Instance.Configuration.Instance.LeaveMessageEnable)
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

        internal Color ParseColor(string color)
        {
            if (color == null)
                return Color.green;
            switch (color.Trim().ToLower())
            {
                case "black":
                    return Color.black;
                case "blue":
                    return Color.blue;
                case "cyan":
                    return Color.cyan;
                case "grey":
                    return Color.grey;
                case "green":
                    return Color.green;
                case "gray":
                    return Color.gray;
                case "magenta":
                    return Color.magenta;
                case "red":
                    return Color.red;
                case "white":
                    return Color.white;
                case "yellow":
                    return Color.yellow;
                case "gold":
                    return new Color(1.0f, 0.843137255f, 0f);
                default:
                    return Color.green;
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
            if (!R.Permissions.HasPermission(player, "jlm.vanish"))
            {
                if ((R.Permissions.HasPermission(player, "jlm.group") || player.IsAdmin) && Instance.Configuration.Instance.GroupMessages)
                {
                    RocketPermissionsGroup group = R.Permissions.GetGroups(player, false).FirstOrDefault();
                    UnturnedChat.Say(Translate(join ? "connect_group_message" : "disconnect_group_message", group != null ? group.DisplayName + ": " : "", player.CharacterName), join == true ? JoinMessageColor : LeaveMessageColor);
                }
                else
                {
                    UnturnedChat.Say(Translate(join ? "connect_message" : "disconnect_message", player.CharacterName), join == true ? JoinMessageColor : LeaveMessageColor);
                }
            }
        }
    }
}
