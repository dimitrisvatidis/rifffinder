namespace rifffinder.Models
{
    public class Posting
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Text { get; set; }
        public required string InstrumentWanted { get; set; }
        public required int BandId { get; set; }
        public PostingStatus Status { get; set; } = PostingStatus.Open;
        public Band? Band { get; set; }
    }
}
