using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using SlimMessageBus;
using SlimMessageBus.Host.Config;
using System.Collections.Generic;
using PluginInterface;

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;


//using static PluginInterface.BasicControlMessage;

namespace middleware {
    public class SensorNodeInputPlugin : IPlugin {
        private Logger log = LogManager.GetCurrentClassLogger();
        private IMessageBus bus;
        private IMqttClient mqttClient;
        

        public SensorNodeInputPlugin() {
            var optionsBuilder = new MqttClientOptionsBuilder()
                .WithTcpServer("135.125.216.231", 1883)
                .WithCredentials("recomo", "Recomo123%");
            var mqttOptions = optionsBuilder.Build();
            mqttClient = new MqttFactory().CreateMqttClient();
            mqttClient.UseApplicationMessageReceivedHandler ( e => { 
                HandleMessageReceived(e.ApplicationMessage); 
            });
            mqttClient.UseConnectedHandler ( e => { 
                Console.WriteLine("CONNECTED!"+e);    
                mqttClient.SubscribeAsync("hand/Move");
            });
            mqttClient.ConnectAsync(mqttOptions, CancellationToken.None);
        }


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
            
            await Task.FromResult(0);
            /*while (true) {
                Console.WriteLine("Senden");
                await bus.Publish(new BasicControlMessage() { command = "OK" });
                
                Thread.Sleep(1000);
            }*/
        }

        
        private void HandleMessageReceived(MqttApplicationMessage applicationMessage)
        {
            Console.WriteLine("MQTT DATA: "+applicationMessage.Payload);
            var payload = "";
            var applicationPayload = Encoding.UTF8.GetString(applicationMessage.Payload);
            if (applicationPayload == "UP" || applicationPayload == "DOWN" ) {
                payload = applicationPayload;
            } else if (applicationPayload == "RIGHT") {
                payload = "OK";
            }
            if (payload != "") {
                bus.Publish(new BasicControlMessage() { command = payload });
            }
        }
    }
}