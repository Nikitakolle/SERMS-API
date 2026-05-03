using SERMS.API.Interfaces;

namespace SERMS.API.Services
{
    public class NotificationService : INotificationService
    {

        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ILogger<NotificationService> logger)
        {
            _logger = logger;
        }

        public Task SendEventReminder(int eventId)
        {
            _logger.LogInformation("Reminder: Event {EventId} is starting soon!", eventId);

            return Task.CompletedTask;
        }
    }
}