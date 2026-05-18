using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ChargingService : IChargingService
    {
        private readonly SessionManager _sessionManager = new SessionManager();

        public string StartSession(string vehicleId)
        {
            if (string.IsNullOrWhiteSpace(vehicleId))
            {
                throw new FaultException<CustomFault>(new CustomFault("VehicleId ne sme biti prazan."));
            }

            _sessionManager.OpenSession(vehicleId);

            EventPublisher.RaiseTransferStarted(vehicleId);

            Console.WriteLine($"[START] Sesija otvorena za vozilo: {vehicleId}");
            return vehicleId;
        }

        public void PushSample(ChargingSample sample)
        {
            ValidateSample(sample);

            _sessionManager.WriteSample(sample);

            EventPublisher.RaiseSampleReceived(sample);

            AnalyticsEngine.ProcessSample(sample);

            Console.WriteLine($"[PRENOS U TOKU] {sample}");
        }

        public void EndSession(string vehicleId)
        {
            if (string.IsNullOrWhiteSpace(vehicleId))
            {
                throw new FaultException<CustomFault>(new CustomFault("VehicleId ne sme biti prazan."));
            }

            _sessionManager.CloseSession(vehicleId);

            EventPublisher.RaiseTransferCompleted(vehicleId);

            Console.WriteLine($"[KRAJ] Sesija zatvorena za vozilo: {vehicleId}");
        }

        private void ValidateSample(ChargingSample sample)
        {
            if (sample == null)
            {
                throw new FaultException<CustomFault>(new CustomFault("Sample ne sme biti null."));
            }

            if (string.IsNullOrWhiteSpace(sample.Timestamp))
            {
                throw new FaultException<CustomFault>(new CustomFault($"[Row {sample.RowIndex}] Timestamp je prazan."));
            }

            if (sample.VoltageAvg <= 0)
            {
                throw new FaultException<CustomFault>(new CustomFault($"[Row {sample.RowIndex}] Napon mora biti > 0. Vrednost: {sample.VoltageAvg}"));
            }

            if (sample.FrequencyAvg <= 0)
            {
                throw new FaultException<CustomFault>(new CustomFault($"[Row {sample.RowIndex}] Frekvencija mora biti > 0. Vrednost: {sample.FrequencyAvg}"));
            }
        }
    }
}
