
namespace EasyBus.Infrastructure.Services;

public class Clock : IClock
{
    public DateTimeOffset UtcOffsetNow => DateTimeOffset.UtcNow;
    public DateTime Now => DateTime.Now;
}