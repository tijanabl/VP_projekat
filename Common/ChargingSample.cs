using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{

    [DataContract]
    public class ChargingSample
    {

        [DataMember]
        public string VehicleId { get; set; }

        [DataMember]
        public int RowIndex { get; set; }

        [DataMember]
        public string Timestamp { get; set; }

        [DataMember]
        public double VoltageMin { get; set; }

        [DataMember]
        public double VoltageAvg { get; set; }

        [DataMember]
        public double VoltageMax { get; set; }

        [DataMember]
        public double CurrentMin { get; set; }

        [DataMember]
        public double CurrentAvg { get; set; }

        [DataMember]
        public double CurrentMax { get; set; }

        [DataMember]
        public double RealPowerMin { get; set; }

        [DataMember]
        public double RealPowerAvg { get; set; }

        [DataMember]
        public double RealPowerMax { get; set; }

        [DataMember]
        public double ReactivePowerMin { get; set; }

        [DataMember]
        public double ReactivePowerAvg { get; set; }

        [DataMember]
        public double ReactivePowerMax { get; set; }

        [DataMember]
        public double ApparentPowerMin { get; set; }

        [DataMember]
        public double ApparentPowerAvg { get; set; }

        [DataMember]
        public double ApparentPowerMax { get; set; }

        [DataMember]
        public double FrequencyMin { get; set; }

        [DataMember]
        public double FrequencyAvg { get; set; }

        [DataMember]
        public double FrequencyMax { get; set; }

        public override string ToString()
        {
            return $"[Row {RowIndex}] Vehicle: {VehicleId} | Timestamp: {Timestamp} | " +
                   $"Voltage(Avg): {VoltageAvg:F2} | Current(Avg): {CurrentAvg:F2} | " +
                   $"RealPower(Avg): {RealPowerAvg:F2} | Frequency(Avg): {FrequencyAvg:F2}";
        }
    }
}
