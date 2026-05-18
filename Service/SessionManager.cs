using Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Service
{
    public class SessionManager : IDisposable
    {
        private readonly Dictionary<string, StreamWriter> _writers = new Dictionary<string, StreamWriter>();

        private bool _disposed = false;
        public void OpenSession(string vehicleId)
        {
            string folderPath = Path.Combine(
                "Data",
                vehicleId,
                DateTime.Now.ToString("yyyy-MM-dd")
            );
            Directory.CreateDirectory(folderPath);

            string sessionPath = Path.Combine(folderPath, "session.csv");
            FileStream fs = new FileStream(sessionPath, FileMode.Append, FileAccess.Write);
            StreamWriter writer = new StreamWriter(fs);

            if (new FileInfo(sessionPath).Length == 0)
            {
                writer.WriteLine(
                    "Timestamp,VoltageMin,VoltageAvg,VoltageMax," +
                    "CurrentMin,CurrentAvg,CurrentMax," +
                    "RealPowerMin,RealPowerAvg,RealPowerMax," +
                    "ReactivePowerMin,ReactivePowerAvg,ReactivePowerMax," +
                    "ApparentPowerMin,ApparentPowerAvg,ApparentPowerMax," +
                    "FrequencyMin,FrequencyAvg,FrequencyMax,RowIndex"
                );
            }

            _writers[vehicleId] = writer;
        }

        public void WriteSample(ChargingSample sample)
        {
            if (!_writers.ContainsKey(sample.VehicleId))
            {
                WriteReject(sample, "Sesija nije otvorena za ovo vozilo.");
                return;
            }

            StreamWriter writer = _writers[sample.VehicleId];

            writer.WriteLine(
                $"{sample.Timestamp}," +
                $"{sample.VoltageMin},{sample.VoltageAvg},{sample.VoltageMax}," +
                $"{sample.CurrentMin},{sample.CurrentAvg},{sample.CurrentMax}," +
                $"{sample.RealPowerMin},{sample.RealPowerAvg},{sample.RealPowerMax}," +
                $"{sample.ReactivePowerMin},{sample.ReactivePowerAvg},{sample.ReactivePowerMax}," +
                $"{sample.ApparentPowerMin},{sample.ApparentPowerAvg},{sample.ApparentPowerMax}," +
                $"{sample.FrequencyMin},{sample.FrequencyAvg},{sample.FrequencyMax}," +
                $"{sample.RowIndex}"
            );

            writer.Flush();
        }

        public void WriteReject(ChargingSample sample, string reason)
        {
            string folderPath = Path.Combine("Data", sample.VehicleId, DateTime.Now.ToString("yyyy-MM-dd"));

            Directory.CreateDirectory(folderPath);
            string rejectsPath = Path.Combine(folderPath, "rejects.csv");

            using (StreamWriter rejectWriter = new StreamWriter(rejectsPath, append: true))
            {
                rejectWriter.WriteLine($"{sample.RowIndex},{sample.Timestamp},{reason}");
            }
        }

        public void CloseSession(string vehicleId)
        {
            if (_writers.ContainsKey(vehicleId))
            {
                _writers[vehicleId].Dispose();
                _writers.Remove(vehicleId);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                foreach (var writer in _writers.Values)
                {
                    writer.Dispose();
                }
                _writers.Clear();
            }

            _disposed = true;
        }
    }
}


