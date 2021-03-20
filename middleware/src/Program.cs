using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using NLog;
using SlimMessageBus;
using SlimMessageBus.Host.Serialization.Json;
using SlimMessageBus.Host.Config;
using SlimMessageBus.Host.Memory;
using SlimMessageBus.Host.DependencyResolver;
using PluginInterface;


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

        private static Dictionary<System.Type, object> handlerResolver = new Dictionary<System.Type, object>();
        private static LinkedList<IPlugin> plugins = new LinkedList<IPlugin>();
        public static void Main(string[] args)
        {
            string[] pluginFilesArray = 
                Directory.GetFiles("plugins/", "*.dll");
            ArrayList pluginFiles = new ArrayList(pluginFilesArray);
            pluginFiles.Add("../Plugins/BasicInputPlugin/build/net5.0/BasicInputPlugin.dll");
            pluginFiles.Add("../Plugins/MenuOutputPlugin/build/net5.0/MenuOutputPlugin.dll");
            // Use this line to add a plugin that is not within the plugin subfolder
            // dlls within the plugin folder are loaded automatically
            //pluginFiles.Add(@"D:\Dokumente\Programmieren\UpdateDeutschland\middleware\PluginSrc\BasicInputPlugin\build\net5.0\BasicInputPlugin.dll");
            //pluginFiles.Add(@"D:\Dokumente\Programmieren\UpdateDeutschland\middleware\PluginSrc\BasicOutputPlugin\build\net5.0\BasicOutputPlugin.dll");
            
            foreach (string pluginFile in pluginFiles) {
                var DLL = Assembly.LoadFile(Path.GetFullPath(pluginFile));
                Console.WriteLine("dll exported types: "+DLL.GetExportedTypes().Length);
                foreach (Type type in DLL.GetExportedTypes())
                {
                    try {
                        var c = Activator.CreateInstance(type);
                        IPlugin plugin = (IPlugin) c;
                        plugins.AddFirst(plugin);
                        Console.WriteLine("found plugin: "+type);
                    } catch (Exception e) {
                        if (! (e.GetType() == typeof(MissingMethodException))) {
                            Console.WriteLine(e.ToString());
                            Console.WriteLine("invalid plugin found: "+type);
                        } else {
                            Console.WriteLine("ignored invalid interface class");
                        }
                    }
                }
            } 
            
            
            
            MessageBusBuilder builder = MessageBusBuilder.Create()
                .WithSerializer(new JsonMessageSerializer())
                .WithDependencyResolver(new LookupDependencyResolver(type =>
                {
                    if (handlerResolver.ContainsKey(type)) {
                        return handlerResolver[type];
                    } else if (type == typeof(ILoggerFactory)) {
                        return null;
                    } else {
                        Console.WriteLine("other type!! "+type);
                    }
                    throw new InvalidOperationException();
                }));
            foreach (IPlugin plugin in plugins) {
                builder = plugin.initBus(builder);

                Dictionary<System.Type, object> pluginHandlers = plugin.getHandlerResolvers();
                foreach (KeyValuePair<System.Type, object> entry in pluginHandlers) {
                    if (handlerResolver.ContainsKey(entry.Key)) {
                        Console.WriteLine("ERROR, Handler Resolver already contained!");
                        throw new Exception("ERROR, Handler Resolver already contained!");
                    }

                    handlerResolver[entry.Key] = entry.Value;
                }
            }



            bus = builder
                .Do(builder => {
                    builder.WithProviderMemory(new MemoryMessageBusSettings{EnableMessageSerialization = true});
                })
                .Build();
            foreach (IPlugin plugin in plugins) {
                plugin.setMessageBus(bus);
            }
            
            foreach (IPlugin plugin in plugins) {
                var pluginTask = Task.Factory.StartNew(plugin.loop, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
            
            
            //TestPlugin.setMessageBus(bus);
            //var produceTask = Task.Factory.StartNew(TestPlugin.loop, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            //produceTask.Start();

            while(true) {
                Task.Delay(100);
            }
        }
    }

}
