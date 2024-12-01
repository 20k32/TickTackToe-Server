namespace Server.Core.GamePool
{
    public static class DiExtensions
    {
        public static void AddGamePool(this IServiceCollection collection) 
            => collection.AddSingleton<GamePool>();
    }
}
