using System;
using System.Threading.Tasks;
using NLog;
using SlimMessageBus;
using SlimMessageBus.Host.Config;
using System.Collections.Generic;
using PluginInterface;
//using static PluginInterface.BasicControlMessage;

namespace middleware {
    public class BasicOutputPlugin : IPlugin {
        private Logger log = LogManager.GetCurrentClassLogger();
        private IMessageBus bus;


        public MessageBusBuilder initBus(MessageBusBuilder bus) {
            return bus
                .Consume<BasicControlMessage>(x => x.Topic("Controls").WithConsumer<BasicControlMessageConsumer>());
        }

        public Dictionary<System.Type, object> getHandlerResolvers() {
            Dictionary<System.Type, object> dict = new Dictionary<System.Type, object>();
            dict[typeof(BasicControlMessageConsumer)] = new BasicControlMessageConsumer();
            return dict;
        }

        public void setMessageBus(IMessageBus newBus) {
            bus = newBus;
        }
        
        public async Task loop() {
            while (true) {
                Console.WriteLine("Senden");
                await bus.Publish(new BasicControlMessage() { command = "GO UP!" });
                
                await Task.Delay(1000);
            }
        }
    }

    public class BasicControlMessageConsumer : IConsumer<BasicControlMessage> {
        public async Task OnHandle(BasicControlMessage message, string name) {
            Console.WriteLine("received control command: "+message.command);
            await Task.Delay(50);
        }
    }
}