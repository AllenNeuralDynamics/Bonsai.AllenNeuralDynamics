﻿using System;
using System.ComponentModel;
using System.Reactive.Linq;
using Bonsai;
using Zaber.Motion;
using Zaber.Motion.Ascii;

namespace AllenNeuralDynamics.Zaber
{
    /// <summary>
    /// Represents an operator that sends reads a setting from a given axis..
    /// </summary>
    [Description("Gets the value of a setting from an axis.")]
    public class GetSetting : Combinator<double>
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
        public int? Device { get; set; }

        /// <summary>
        /// Gets or sets the axis of the manipulator to be controlled.
        /// </summary>
        [Description("The axis index to be actuated.")]
        public int Axis { get; set; }

        /// <summary>
        /// Gets or sets the axis setting to be queried.
        /// </summary>
        [Description("The setting to get.")]
        public string Setting { get; set; }


        /// <summary>
        /// Gets or sets the Units of the transaction.
        /// </summary>
        [Description("The setting to get.")]
        public Units Units { get; set; } = Units.Native;

        /// <summary>
        /// Sends a request to get a specific setting for an axis.
        /// </summary>
        /// <returns>
        /// On each event, it will emit the response of the device.
        /// </returns>
        public override IObservable<double> Process<T>(IObservable<T> source)
        {
            return Observable.Using(
                async token => await ZaberDeviceManager.ReserveConnectionAsync(PortName),
                async (connection, cancellationToken) => source.Select(_ =>
                    Observable.FromAsync( async token =>
                        await connection.Device.GetSetting(Device, Axis, Setting, Units)
                    )).Concat());
        }
    }
}
