using AgentServer.Services;
using Google.Protobuf;
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
        private GrpcChannelService ChannelService { get; }

        public GameHub(IHubContext<GameHub> hubContext, GrpcChannelService channel)
        {
            _hubContext = hubContext;
            ChannelService = channel;
        }

        public override async Task OnConnectedAsync()
        {
            var _client = new Mailer.MailerClient(ChannelService.Channel);
            var _call = _client.Mailbox(headers: new Metadata { new Metadata.Entry("mailbox-name", "agent") });
            Context.Items.Add("_client", _client);
            Context.Items.Add("_call", _call);

            var ConnectionId = Context.ConnectionId;
            var responseTask = Task.Run(async () =>
            {
                await foreach(var message in _call.ResponseStream.ReadAllAsync())
                {
                    await _hubContext.Clients.Client(ConnectionId).SendAsync("StoCMessage", message.Id, message.Content.ToBase64());
                    if (message.Id == 999999)
                    {
                        break;
                    }
                }

                if (!Context.ConnectionAborted.IsCancellationRequested)
                {
                    Context.Abort();
                }

                //Console.ForegroundColor = ConsoleColor.Green;
                //Console.WriteLine("!!!end");
                //Console.ResetColor();
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

        public async Task CtoSMessage(int id, string message)
        {
            var _call = Context.Items["_call"] as AsyncDuplexStreamingCall<ForwardMailMessage, MailboxMessage>;
            var forward = new ForwardMailMessage
            {
                Id = id,
                Content = ByteString.FromBase64(message)
            };
            await _call.RequestStream.WriteAsync(forward);
            //return Clients.All.SendAsync("StoCMessage", message);
        }
    }
}
