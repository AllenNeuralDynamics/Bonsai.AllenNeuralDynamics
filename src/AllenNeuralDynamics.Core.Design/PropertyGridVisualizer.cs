﻿using Bonsai;
using Bonsai.Design;
using Bonsai.Expressions;
using Bonsai.Reactive;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AllenNeuralDynamics.Core.Design
{
    public class PropertyGridVisualizer : DialogTypeVisualizer
    {
        PropertyGrid control;
        PropertyGridSource source;

        public override void Load(IServiceProvider provider)
        {

            var workflow = (WorkflowBuilder)provider.GetService(typeof(WorkflowBuilder)); //gets the whole workflow
            var context = (ITypeVisualizerContext)provider.GetService(typeof(ITypeVisualizerContext));
            var visualizerElement = ExpressionBuilder.GetVisualizerElement(context.Source); // get the class reference that originated the visualizer
            source = (PropertyGridSource)ExpressionBuilder.GetWorkflowElement(visualizerElement.Builder);

            var contextStack = new List<ExpressionBuilderGraph>();
            GetContextByElement(workflow.Workflow, visualizerElement.Builder, contextStack);

            if (contextStack.Count == 0) { throw new NullReferenceException("Could not find the reference for the target object in the workflow."); }
            if (contextStack.Count > 1) { throw new InvalidOperationException("Found multiple references to the same object in the workflow."); }

            control = new PropertyGrid();
            control.Font = new Font(control.Font.FontFamily, 16.2F);
            control.Dock = System.Windows.Forms.DockStyle.Fill;
            control.SelectedObject = contextStack.First();
            control.Size = new Size(400, 450);
            control.Enabled = source.Enabled;

            var visualizerService = (IDialogTypeVisualizerService)provider.GetService(typeof(IDialogTypeVisualizerService));
            if (visualizerService != null)
            {
                visualizerService.AddControl(control);
            }
        }

        public override void Show(object value)
        {
            if (!control.Enabled == source.Enabled)
            {
                control.Enabled = source.Enabled;
            }
            control.Refresh();
        }

        public override void Unload()
        {
            if (control != null)
            {
                control.Dispose();
                control = null;
                source = null;
            }
        }

        static bool IsGroup(IWorkflowExpressionBuilder builder)
        {
            return builder is IncludeWorkflowBuilder || builder is GroupWorkflowBuilder;
        }

        static bool IsNested(ExpressionBuilder element)
        {
            var groupBuilder = element as IWorkflowExpressionBuilder;
            var workflowExpressionBuilder = element is WorkflowExpressionBuilder;
            return groupBuilder is IncludeWorkflowBuilder || groupBuilder is GroupWorkflowBuilder || groupBuilder is Defer || workflowExpressionBuilder;
        }

        static IEnumerable<ExpressionBuilder> SelectContextElements(ExpressionBuilderGraph source, bool recurseGroups)
        {
            foreach (var node in source)
            {
                var element = ExpressionBuilder.Unwrap(node.Value);
                if (element is DisableBuilder) continue;
                yield return element;

                var workflowBuilder = element as IWorkflowExpressionBuilder;
                if (recurseGroups)
                {
                    if (IsGroup(workflowBuilder) & recurseGroups)
                    {
                        var workflow = workflowBuilder.Workflow;
                        if (workflow == null) continue;
                        foreach (var groupElement in SelectContextElements(workflow, recurseGroups))
                        {
                            yield return groupElement;
                        }
                    }
                }
            }
        }

        static void GetContextByElement(ExpressionBuilderGraph sourceWorkflow, ExpressionBuilder targetElement, List<ExpressionBuilderGraph> contexts)
        {
            foreach (var element in SelectContextElements(sourceWorkflow, false)) //Loop elements
            {
                if (element == targetElement)
                { //compare the reference to the target object
                    contexts.Add(sourceWorkflow);
                }

                if (IsNested(element))
                {
                    var groupBuilder = element as IWorkflowExpressionBuilder;
                    GetContextByElement(groupBuilder.Workflow, targetElement, contexts);
                }
            }
        }
    }
}
