using Rocket.API;
using Rocket.API.Collections;
using Rocket.API.Serialisation;
using Rocket.Core;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
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
            Instance.Configuration.Save();
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
                    { "disconnect_group_message", "{0}{1} disconnected to the server." },

                    { "connect_message_extended", "{0} [{1}] ({2}) connected to the server." },
                    { "disconnect_message_extended", "{0} [{1}] ({2}) disconnected from the server." },
                    { "connect_group_message_extended", "{0}{1} [{2}] ({3}) connected to the server." },
                    { "disconnect_group_message_extended", "{0}{1} [{2}] ({3}) disconnected to the server." }
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
                    float r;
                    float g;
                    float b;
                    string[] colors = color.Split(',');
                    return (colors.Length == 3 && float.TryParse(colors[0], out r) && float.TryParse(colors[1], out g) && float.TryParse(colors[2], out b) && r >= 0 && r <= 255 && g >= 0 && g <= 255 && b >= 0 && b <= 255) ? new Color( r/255, g/255, b/255 ) : Color.green;
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
                    if (!Instance.Configuration.Instance.ExtendedMessages)
                        UnturnedChat.Say(Translate(join ? "connect_group_message" : "disconnect_group_message", group != null ? group.DisplayName + ": " : "", player.CharacterName), join == true ? JoinMessageColor : LeaveMessageColor);
                    else
                    {
                        foreach (SteamPlayer SDGPlayer in Provider.Players)
                        {
                            if (SDGPlayer != null)
                            {
                                if (R.Permissions.HasPermission(new RocketPlayer(SDGPlayer.SteamPlayerID.CSteamID.ToString()), "jlm.extended") || SDGPlayer.IsAdmin)
                                    UnturnedChat.Say(SDGPlayer.SteamPlayerID.CSteamID, Translate( join ? "connect_group_message_extended" : "disconnect_group_message_extended", group != null ? group.DisplayName + ": " : "", player.CharacterName, player.SteamName, player.CSteamID.ToString() ), join == true ? JoinMessageColor : LeaveMessageColor);
                                else
                                    UnturnedChat.Say(SDGPlayer.SteamPlayerID.CSteamID, Translate( join ? "connect_group_message" : "disconnect_group_message", group != null ? group.DisplayName + ": " : "", player.CharacterName ), join == true ? JoinMessageColor : LeaveMessageColor);
                            }
                        }
                    }
                }
                else
                {
                    if (!Instance.Configuration.Instance.ExtendedMessages)
                        UnturnedChat.Say(Translate( join ? "connect_message" : "disconnect_message", player.CharacterName ), join == true ? JoinMessageColor : LeaveMessageColor);
                    else
                    {
                        foreach (SteamPlayer SDGPlayer in Provider.Players)
                        {
                            if (SDGPlayer != null)
                            {
                                if (R.Permissions.HasPermission(new RocketPlayer(SDGPlayer.SteamPlayerID.CSteamID.ToString()), "jlm.extended") || SDGPlayer.IsAdmin)
                                    UnturnedChat.Say(SDGPlayer.SteamPlayerID.CSteamID, Translate( join ? "connect_message_extended" : "disconnect_message_extended", player.CharacterName, player.SteamName, player.CSteamID.ToString() ), join == true ? JoinMessageColor : LeaveMessageColor);
                                else
                                    UnturnedChat.Say(SDGPlayer.SteamPlayerID.CSteamID, Translate( join ? "connect_message" : "disconnect_message", player.CharacterName ), join == true ? JoinMessageColor : LeaveMessageColor);
                            }
                        }
                    }
                }
            }
        }
    }
}
