using Core;
namespace RingoMediaTask
{
    public class ReminderService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public ReminderService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var reminders = context.Reminders
                                                .Where(r => r.ReminderDateTime <= DateTime.Now && !r.IsSent)
                                                .ToList();

                    foreach (var reminder in reminders)
                    {
                        // Send email logic
                        reminder.IsSent = true;
                        context.Update(reminder);
                    }

                    await context.SaveChangesAsync();
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }

}
