using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            string datasetPath = ConfigurationManager.AppSettings["DatasetPath"];

            string vehicleId = ChooseVehicle(datasetPath);
            if (vehicleId == null) return;

            ChannelFactory<IChargingService> factory = null;
            IChargingService proxy = null;

            try
            {
                factory = new ChannelFactory<IChargingService>("ChargingService");
                proxy = factory.CreateChannel();

                proxy.StartSession(vehicleId);
                Console.WriteLine($"Sesija pokrenuta za: {vehicleId}");

                string csvPath = Path.Combine(datasetPath, vehicleId, "Charging_Profile.csv");

                using (CsvReader csvReader = new CsvReader())
                {
                    foreach (ChargingSample sample in csvReader.ReadSamples(csvPath, vehicleId))
                    {
                        try
                        {
                            proxy.PushSample(sample);
                            Console.WriteLine($"[POSLATO] {sample}");
                        }
                        catch (FaultException<CustomFault> fe)
                        {
                            Console.WriteLine($"[ODBACEN] Row {sample.RowIndex}: {fe.Detail.Message}");
                        }
                    }
                }

                proxy.EndSession(vehicleId);
                Console.WriteLine("Prenos zavrsen.");
            }
            catch (FaultException<CustomFault> fe)
            {
                Console.WriteLine($"Greska servisa: {fe.Detail.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Greska: {e.Message}");
            }
            finally
            {
                if (proxy != null)
                    ((IClientChannel)proxy).Close();

                if (factory != null)
                    factory.Close();
            }

            Console.WriteLine("Pritisnite bilo koji taster za izlaz...");
            Console.ReadKey();
        }

        static string ChooseVehicle(string datasetPath)
        {
            using (CsvReader csvReader = new CsvReader())
            {
                List<string> vehicles = csvReader.GetAvailableVehicles(datasetPath);

                if (vehicles.Count == 0)
                {
                    Console.WriteLine("Nema dostupnih vozila u datasetu.");
                    return null;
                }

                Console.WriteLine("=== Dostupna vozila ===");
                for (int i = 0; i < vehicles.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {vehicles[i]}");
                }

                Console.Write($"Izaberite vozilo (1-{vehicles.Count}): ");
                string input = Console.ReadLine();

                if (int.TryParse(input, out int choice) &&
                    choice >= 1 && choice <= vehicles.Count)
                {
                    return vehicles[choice - 1];
                }

                Console.WriteLine("Nevalidan izbor.");
                return null;
            }
        }
    }
}
