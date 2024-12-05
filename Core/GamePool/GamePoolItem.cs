namespace Server.Core.GamePool
{
    internal sealed class GamePoolItem
    {
        public string UserId { get; init; }

        public GamePoolItem(string userId) => (UserId) = (userId);

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
