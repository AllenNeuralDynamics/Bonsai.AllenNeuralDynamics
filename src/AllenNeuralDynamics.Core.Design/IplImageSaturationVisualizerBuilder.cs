
using Bonsai.Expressions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using Bonsai;
using AllenNeuralDynamics.Core.Design;
using OpenCV.Net;
using System.Configuration;

namespace AllenNeuralDynamics.VrForaging
{
    [TypeVisualizer(typeof(IplImageSaturationVisualizer))]
    [WorkflowElementCategory(ElementCategory.Combinator)]
    [Description("Create an operator with an IplImageSaturationVisualizer")]
    public class IplImageSaturationVisualizerBuilder : SingleArgumentExpressionBuilder
    {
        [Range(0, 255)]
        [Editor(DesignTypes.SliderEditor, DesignTypes.UITypeEditor)]
        [Description("The minimum saturation value below which values will be painted blue.")]
        public byte Minimum { get; set; } = 0;

        [Range(0, 255)]
        [Editor(DesignTypes.SliderEditor, DesignTypes.UITypeEditor)]
        [Description("The maximum saturation value above which values will be painted red.")]
        public byte Maximum { get; set; } = 255;

        [Range(0, 1)]
        [Editor(DesignTypes.SliderEditor, DesignTypes.UITypeEditor)]
        [Description("Alpha value to paint the overlay color with.")]
        public float OverlayAlpha { get; set; } = 1;


        /// <inheritdoc/>
        public override Expression Build(IEnumerable<Expression> arguments)
        {
            var source = arguments.First();

            return Expression.Call(typeof(IplImageSaturationVisualizerBuilder), "Process", null, source);
        }

        static IObservable<IplImage> Process(IObservable<IplImage> source)
        {
            return source;
        }
    }

}
