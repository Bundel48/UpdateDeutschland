using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NLog;
using SlimMessageBus;
using SlimMessageBus.Host.Serialization.Json;
using SlimMessageBus.Host.Config;
using SlimMessageBus.Host.Memory;
using SlimMessageBus.Host.DependencyResolver;


namespace middleware
{
    class Program
    {
        private static Logger log = LogManager.GetCurrentClassLogger();
        private static IMessageBus bus = TestPlugin.initBus(MessageBusBuilder.Create())


            .WithSerializer(new JsonMessageSerializer())
            .WithDependencyResolver(new LookupDependencyResolver(type =>
            {
                Dictionary<System.Type, object> dict = TestPlugin.getHandlerResolvers();
                if (dict.ContainsKey(type)) {
                    return dict[type];
                }
                // Simulate a dependency container
                if (type == typeof(ILoggerFactory)) {
                    return null;
                } else {
                    Console.WriteLine("other type!! "+type);
                }
                throw new InvalidOperationException();
            }))
            .Do(builder => {
                builder.WithProviderMemory(new MemoryMessageBusSettings{EnableMessageSerialization = true});
            })
            .Build();
        public static void Main(string[] args)
        {
            TestPlugin.setMessageBus(bus);
            var produceTask = Task.Factory.StartNew(TestPlugin.loop, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            //produceTask.Start();

            while(true) {
                Task.Delay(100);
            }
        }
    }

}
