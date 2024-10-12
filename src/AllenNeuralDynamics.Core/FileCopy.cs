﻿using Bonsai;
using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.IO;

namespace AllenNeuralDynamics.Core
{
    [Combinator]
    [Description("Copies a target file.")]
    [WorkflowElementCategory(ElementCategory.Sink)]
    public class FileCopy
    {
        private string sourcePath;
        [Editor("Bonsai.Design.OpenFileNameEditor, Bonsai.Design", DesignTypes.UITypeEditor)]
        [Description("Name of the source file to be copied.")]
        public string SourcePath
        {
            get { return sourcePath; }
            set { sourcePath = value; }
        }

        private string destinationPath;
        [Editor("Bonsai.Design.SaveFileNameEditor, Bonsai.Design", DesignTypes.UITypeEditor)]
        [Description("Name of the destination file.")]
        public string DestinationPath
        {
            get { return destinationPath; }
            set { destinationPath = value; }
        }

        private bool overwrite;
        public bool Overwrite
        {
            get { return overwrite; }
            set { overwrite = value; }
        }

        public IObservable<T> Process<T>(IObservable<T> source)
        {
            return source.Do(_ =>
            {
                File.Copy(sourcePath, destinationPath, overwrite);
            });
        }
    }
}
