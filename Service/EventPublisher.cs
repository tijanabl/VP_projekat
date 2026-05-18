using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public static class EventPublisher
    {
        public static event TransferStartedHandler OnTransferStarted;
        public static event SampleReceivedHandler OnSampleReceived;
        public static event TransferCompletedHandler OnTransferCompleted;
        public static event WarningRaisedHandler OnWarningRaised;

        public static void Subscribe()
        {
            var listener = new ChargingListener();
            OnTransferStarted += listener.HandleTransferStarted;
            OnSampleReceived += listener.HandleSampleReceived;
            OnTransferCompleted += listener.HandleTransferCompleted;
            OnWarningRaised += listener.HandleWarningRaised;
        }

        public static void RaiseTransferStarted(string vehicleId)
        {
            OnTransferStarted?.Invoke(null, new TransferStartedEventArgs(vehicleId));
        }

        public static void RaiseSampleReceived(ChargingSample sample)
        {
            OnSampleReceived?.Invoke(null, new SampleReceivedEventArgs(sample));
        }

        public static void RaiseTransferCompleted(string vehicleId)
        {
            OnTransferCompleted?.Invoke(null, new TransferCompletedEventArgs(vehicleId));
        }

        public static void RaiseWarning(string vehicleId, int rowIndex, string warningType, double currentValue, string message)
        {
            OnWarningRaised?.Invoke(null, new WarningEventArgs(vehicleId, rowIndex, warningType, currentValue, message));
        }
    }
}