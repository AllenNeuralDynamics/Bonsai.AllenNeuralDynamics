using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace AllenNeuralDynamics.AindManipulator
{
    [Combinator]
    [Description("Returns the conversion factor that goes from step to mm.")]
    [WorkflowElementCategory(ElementCategory.Transform)]

    public class ManipulatorSiUnitConverter
    {
        public AindManipulatorCalibration Calibration { get; set; }

        public ConverterMode ConverterMode { get; set; } = ConverterMode.StepToMm;

        public IObservable<ManipulatorPosition> Process(IObservable<ManipulatorPosition> source)
        {
            ManipulatorPosition scalingFactor = ComputerCalibrationFactor(Calibration);

            return source.Select(value =>
            {
                if (ConverterMode == ConverterMode.StepToMm)
                    return value * scalingFactor;
                else if (ConverterMode == ConverterMode.MmToStep)
                    return value / scalingFactor;
                else
                    throw new ArgumentOutOfRangeException("Unknown converter mode.");
            });
        }

        public IObservable<double> Process(IObservable<Tuple<Axis, double>> source)
        {
            ManipulatorPosition scalingFactor = ComputerCalibrationFactor(Calibration);

            return source.Select(value =>
            {
                if (ConverterMode == ConverterMode.StepToMm)
                    return value.Item2 * scalingFactor[value.Item1];
                else if (ConverterMode == ConverterMode.MmToStep)
                    return value.Item2 / scalingFactor[value.Item1];
                else
                    throw new ArgumentOutOfRangeException("Unknown converter mode.");
            });
        }

        public IObservable<double> Process(IObservable<Tuple<Axis, int>> source)
        {
            return Process(source.Select(value => Tuple.Create(value.Item1, (double)value.Item2)));
        }

        private ManipulatorPosition ComputerCalibrationFactor(AindManipulatorCalibration calibration)
        {
            var calibrationFactor = new ManipulatorPosition() { X = 1, Y1 = 1, Y2 = 1, Z = 1 };
            foreach (AxisConfiguration axis in calibration.AxisConfiguration)
            {
                calibrationFactor[axis.Axis] =
                    calibration.FullStepToMm[axis.Axis] *
                    GetMicrostepCorrection(axis.MicrostepResolution);
            }
            return calibrationFactor;
        }

        private double GetMicrostepCorrection(MicrostepResolution microstepResolution)
        {
            switch (microstepResolution)
            {
                case MicrostepResolution.Microstep8:
                    return 1.0 / 8.0;
                case MicrostepResolution.Microstep16:
                    return 1.0 / 16.0;
                case MicrostepResolution.Microstep32:
                    return 1.0 / 32.0;
                case MicrostepResolution.Microstep64:
                    return 1.0 / 64.0;
                default:
                    throw new ArgumentOutOfRangeException("Unknown microstep resolution.");

            }
        }
    }

    public enum ConverterMode
    {
        StepToMm,
        MmToStep
    }
}