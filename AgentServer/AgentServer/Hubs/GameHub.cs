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
        private GameGrpcChannel ChannelService { get; }

        public GameHub(IHubContext<GameHub> hubContext, GameGrpcChannel channel)
        {
            _hubContext = hubContext;
            ChannelService = channel;
        }

        public override async Task OnConnectedAsync()
        {
            var _client = new Mailer.MailerClient(ChannelService.Channel);
            var _call = _client.Mailbox(headers: new Metadata { new Metadata.Entry("mailbox-name", "agent"), new Metadata.Entry("user-identifier", Context.UserIdentifier) });
            Context.Items.Add("_client", _client);
            Context.Items.Add("_call", _call);

            var CancellationToken = Context.ConnectionAborted;
            var ConnectionId = Context.ConnectionId;
            var HttpContext = Context.GetHttpContext();
            void abort()
            {
                if (!CancellationToken.IsCancellationRequested)
                {
                    HttpContext.Abort();
                }
            }

            var responseTask = Task.Run(async () =>
            {
                await foreach(var message in _call.ResponseStream.ReadAllAsync())
                {
                    var packet = new NetPacket { Id = message.Id, Content = message.Content.ToBase64(), Reserve = message.Reserve };
                    await _hubContext.Clients.Client(ConnectionId).SendAsync("StoCMessage", packet);
                    if (message.Id == 999999)
                    {
                        break;
                    }
                }

                abort();

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

        public async Task CtoSMessage(NetPacket packet)
        {
            var _call = Context.Items["_call"] as AsyncDuplexStreamingCall<ForwardMailMessage, MailboxMessage>;
            var forward = new ForwardMailMessage
            {
                Id = packet.Id,
                Content = ByteString.FromBase64(packet.Content),
                Reserve = packet.Reserve
            };
            await _call.RequestStream.WriteAsync(forward);
        }
    }
}
