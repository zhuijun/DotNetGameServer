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
        private readonly IHubContext<GameHub> _hubContext;

        public GameHub(IHubContext<GameHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public override async Task OnConnectedAsync()
        {
            var _channel = GrpcChannel.ForAddress("https://localhost:5005");
            var _client = new Mailer.MailerClient(_channel);
            var _call = _client.Mailbox(headers: new Metadata { new Metadata.Entry("mailbox-name", "agent") });
            Context.Items.Add("_channel", _channel);
            Context.Items.Add("_client", _client);
            Context.Items.Add("_call", _call);

            var ConnectionId = Context.ConnectionId;
            var responseTask = Task.Run(async () =>
            {
                await foreach(var message in _call.ResponseStream.ReadAllAsync())
                {
                    Console.ForegroundColor = message.Reason == MailboxMessage.Types.Reason.Received ? ConsoleColor.White : ConsoleColor.Green;
                    Console.WriteLine();
                    Console.WriteLine(message.Reason == MailboxMessage.Types.Reason.Received ? "Mail received" : "Mail forwarded");
                    Console.WriteLine($"New mail: {message.New}, Forwarded mail: {message.Forwarded}");
                    Console.ResetColor();

                    await _hubContext.Clients.Client(ConnectionId).SendAsync("StoCMessage", message.ToString());
                }
                Console.WriteLine("!!!end");
            });
            Context.Items.Add("_task", responseTask);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var _call = Context.Items["_call"] as AsyncDuplexStreamingCall<ForwardMailMessage, MailboxMessage>;
            await _call.RequestStream.CompleteAsync();

            var responseTask = Context.Items["_task"] as Task;
            await responseTask;

            await base.OnDisconnectedAsync(exception);
        }

        public async Task CtoSMessage(string message)
        {
            var _call = Context.Items["_call"] as AsyncDuplexStreamingCall<ForwardMailMessage, MailboxMessage>;
            var forward = new ForwardMailMessage
            {
                ConnectionId = Context.ConnectionId
            };
            await _call.RequestStream.WriteAsync(forward);
            //return Clients.All.SendAsync("StoCMessage", message);
        }
    }
}
