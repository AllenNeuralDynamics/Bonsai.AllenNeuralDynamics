﻿using System;
using System.Reactive;
using System.Windows.Forms;

namespace AllenNeuralDynamics.Core.Design
{
    public partial class PushButtonControl : UserControl
    {
        public string ButtonLabel {
            get{return button.Text;}
            set { button.Text = value;} 
        }

        public PushButtonControl(PushButton source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            InitializeComponent();
        }

        public PushButton Source { get; }

        private void tareButton_Click(object sender, EventArgs e)
        {
            Source.OnNext(Unit.Default);
        }
    }
}