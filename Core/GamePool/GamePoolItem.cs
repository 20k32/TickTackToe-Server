namespace Server.Core.GamePool
{
    public class GamePoolItem
    {
        public string UserId { get; init; }
        public string UserName { get; init; }

        public GamePoolItem(string userId, string userName) => (UserId, UserName) = (userId, userName);
    }
}
