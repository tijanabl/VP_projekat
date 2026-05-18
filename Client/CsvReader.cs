using Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class CsvReader : IDisposable
    {
        private bool _disposed = false;

        public List<string> GetAvailableVehicles(string datasetPath)
        {
            var vehicles = new List<string>();

            if (!Directory.Exists(datasetPath))
            {
                Console.WriteLine($"Dataset folder ne postoji: {datasetPath}");
                return vehicles;
            }


            string[] folders = Directory.GetDirectories(datasetPath);

            foreach (string folder in folders)
            {
                vehicles.Add(Path.GetFileName(folder));
            }

            return vehicles;
        }

        public IEnumerable<ChargingSample> ReadSamples(string csvPath, string vehicleId)
        {
            if (!File.Exists(csvPath))
            {
                Console.WriteLine($"CSV fajl ne postoji: {csvPath}");
                yield break;
            }

            using (StreamReader reader = new StreamReader(csvPath))
            {

                string header = reader.ReadLine();
                if (header == null) yield break;

                int rowIndex = 0;
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    rowIndex++;

                    ChargingSample sample = TryParseLine(line, rowIndex, vehicleId);

                    if (sample != null)
                    {
                        yield return sample;
                    }
                    else
                    {
                        LogInvalidRow(rowIndex, line, csvPath);
                    }
                }
            }
        }

        private ChargingSample TryParseLine(string line, int rowIndex, string vehicleId)
        {
            try
            {
                string[] parts = line.Split(',');

                if (parts.Length < 19)
                    return null;

                var culture = CultureInfo.InvariantCulture;

                return new ChargingSample
                {
                    VehicleId = vehicleId,
                    RowIndex = rowIndex,
                    Timestamp = parts[0].Trim(),

                    VoltageMin = double.Parse(parts[1].Trim(), culture),
                    VoltageAvg = double.Parse(parts[2].Trim(), culture),
                    VoltageMax = double.Parse(parts[3].Trim(), culture),

                    CurrentMin = double.Parse(parts[4].Trim(), culture),
                    CurrentAvg = double.Parse(parts[5].Trim(), culture),
                    CurrentMax = double.Parse(parts[6].Trim(), culture),

                    RealPowerMin = double.Parse(parts[7].Trim(), culture),
                    RealPowerAvg = double.Parse(parts[8].Trim(), culture),
                    RealPowerMax = double.Parse(parts[9].Trim(), culture),

                    ReactivePowerMin = double.Parse(parts[10].Trim(), culture),
                    ReactivePowerAvg = double.Parse(parts[11].Trim(), culture),
                    ReactivePowerMax = double.Parse(parts[12].Trim(), culture),

                    ApparentPowerMin = double.Parse(parts[13].Trim(), culture),
                    ApparentPowerAvg = double.Parse(parts[14].Trim(), culture),
                    ApparentPowerMax = double.Parse(parts[15].Trim(), culture),

                    FrequencyMin = double.Parse(parts[16].Trim(), culture),
                    FrequencyAvg = double.Parse(parts[17].Trim(), culture),
                    FrequencyMax = double.Parse(parts[18].Trim(), culture),
                };
            }
            catch
            {
                return null;
            }
        }

        private void LogInvalidRow(int rowIndex, string line, string csvPath)
        {
            string logPath = Path.Combine(Path.GetDirectoryName(csvPath), "invalid_rows.log");

            using (StreamWriter logWriter = new StreamWriter(logPath, append: true))
            {
                logWriter.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] " + $"Row {rowIndex} - nevalidan format: {line}");
            }

            Console.WriteLine($"[UPOZORENJE] Row {rowIndex} preskocen - nevalidan format.");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;
        }
    }
}
