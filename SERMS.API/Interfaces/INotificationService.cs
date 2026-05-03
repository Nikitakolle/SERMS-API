namespace SERMS.API.Interfaces
{
    public interface INotificationService
    {
            Task SendEventReminder(int eventId);
        
    }
}

