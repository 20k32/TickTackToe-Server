namespace Server.Persistence.Entity
{
    public sealed class GameHistory
    {
        public string TimeTaken { get; set; }
        public double PointsReceived { get; set; }
        public string OpponentName { get; set; }
    }
}
