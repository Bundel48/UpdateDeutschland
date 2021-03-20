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
}