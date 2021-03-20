using System;
using System.Threading.Tasks;
using NLog;
using SlimMessageBus;
using SlimMessageBus.Host.Config;
using System.Collections.Generic;
using PluginInterface;
//using static PluginInterface.BasicControlMessage;

namespace middleware {
    public class BasicInputPlugin : IPlugin {
        private Logger log = LogManager.GetCurrentClassLogger();
        private IMessageBus bus;


        public MessageBusBuilder initBus(MessageBusBuilder bus) {
            return bus
                .Produce<BasicControlMessage>(x => x.DefaultTopic("Controls"));
        }

        public Dictionary<System.Type, object> getHandlerResolvers() {
            Dictionary<System.Type, object> dict = new Dictionary<System.Type, object>();
            
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
}