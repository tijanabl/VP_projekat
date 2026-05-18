using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{

    [ServiceContract]
    public interface IChargingService
    {
        [OperationContract]
        [FaultContract(typeof(CustomFault))]
        string StartSession(string vehicleId);

        [OperationContract]
        [FaultContract(typeof(CustomFault))]
        void PushSample(ChargingSample sample);

        [OperationContract]
        [FaultContract(typeof(CustomFault))]
        void EndSession(string vehicleId);
    }
}
