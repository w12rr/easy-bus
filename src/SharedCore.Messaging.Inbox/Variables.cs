namespace SharedCore.Messaging.Inbox;

public static class Variables
{
    public static string LockName(string definitionId, string correlationId) => $"{definitionId}-{correlationId}";
    public const int ConsumingErrorSleepTimeInSeconds = 10;
    public const int SleepTimeBetweenConsumingSessionsInMilliseconds = 500;
    public const int SleepTimeBetweenErrorConsumingSessionsInMilliseconds = 1000;
}