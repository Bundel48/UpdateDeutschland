using System;
using System.Reflection;
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
        //TestPlugin.initBus(MessageBusBuilder.Create())
        //TestPlugin.getHandlerResolvers()
        //TestPlugin.setMessageBus(bus)
        //TestPlugin.loop
        private static IMessageBus bus;
        private static string[] pluginMethods = {
            "initBus",
            "getHandlerResolvers",
            "setMessageBus",
            "loop"
        };
        public static void Main(string[] args)
        {
            var DLL = Assembly.LoadFile(@"D:\Dokumente\Programmieren\UpdateDeutschland\plugins\TestPlugin\bin\Debug\net5.0\Plugin.dll");
            Console.WriteLine("dll exported types: "+DLL.GetExportedTypes().Length);
            foreach (Type type in DLL.GetExportedTypes())
            {
                var c = Activator.CreateInstance(type);
                bool validPlugin = true;
                foreach (string method in pluginMethods) {
                    if (c.GetType().GetMethod(method) == null) {
                        validPlugin = false;
                    }
                }

                if (validPlugin) {
                    Console.WriteLine("found plugin: "+type);
                } else {
                    Console.WriteLine("invalid plugin found: "+type);
                }
            }
            
            MessageBusBuilder builder = MessageBusBuilder.Create()
                .WithSerializer(new JsonMessageSerializer())
                .WithDependencyResolver(new LookupDependencyResolver(type =>
                {
                    //Dictionary<System.Type, object> dict = TestPlugin.getHandlerResolvers();
                    //if (dict.ContainsKey(type)) {
                    //    return dict[type];
                    //}
                    // Simulate a dependency container
                    if (type == typeof(ILoggerFactory)) {
                        return null;
                    } else {
                        Console.WriteLine("other type!! "+type);
                    }
                    throw new InvalidOperationException();
                }));
            
            

            bus = builder
                .Do(builder => {
                    builder.WithProviderMemory(new MemoryMessageBusSettings{EnableMessageSerialization = true});
                })
                .Build();
            //TestPlugin.setMessageBus(bus);
            //var produceTask = Task.Factory.StartNew(TestPlugin.loop, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            //produceTask.Start();

            while(true) {
                Task.Delay(100);
            }
        }
    }

}
