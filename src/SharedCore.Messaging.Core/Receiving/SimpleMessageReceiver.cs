﻿// using MediatR;
// using SharedCore.Messaging.Core.Definitions;
//
// namespace SharedCore.Messaging.Core.Receiving;
//
// public class SimpleMessageReceiver : IMessageReceiver
// {
//     private readonly IMediator _mediator;
//
//     public SimpleMessageReceiver(IMediator mediator)
//     {
//         _mediator = mediator;
//     }
//     
//     public async Task Receive(string message, string correlationId, string messageId, IEventReceiverDefinition definition, CancellationToken cancellationToken)
//     {
//         var notification = definition.GetNotification(message);
//         await _mediator.Publish(notification, cancellationToken);
//     }
// }