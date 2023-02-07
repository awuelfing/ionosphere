using ClusterConnection;

namespace ClusterConsole
{
    internal class Program
    {
        static void ReceiveSpots(object? sender, SpotEventArgs e)
        {
            Console.WriteLine($"{e.Spotter} saw {e.Spottee}");
        }
        static void ReceiveDisconnect(object? sender, EventArgs e)
        {
            Console.WriteLine("Disconnected.");
        }
        static void Main(string[] args)
        {
            ClusterClient clusterClient = new ClusterClient("cluster.pota.app", 8000, "AJ1AJ");
            if(!clusterClient.Connect())
            {
                Console.WriteLine("Failed to connect.");
                return;
            }
            clusterClient.SpotReceived += ReceiveSpots;
            clusterClient.Disconnected += ReceiveDisconnect;
            clusterClient.ProcessSpots();
            
        }
    }
}