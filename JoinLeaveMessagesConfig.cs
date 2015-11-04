using Rocket.API;

namespace JoinLeaveMessages
{
    public class JoinLeaveMessagesConfig : IRocketPluginConfiguration
    {

        public bool JoinMessageEnable;
        public bool LeaveMessageEnable;
        public string JoinMessageColor;
        public string LeaveMessageColor;
        public bool GroupMessages;
        public void LoadDefaults()
        {
            JoinMessageEnable = true;
            LeaveMessageEnable = true;
            JoinMessageColor = "green";
            LeaveMessageColor = "green";
            GroupMessages = false;
        }
    }
}