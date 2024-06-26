﻿using System.ComponentModel;
using System.Reactive;
using System;
using Bonsai;
using System.Reactive.Linq;

namespace AllenNeuralDynamics.Core.Design
{
    [Obsolete]
    [TypeVisualizer(typeof(PropertyGridVisualizer))]
    [Description("Generates a sequence of string events.")]
    public class PropertyGridSource : Source<Unit> { 

        public PropertyGridSource() { }

        public bool Enabled {get; set;} = true;

        public override IObservable<Unit> Generate()
        {
            return Observable.Never(new Unit());
        }

        public IObservable<Unit> Generate<TSource>(IObservable<TSource> source)
        {
            return source.Select(source => new Unit());
        }
    }
}
