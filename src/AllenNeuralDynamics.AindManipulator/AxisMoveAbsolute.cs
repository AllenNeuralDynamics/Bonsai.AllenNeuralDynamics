using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Harp.StepperDriver;
using Bonsai.Harp;

namespace AllenNeuralDynamics.AindManipulator
{
    [Combinator]
    [Description("Creates a message to move a single axis to a given absolute position.")]
    [WorkflowElementCategory(ElementCategory.Combinator)]
    public class MoveAbsoluteSingleAxis
    {
        private Axis axis = Axis.Y1;
        public Axis Axis
        {
            get { return axis; }
            set { axis = value; }
        }

        private MessageType messageType = MessageType.Write;
        public MessageType MessageType
        {
            get { return messageType; }
            set { messageType = value; }
        }

        public IObservable<HarpMessage> Process(IObservable<ManipulatorPosition> source)
        {
            return source
                .Where(value => new double[] { value.X, value.Y1, value.Y2, value.Z }.Any(pos => pos != 0))
                .Select(value =>
            {
                double[] targetPosition = new double[4] { value.X, value.Y1, value.Y2, value.Z };
                Axis? axis = null;
                int i;
                for (i = 0; i < targetPosition.Length; i++)
                {
                    if (targetPosition[i] != 0)
                    {
                        if (axis.HasValue)
                        {
                            throw new InvalidOperationException("Multiple axis selected. Only one axis can be non-zero");
                        }
                        else
                        {
                            axis = (Axis)i + 1;
                        }
                    }
                }
                if (!axis.HasValue)
                {
                }
                return BuildMessage(axis.Value, messageType, (int)targetPosition[(int)axis]);
            });
        }

        public IObservable<HarpMessage> Process(IObservable<AindManipulatorPosition> source)
        {
            return Process(source.Select(value => value.ToManipulatorPosition()));
        }

        public IObservable<HarpMessage> Process(IObservable<int> source)
        {
            return source.Select(value =>
            {
                return BuildMessage(axis, messageType, value);
            });
        }

        private static HarpMessage BuildMessage(Axis axis, MessageType messageType, int position)
        {
            switch (axis)
            {
                case Axis.X:
                    return Motor0MoveAbsolute.FromPayload(messageType, position);
                case Axis.Y1:
                    return Motor1MoveAbsolute.FromPayload(messageType, position);
                case Axis.Y2:
                    return Motor2MoveAbsolute.FromPayload(messageType, position);
                case Axis.Z:
                    return Motor3MoveAbsolute.FromPayload(messageType, position);
                default:
                    throw new InvalidOperationException("Invalid axis selection.");
            }
        }
    }
}