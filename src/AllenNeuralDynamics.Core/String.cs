using Bonsai;
using Bonsai.Design;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace AllenNeuralDynamics.Core
{
    [Description("An operator that emits a specified string.")]
    [DefaultProperty(nameof(Value))]
    public class String : Source<string>
    {
        [Editor("Bonsai.Design.RichTextEditor, Bonsai.Design", DesignTypes.UITypeEditor)]
        [Description("The value of the string to be propagated.")]
        public string Value { get; set; }

        public IObservable<string> Generate<TSource>(IObservable<TSource> source)
        {
            return source.SelectMany(input => Generate());
        }

        public override IObservable<string> Generate()
        {
            return Observable.Return(Value);
        }
    }
}
