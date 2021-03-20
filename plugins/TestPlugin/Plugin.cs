using System;
using System.Threading.Tasks;
using NLog;
using SlimMessageBus;
using SlimMessageBus.Host.Config;
using System.Collections.Generic;

namespace middleware {
    class TestPlugin {
        private static Logger log = LogManager.GetCurrentClassLogger();
        private static IMessageBus bus;

        public static MessageBusBuilder initBus(MessageBusBuilder bus) {
            return bus
                .Produce<MessageRequest>(x => x.DefaultTopic("Send"))
                .Handle<MessageRequest, MessageResponse>(x => x.Topic("Send").WithHandler<Handler>())
                .ExpectRequestResponses(x => x.ReplyToTopic("Respond"));
        }

        public static Dictionary<System.Type, object> getHandlerResolvers() {
            Dictionary<System.Type, object> dict = new Dictionary<System.Type, object>();
            dict.Add(typeof(Handler), new Handler());

            return dict;
        }

        public static void setMessageBus(IMessageBus newBus) {
            bus = newBus;
        }

        public static async Task loop() {
            while (true) {
                Console.WriteLine("Senden");
                var response = await bus.Send(new MessageRequest("Hallo Welt!"));
                Console.WriteLine("response: "+response.getResponse());

                await Task.Delay(1000);
            }
        }
    }

    
    class Handler : IRequestHandler<MessageRequest, MessageResponse>{
        public async Task<MessageResponse> OnHandle(MessageRequest request, string name) {
            // handle the request message and return response
            await Task.Delay(100);
            Console.WriteLine("Hallo Welt!!"+request.getMessage());
            return new MessageResponse(request.getMessage() + " received.");
        }
    }

    
    class MessageRequest : IRequestMessage<MessageResponse> {
        public string message;
        public MessageRequest(string message) {
            this.message = message;
        }

        public string getMessage() {
            return this.message;
        }
    }

    class MessageResponse {
        public string response;
        public MessageResponse(string response) {
            this.response = response;
        }

        public string getResponse() {
            return this.response;
        }
    }
}