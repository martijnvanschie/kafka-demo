using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using org.apache.zookeeper;

namespace ZooKeeperAgent
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Runner r = new Runner();
            r.Run();
        }
    }

    internal class Runner
    {
        internal async void Run()
        {
            var data = Encoding.UTF8.GetBytes("test");

            ZooKeeper zk = new ZooKeeper("localhost:2181", 50000, NullWatcher.Instance);
            
            var zNodeRoot = zk.existsAsync("/").Result.getCversion();
            
            var zNodeA = zk.existsAsync("/a").Result;
            if (zNodeA == null)
            {
                await zk.createAsync("/a", null, ZooDefs.Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT);
            }

            var node = zk.existsAsync("/a/1").Result;
            Task<string> nodetask = null;
            if (node == null)
            {
                nodetask = zk.createAsync("/a/1", data, ZooDefs.Ids.OPEN_ACL_UNSAFE, CreateMode.EPHEMERAL);
            }

            nodetask.Wait();
            
            IList<string> children = zk.getChildrenAsync("/a", false).Result.Children;
            
            var test2 = zk.getDataAsync("/a", NodeWatcher.Instance).Result;
            var test3 = zk.setDataAsync("/a", data).Result;


            var closeEvent = zk.closeAsync();
            closeEvent.Wait();
        }
    }

    internal class NullWatcher : Watcher
    {
        public static readonly NullWatcher Instance = new NullWatcher();
        private NullWatcher() { }
        public override Task process(WatchedEvent e)
        {
            Console.WriteLine($"NullWatcher: {e}");
            return Task.CompletedTask;
        }
    }

    internal class NodeWatcher : Watcher
    {
        public static readonly NodeWatcher Instance = new NodeWatcher();
        private NodeWatcher() { }
        public override Task process(WatchedEvent e)
        {
            Console.WriteLine($"NodeWatcher: {e}");
            return Task.CompletedTask;
        }
    }
}
