// using Microsoft.Extensions.Hosting;
// using SharedCore.Messaging.Core.Receiving;
//
// namespace SharedCore.Messaging.Infrastructure;
//
// public class ReceiverHostedService : IHostedService
// {
//     private readonly IReceiver _receiver;
//
//     public ReceiverHostedService(IReceiver receiver)
//     {
//         _receiver = receiver;
//     }
//     
//     public async Task StartAsync(CancellationToken cancellationToken)
//     {
//         await _receiver.StartReceiving(cancellationToken);
//     }
//
//     public async Task StopAsync(CancellationToken cancellationToken)
//     {
//         await _receiver.StopReceiving(cancellationToken);
//     }
// }