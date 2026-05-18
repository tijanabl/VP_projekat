using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class TransferStartedEventArgs : EventArgs
    {
        public string VehicleId { get; set; }
        public DateTime StartTime { get; set; }

        public TransferStartedEventArgs(string vehicleId)
        {
            VehicleId = vehicleId;
            StartTime = DateTime.Now;
        }
    }

    public class SampleReceivedEventArgs : EventArgs
    {
        public ChargingSample Sample { get; set; }
        public DateTime ReceivedAt { get; set; }

        public SampleReceivedEventArgs(ChargingSample sample)
        {
            Sample = sample;
            ReceivedAt = DateTime.Now;
        }
    }

    public class TransferCompletedEventArgs : EventArgs
    {
        public string VehicleId { get; set; }
        public DateTime EndTime { get; set; }

        public TransferCompletedEventArgs(string vehicleId)
        {
            VehicleId = vehicleId;
            EndTime = DateTime.Now;
        }
    }

    public class WarningEventArgs : EventArgs
    {
        public string VehicleId { get; set; }
        public int RowIndex { get; set; }
        public string WarningType { get; set; }
        public double CurrentValue { get; set; }
        public string Message { get; set; }

        public WarningEventArgs(string vehicleId, int rowIndex, string warningType, double currentValue, string message)
        {
            VehicleId = vehicleId;
            RowIndex = rowIndex;
            WarningType = warningType;
            CurrentValue = currentValue;
            Message = message;
        }
    }

    public delegate void TransferStartedHandler(object sender, TransferStartedEventArgs e);
    public delegate void SampleReceivedHandler(object sender, SampleReceivedEventArgs e);
    public delegate void TransferCompletedHandler(object sender, TransferCompletedEventArgs e);
    public delegate void WarningRaisedHandler(object sender, WarningEventArgs e);
}
