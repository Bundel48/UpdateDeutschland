using SlimMessageBus;
using System.Collections.Generic;
using SlimMessageBus.Host.Config;
using System.Threading.Tasks;

namespace PluginInterface {
    
    public interface IPlugin
    {
        MessageBusBuilder initBus(MessageBusBuilder bus);

        Dictionary<System.Type, object> getHandlerResolvers();

        void setMessageBus(IMessageBus newBus);

        Task loop();
    }

    public class BaseResponse {
        public object payload;
    }

    public abstract class BaseMessage : IRequestMessage<BaseResponse> {
        public string command { get; set; }
        public object payload { get; set; }
        public string pluginName { get; set; }
    }

    public class BasicControlMessage : BaseMessage {
    }
    public class BasicFunctionalityBroadcast : BaseMessage {
    }
    public class BasicUiMessage : BaseMessage {

    }
}