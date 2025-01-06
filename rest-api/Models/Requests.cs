using rifffinder.Models;
using System.Text.Json.Serialization;

public class Request
{
    public int Id { get; set; }

    [JsonIgnore]
    public  int MusicianId { get; set; }
    public int BandId { get; set; }

    public int? PostingId { get; set; }
    public RequestStatus Status { get; set; } = RequestStatus.Pending; 
}