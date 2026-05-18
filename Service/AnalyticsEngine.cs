using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public static class AnalyticsEngine
    {
        private static readonly double RealPowerMaxThreshold = double.Parse(ConfigurationManager.AppSettings["RealPowerMaxThreshold"]);

        private static readonly int EnergyStalledRows = int.Parse(ConfigurationManager.AppSettings["EnergyStalledRows"]);

        private static readonly double FrequencyNominal = double.Parse(ConfigurationManager.AppSettings["FrequencyNominal"]);

        private static readonly double FrequencyDeviationMax = double.Parse(ConfigurationManager.AppSettings["FrequencyDeviationMax"]);

        private static readonly double FrequencySpikeMax = double.Parse(ConfigurationManager.AppSettings["FrequencySpikeMax"]);

        private static readonly Dictionary<string, VehicleAnalyticsState> _states = new Dictionary<string, VehicleAnalyticsState>();

        public static void ProcessSample(ChargingSample sample)
        {
            if (!_states.ContainsKey(sample.VehicleId))
                _states[sample.VehicleId] = new VehicleAnalyticsState();

            VehicleAnalyticsState state = _states[sample.VehicleId];

            CheckEnergy(sample, state);

            CheckFrequency(sample, state);

            state.PreviousSample = sample;
        }
        private static void CheckEnergy(ChargingSample sample, VehicleAnalyticsState state)
        {
            state.CumulativeEnergy += sample.RealPowerAvg;

            if (state.PreviousSample != null)
            {
                double energyGrowth = sample.RealPowerAvg - state.PreviousSample.RealPowerAvg;

                if (energyGrowth < 0.01)
                {
                    state.StallCount++;
                }
                else
                {
                    state.StallCount = 0;
                }

                if (state.StallCount >= EnergyStalledRows)
                {
                    EventPublisher.RaiseWarning(
                        sample.VehicleId,
                        sample.RowIndex,
                        "EnergyStallWarning",
                        state.CumulativeEnergy,
                        $"Energija stagnira vec {state.StallCount} redova. " +
                        $"Kumulativna energija: {state.CumulativeEnergy:F3} kW"
                    );
                    state.StallCount = 0;
                }
            }

            if (sample.RealPowerMax > RealPowerMaxThreshold)
            {
                EventPublisher.RaiseWarning(
                    sample.VehicleId,
                    sample.RowIndex,
                    "OverloadWarning",
                    sample.RealPowerMax,
                    $"Real Power Max ({sample.RealPowerMax:F3} kW) " +
                    $"premasuje prag od {RealPowerMaxThreshold} kW"
                );
            }
        }

        private static void CheckFrequency(ChargingSample sample, VehicleAnalyticsState state)
        {
            double deviation = Math.Abs(sample.FrequencyAvg - FrequencyNominal);

            if (deviation > FrequencyDeviationMax)
            {
                EventPublisher.RaiseWarning(
                    sample.VehicleId,
                    sample.RowIndex,
                    "FrequencyDeviationWarning",
                    sample.FrequencyAvg,
                    $"Frekvencija ({sample.FrequencyAvg:F3} Hz) odstupa za " +
                    $"{deviation:F3} Hz od nominalne vrednosti {FrequencyNominal} Hz"
                );
            }
            if (state.PreviousSample != null)
            {
                double spikeMin = Math.Abs(sample.FrequencyMin - state.PreviousSample.FrequencyMin);
                double spikeMax = Math.Abs(sample.FrequencyMax - state.PreviousSample.FrequencyMax);

                if (spikeMin > FrequencySpikeMax)
                {
                    EventPublisher.RaiseWarning(
                        sample.VehicleId,
                        sample.RowIndex,
                        "FrequencySpike",
                        spikeMin,
                        $"Nagli skok Frequency Min: delta = {spikeMin:F3} Hz " +
                        $"(prag: {FrequencySpikeMax} Hz)"
                    );
                }

                if (spikeMax > FrequencySpikeMax)
                {
                    EventPublisher.RaiseWarning(
                        sample.VehicleId,
                        sample.RowIndex,
                        "FrequencySpike",
                        spikeMax,
                        $"Nagli skok Frequency Max: delta = {spikeMax:F3} Hz " +
                        $"(prag: {FrequencySpikeMax} Hz)"
                    );
                }
            }
        }
    }
    public class VehicleAnalyticsState
    {
        public double CumulativeEnergy { get; set; } = 0;

        public int StallCount { get; set; } = 0;

        public ChargingSample PreviousSample { get; set; } = null;
    }
}

