﻿namespace rifffinder.Models
{
    public class RequestWithMusicianDto
    {
        public int Id { get; set; }
        public int MusicianId { get; set; }
        public int BandId { get; set; }
        public int? PostingId { get; set; }
        public RequestStatus Status { get; set; }
    }
}
