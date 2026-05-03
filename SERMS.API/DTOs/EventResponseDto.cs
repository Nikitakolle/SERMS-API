namespace SERMS.API.DTOs
{
    public class EventResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string RoomName { get; set; } = null!;
        public string OrganizerName { get; set; } = null!;
        public int ParticipantCount { get; set; }
    }
}
