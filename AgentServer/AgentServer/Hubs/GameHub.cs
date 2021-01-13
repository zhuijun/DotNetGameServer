using Grpc.Core;
using Grpc.Net.Client;
using Mail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgentServer.Hubs
{
    [Authorize(Policy = "Game")]
    public class GameHub : Hub
    {
        private readonly GrpcChannel _channel;
        private readonly Mailer.MailerClient _client;
        private readonly AsyncDuplexStreamingCall<ForwardMailMessage, MailboxMessage> _call;
        private readonly Task _responseTask;
        private readonly IHubContext<GameHub> _hubContext;

        public GameHub(IHubContext<GameHub> hubContext)
        {
            _channel = GrpcChannel.ForAddress("https://localhost:5005");
            _client = new Mailer.MailerClient(_channel);
            _call = _client.Mailbox(headers: new Metadata { new Metadata.Entry("mailbox-name", "agent") });

            _hubContext = hubContext;
            _responseTask = Task.Run(ResponseTask);
        }

        private async Task ResponseTask()
        {
            await foreach (var message in _call.ResponseStream.ReadAllAsync())
            {
                Console.ForegroundColor = message.Reason == MailboxMessage.Types.Reason.Received ? ConsoleColor.White : ConsoleColor.Green;
                Console.WriteLine();
                Console.WriteLine(message.Reason == MailboxMessage.Types.Reason.Received ? "Mail received" : "Mail forwarded");
                Console.WriteLine($"New mail: {message.New}, Forwarded mail: {message.Forwarded}");
                Console.ResetColor();

                await _hubContext.Clients.All.SendAsync("StoCMessage", message.ToString());
            }
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await _call.RequestStream.CompleteAsync();
            await _responseTask;

            await base.OnDisconnectedAsync(exception);
        }

        public async Task CtoSMessage(string message)
        {
            await _call.RequestStream.WriteAsync(new ForwardMailMessage());
            //return Clients.All.SendAsync("StoCMessage", message);
        }
    }
}
