﻿using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Bonsai;
using Zaber.Motion;


namespace AllenNeuralDynamics.Zaber
{
    /// <summary>
    /// Represents an operator that instructs a <see cref="ZaberDevice"/> move to the target absolute position.
    /// </summary>
    [Description("Moves a target axis to a absolute position")]
    public class MoveAbsolute : Sink<double>
    {
        /// <summary>
        /// Gets or sets the COM port or alias of the target <see cref="ZaberDevice"/>
        /// </summary>
        [TypeConverter(typeof(PortNameConverter))]
        [Description("The name of the serial port used to communicate with the manipulator.")]
        public string PortName { get; set; }

        /// <summary>
        /// Gets or sets the device to be controlled. Defaults to 0.
        /// </summary>
        [Description("The axis index to be actuated.")]
        public int Device { get; set; } = 0;

        /// <summary>
        /// Gets or sets the axis of the manipulator to be controlled.
        /// </summary>
        [Description("The index of the axis of the manipulator to be controlled.")]
        public int Axis { get; set; }

        /// <summary>
        /// Gets or sets optional velocity of the movement.
        /// </summary>
        [Description("Optional velocity used to generate the movement.")]
        public double? Velocity { get; set; } = null;

        /// <summary>
        /// Gets or sets optional acceleration of the movement.
        /// </summary>
        [Description("Optional acceleration used to generate the movement.")]
        public double? Acceleration { get; set; } = null;

        /// <summary>
        /// Gets or sets the Units of position the manipulator instruction is operating on.
        /// </summary>
        [TypeConverter(typeof(PositionUnitsConverter))]
        [Description("The axis index to be actuated.")]
        public Units Units { get; set; } = Units.Native;

        /// <summary>
        /// Gets or sets the Units of velocity the manipulator instruction is operating on.
        /// </summary>
        [TypeConverter(typeof(VelocityUnitsConverter))]
        [Description("The axis index to be actuated.")]
        public Units VelocityUnits { get; set; } = Units.Native;

        /// <summary>
        /// Gets or sets the Units of acceleration the manipulator instruction is operating on.
        /// </summary>
        [TypeConverter(typeof(AccelerationUnitsConverter))]
        [Description("The axis index to be actuated.")]
        public Units AccelerationUnits { get; set; } = Units.Native;

        /// <summary>
        /// Moves to the target absolute position when a valid value is received.
        /// </summary>
        /// <returns>
        /// Returns the original input sequence.
        /// </returns>
        public override IObservable<double> Process(IObservable<double> source)
        {
            return Observable.Using(
                cancellationToken => ZaberDeviceManager.ReserveConnectionAsync(PortName),
                (connection, cancellationToken) =>
                {
                    return Task.FromResult(source.Do(value =>
                    {
                        lock (connection.Device)
                        {
                            connection.Device.MoveAbsolute(Device, Axis, value,
                                Velocity.HasValue ? Velocity.Value : 0,
                                Acceleration.HasValue ? Acceleration.Value : 0,
                                Units, VelocityUnits, AccelerationUnits);
                        }
                    }));
                });
        }
    }
}
