namespace DiscordBot
{
    [System.Serializable]
    public class InviteRoleLink
    {
        public string inviteCode;
        public int inviteUsageCount;
        public ulong roleId;

        public InviteRoleLink(string inviteCode, int inviteUsageCount, ulong roleId)
        {
            this.inviteCode = inviteCode;
            this.inviteUsageCount = inviteUsageCount;
            this.roleId = roleId;
        }
    }
}