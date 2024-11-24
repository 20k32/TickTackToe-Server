using MongoDB.Bson;

namespace Server.Models.Api.Result
{
    public sealed class SuccessLoginResult
    {
        public string UserName { get; init; }
        public ObjectId Id { get; init; }

        public SuccessLoginResult(ObjectId id, string userName)
            => (Id, UserName) = (id, userName);
    }
}
