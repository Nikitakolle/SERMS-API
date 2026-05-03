namespace SERMS.API.DTOs
{
    public class CreateRoomDto
    {

        public string Name { get; set; } = null!;
        public int Capacity { get; set; }
        public string Equipment { get; set; } = null!;
    }
}
