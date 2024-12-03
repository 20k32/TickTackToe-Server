namespace Server.Core.GamePool
{
    internal sealed class GamePoolItem
    {
        public string UserId { get; init; }
        public string UserName { get; init; }

        public GamePoolItem(string userId, string userName) => (UserId, UserName) = (userId, userName);

        public override bool Equals(object obj)
        {
            if(obj is GamePoolItem item)
            {
                return UserId.Equals(item.UserId);
            }

            return false;
        }
    }
}
