namespace rifffinder.DTOs
{
    public class BandDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Genre { get; set; }
        public required string Bio { get; set; }
    }
}