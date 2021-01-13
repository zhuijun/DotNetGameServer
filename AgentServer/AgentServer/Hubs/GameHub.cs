using Grpc.Core;
using Grpc.Net.Client;
using Mail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Mail.Mailer;

namespace AgentServer.Hubs
{
    [Authorize(Policy = "Game")]
    public class GameHub : Hub
    {
        private readonly GrpcChannel channel;
        private readonly MailerClient client;
        private readonly AsyncDuplexStreamingCall<ForwardMailMessage, MailboxMessage> call;
        private readonly Task responseTask;

        public GameHub()
        {
            channel = GrpcChannel.ForAddress("https://localhost:5005");
            client = new Mailer.MailerClient(channel);
            call = client.Mailbox(headers: new Metadata { new Metadata.Entry("mailbox-name", "agent") });

            responseTask = Task.Run(async () =>
            {
                await foreach (var message in call.ResponseStream.ReadAllAsync())
                {
                    Console.ForegroundColor = message.Reason == MailboxMessage.Types.Reason.Received ? ConsoleColor.White : ConsoleColor.Green;
                    Console.WriteLine();
                    Console.WriteLine(message.Reason == MailboxMessage.Types.Reason.Received ? "Mail received" : "Mail forwarded");
                    Console.WriteLine($"New mail: {message.New}, Forwarded mail: {message.Forwarded}");
                    Console.ResetColor();
                }
            });
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await call.RequestStream.CompleteAsync();
            await responseTask;

            await base.OnDisconnectedAsync(exception);
        }

        public async Task CtoSMessage(string message)
        {
                await call.RequestStream.WriteAsync(new ForwardMailMessage());
            //return Clients.All.SendAsync("StoCMessage", message);
        }
    }
}
