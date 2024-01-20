using MediatR;

namespace SharedCore.Messaging.Core;

public sealed record EventWrapper<T>(T Content) : INotification, IRequest;