using System;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using SlimMessageBus;
using SlimMessageBus.Host.Config;
using System.Collections.Generic;
using PluginInterface;
using WatsonWebsocket;
//using static PluginInterface.BasicControlMessage;

namespace middleware {
    public class MenuOutputPlugin : IPlugin {
        private Logger log = LogManager.GetCurrentClassLogger();
        private IMessageBus bus;
        WatsonWsServer socket = new WatsonWsServer("localhost", 8818, false);


        public MessageBusBuilder initBus(MessageBusBuilder bus) {
            return bus
                .Consume<BasicControlMessage>(x => x.Topic("Controls").WithConsumer<BasicControlMessageConsumer>());
        }

        public Dictionary<System.Type, object> getHandlerResolvers() {
            Dictionary<System.Type, object> dict = new Dictionary<System.Type, object>();
            dict[typeof(BasicControlMessageConsumer)] = new BasicControlMessageConsumer(this.socket);
            return dict;
        }

        public void setMessageBus(IMessageBus newBus) {
            bus = newBus;
        }
        
        public async Task loop() {
            while (true) {
                await Task.Delay(1000);
            }
        }
    }

    public class BasicControlMessageConsumer : IConsumer<BasicControlMessage> {
        WatsonWsServer socket;
        private List<string> lastClients = new List<string>();
        public BasicControlMessageConsumer(WatsonWsServer socket) {
            this.socket = socket;
            socket.ClientConnected += ClientConnected;
            socket.ClientDisconnected += ClientDisconnected;

            socket.Start();
        }

        void ClientConnected(object sender, ClientConnectedEventArgs args) {
            Console.WriteLine("Client connected: " + args.IpPort);
            lastClients.Add(args.IpPort);
        }

        void ClientDisconnected(object sender, ClientDisconnectedEventArgs args) {
            Console.WriteLine("Client connected: " + args.IpPort);
            lastClients.Remove(args.IpPort);
        }

        public async Task OnHandle(BasicControlMessage message, string name) {
            Console.WriteLine("received control command: "+message.command);
            if (lastClients.Count > 0) {
                foreach(string lastClient in lastClients) {
                    await socket.SendAsync(lastClient, message.command, CancellationToken.None);
                }
            } else {
                await Task.Delay(10);
            }
        }
    }
}