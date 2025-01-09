using rifffinder.Models;

namespace rifffinder.DTOs
{
    public class PostingDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string InstrumentWanted { get; set; }
        public int BandId { get; set; }
        public string BandName { get; set; }
        public PostingStatus Status { get; set; }
    }
}