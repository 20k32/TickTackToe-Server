using MongoDB.Driver;

namespace Server.Persistence.MongoDb
{
    public static class AsyncCursorExtensions
    {
        public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IAsyncCursor<T> cursor)
        {
            while (await cursor.MoveNextAsync())
            {
                foreach (var item in cursor.Current)
                {
                    yield return item;
                }
            }
        }
    }
}
