using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(ChargingService));

            EventPublisher.Subscribe();

            try
            {
                host.Open();
                Console.WriteLine("=== Charging Service pokrenut ===");
                Console.WriteLine("Adresa: net.tcp://localhost:8000/ChargingService");
                Console.WriteLine("Pritisnite bilo koji taster za gašenje servisa...");
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Greška pri pokretanju servisa: {e.Message}");
            }
            finally
            {
                host.Close();
                Console.WriteLine("Servis ugašen.");
            }
        }
    }
}
