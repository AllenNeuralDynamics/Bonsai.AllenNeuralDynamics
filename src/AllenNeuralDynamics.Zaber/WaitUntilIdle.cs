﻿using System;
using System.ComponentModel;
using System.Reactive.Linq;
using Bonsai;
using System.Reactive;


namespace AllenNeuralDynamics.Zaber
{
    /// <summary>
    /// Represents an operator that queries and waits to return util a <see cref="ZaberDevice"/> goes idle.
    /// </summary>
    [Description("Returns when the manipulator goes idle.")]
    public class WaitUntilIdle : Combinator<Unit>
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
        [Description("The index of the axis of the manipulator to be controlled. If null, will stop movement on all axes.")]
        public int? Axis { get; set; } = null;

        /// <summary>
        /// Queries the idle state of the manipulator
        /// </summary>
        /// <returns>
        /// When subscribed, it will emit a sequence with a single value when the manipulator is idle.
        /// </returns>
        public IObservable<Unit> Generate()
        {
            return Observable.Using(
                async token => await ZaberDeviceManager.ReserveConnectionAsync(PortName),
                async (connection, cancellationToken) => Observable.Return(
                     await connection.Device.WaitUntilIdle(Device, Axis)
                    ));
        }

        /// <summary>
        /// Queries the idle state of the manipulator
        /// </summary>
        /// <returns>
        /// On each event, it will emit a sequence with a single value when the manipulator is idle.
        /// </returns>
        public override IObservable<Unit> Process<TSource>(IObservable<TSource> source)
        {
            return Observable.Using(
                async token => await ZaberDeviceManager.ReserveConnectionAsync(PortName),
                async (connection, cancellationToken) => source.Select( _ =>
                    Observable.FromAsync( async token =>
                        await connection.Device.WaitUntilIdle(Device, Axis)
                    )).Concat());
        }
    }
}
