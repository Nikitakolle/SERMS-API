namespace SERMS.API.DTOs
{
    public class UpdateRoomDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Capacity { get; set; }
        public string Equipment { get; set; } = null!;
    }
}
