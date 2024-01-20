
using SharedCore.Abstraction.Services;

namespace SharedCore.Infrastructure.Services;

public class Clock : IClock
{
    public DateTimeOffset UtcOffsetNow => DateTimeOffset.UtcNow;
    public DateTime Now => DateTime.Now;
}