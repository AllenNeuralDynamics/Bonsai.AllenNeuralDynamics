using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;


namespace AllenNeuralDynamics.AindManipulator
{
    
    public class DefaultManipulatorSettings : Source<AindManipulatorCalibration>
    {

        [TypeConverter(typeof(NumericRecordConverter))]
        public AindManipulatorPosition InitialPosition { get; set; } = new AindManipulatorPosition() { X = 0, Y1 = 0, Y2 = 0, Z = 0 };

        [TypeConverter(typeof(NumericRecordConverter))]
        public AindManipulatorPosition FullStepToMm { get; set; } = new AindManipulatorPosition() { X = 0.01, Y1 = 0.01, Y2 = 0.01, Z = 0.01};

        [TypeConverter(typeof(UnidimensionalArrayConverter))]
        public Axis[] HomingOrder { get; set; } = new Axis[] {Axis.Y1, Axis.Y2, Axis.X, Axis.Z};

        [TypeConverter(typeof(UnidimensionalArrayConverter))]
        public Axis[] EnabledAxis { get; set; } = new Axis[] { Axis.Y1, Axis.Y2, Axis.X, Axis.Z };

        public int StepAccelerationInterval { get; set; } = 100;

        public int StepInterval { get; set; } = 100;

        public Harp.StepperDriver.MicrostepResolution MicrostepResolution { get; set; } = 0;

        public int MaximumStepInterval { get; set; } = 2000;

        public Harp.StepperDriver.MotorOperationMode MotorOperationMode { get; set; } = 0;

        public double MaxLimit { get; set; } = 30;

        public double MinLimit { get; set; } = -0.01;


        public override IObservable<AindManipulatorCalibration> Generate()
        {
            return Observable.Return(new AindManipulatorCalibration()
            {
                InitialPosition = InitialPosition.ToManipulatorPosition(),
                FullStepToMm = FullStepToMm.ToManipulatorPosition(),
                HomingOrder = HomingOrder.ToList(),
                AxisConfiguration = EnabledAxis.Select(x => DefaultAxisConfiguration(x)).ToList()
            });
        }

        private AxisConfiguration DefaultAxisConfiguration(Axis axis)
        {
            return new AxisConfiguration()
            {
                Axis = axis,
                StepAccelerationInterval = StepAccelerationInterval,
                StepInterval = StepInterval,
                MicrostepResolution = (MicrostepResolution) MicrostepResolution,
                MaximumStepInterval = MaximumStepInterval,
                MotorOperationMode = (MotorOperationMode) MotorOperationMode,
                MaxLimit = MaxLimit,
                MinLimit = MinLimit
            };
        }
    }
}
