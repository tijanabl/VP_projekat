using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ChargingListener
    {
        private readonly string _logPath = Path.Combine("Data", "events.log");

        public ChargingListener()
        {
            Directory.CreateDirectory("Data");
        }

        public void HandleTransferStarted(object sender, TransferStartedEventArgs e)
        {
            string msg = $"[{e.StartTime:HH:mm:ss}] TRANSFER STARTED | Vehicle: {e.VehicleId}";
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(msg);
            Console.ResetColor();
            WriteLog(msg);
        }

        public void HandleSampleReceived(object sender, SampleReceivedEventArgs e)
        {
            string msg = $"[{e.ReceivedAt:HH:mm:ss}] SAMPLE | Vehicle: {e.Sample.VehicleId} | Row: {e.Sample.RowIndex}";
            Console.WriteLine(msg);
            WriteLog(msg);
        }

        public void HandleTransferCompleted(object sender, TransferCompletedEventArgs e)
        {
            string msg = $"[{e.EndTime:HH:mm:ss}] TRANSFER COMPLETED | Vehicle: {e.VehicleId}";
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(msg);
            Console.ResetColor();
            WriteLog(msg);
        }

        public void HandleWarningRaised(object sender, WarningEventArgs e)
        {
            string msg = $"[{DateTime.Now:HH:mm:ss}] WARNING | {e.WarningType} | " +
                         $"Vehicle: {e.VehicleId} | Row: {e.RowIndex} | " +
                         $"Value: {e.CurrentValue:F3} | {e.Message}";
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(msg);
            Console.ResetColor();
            WriteLog(msg);
        }

        private void WriteLog(string message)
        {
            using (StreamWriter writer = new StreamWriter(_logPath, append: true))
            {
                writer.WriteLine(message);
            }
        }
    }
}
