namespace Server.Models.Exceptions
{
    public class FoundUserInDbException : Exception
    {
        public FoundUserInDbException() : base("There is such user in database")
        { }
    }
}
