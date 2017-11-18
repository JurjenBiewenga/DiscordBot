namespace DiscordBot
{
    [System.Serializable]
    public class InviteRoleLink
    {
        public string roleName;
        public string inviteCode;
        public int inviteUsageCount;
        public ulong roleId;

        public InviteRoleLink(string roleName, string inviteCode, int inviteUsageCount, ulong roleId)
        {
            this.roleName = roleName;
            this.inviteCode = inviteCode;
            this.inviteUsageCount = inviteUsageCount;
            this.roleId = roleId;
        }
    }
}