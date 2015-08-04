using Rocket.API;

namespace JoinLeaveMessages
{
    public class JoinLeaveMessagesConfig : IRocketPluginConfiguration
    {
        public bool Enable;
        public void LoadDefaults()
        {
            Enable = true;
        }
    }
}