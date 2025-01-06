namespace rifffinder.Models
{
    public class Musician
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; } 
        public required string Instrument { get; set; }
        public int? BandId { get; set; }
        public Band ?Band { get; set; }
    }

}
