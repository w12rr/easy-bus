namespace EasyBus.Infrastructure.Services;

public interface IClock 
{
    public DateTimeOffset UtcOffsetNow { get; }
    public DateTime Now { get; }
}