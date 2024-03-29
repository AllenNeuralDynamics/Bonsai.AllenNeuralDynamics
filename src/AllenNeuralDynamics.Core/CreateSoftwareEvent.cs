﻿using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Bonsai.Harp;

namespace AllenNeuralDynamics.Core.Logging { 
    [Combinator]
    [Description("Creates a fully populated SoftwareEvent a <T> generic object. If a Harp.Timestamped<T> is provided, temporal information will be added.")]
    [WorkflowElementCategory(ElementCategory.Transform)]
    public class CreateSoftwareEvent
    {
        public string EventName { get; set; } = "SoftwareEvent";

        public IObservable<SoftwareEvent> Process<TSource>(IObservable<Timestamped<TSource>> source)
        {
            var thisName = EventName;
            return source.Select(value => {
                return new SoftwareEvent
                {
                    Data = value.Value,
                    Timestamp = value.Seconds,
                    TimestampSource = SoftwareEventTimestampSource.Harp,
                    FrameIndex = null,
                    FrameTimestamp = null,
                    Name = thisName,
                    DataType = getDataType(value.Value)
                };
            });
        }

        public IObservable<SoftwareEvent> Process<TSource>(IObservable<TSource> source)
        {
            var thisName = EventName;
            return source.Select(value => {
                return new SoftwareEvent
                {
                    Data = value,
                    Timestamp = null,
                    TimestampSource = SoftwareEventTimestampSource.None,
                    FrameIndex = null,
                    FrameTimestamp = null,
                    Name = thisName,
                    DataType = getDataType(value)
                };
            });
        }

        private static SoftwareEventDataType getDataType<T>(T value)
        {
            double parsed;
            if (value == null)
            {
                return SoftwareEventDataType.Null;
            }
            if (double.TryParse(value.ToString(), out parsed))
            {
                return SoftwareEventDataType.Number;
            }
            var type = value.GetType();
            if (type == typeof(string))
            {
                return SoftwareEventDataType.String;
            }
            if (type == typeof(bool))
            {
                return SoftwareEventDataType.Boolean;
            }
            if (type.IsArray)
            {
                return SoftwareEventDataType.Array;
            }
            return SoftwareEventDataType.Object;
        }
    }
}
