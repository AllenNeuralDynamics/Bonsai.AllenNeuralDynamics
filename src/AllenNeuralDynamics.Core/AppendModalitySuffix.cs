using Bonsai;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Collections.Generic;

namespace AllenNeuralDynamics.Core
{
    [Description("Outputs a known data modality string from a dropdown list.")]
    public class AppendModalitySuffix : Transform<string, string>
    {
        public Modality? Modality { get; set; } = null;

        public override IObservable<string> Process(IObservable<string> source)
        {
            return source.Select(value =>
            {
                return Path.Combine(value, Modality.HasValue ? modalityDirectories[Modality.Value] : "");
            });
        }

        private readonly Dictionary<Modality, string> modalityDirectories = new Dictionary<Modality, string>()
        {
                {Core.Modality.Behavior, "behavior"},
                {Core.Modality.Confocal, "confocal"},
                {Core.Modality.Ecephys, "ecephys"},
                {Core.Modality.BehaviorVideos, "behavior-videos"},
                {Core.Modality.Electromyography, "EMG"},
                {Core.Modality.Fib, "fib"},
                {Core.Modality.Fmost, "fMOST"},
                {Core.Modality.Icephys, "icephys"},
                {Core.Modality.Isi, "ISI"},
                {Core.Modality.Merfish, "merfish"},
                {Core.Modality.Mri, "MRI"},
                {Core.Modality.POphys, "ophys"},
                {Core.Modality.Slap, "slap"},
                {Core.Modality.Spim, "SPIM"}
        };
    }

    public enum Modality
    {
        Behavior = 1,
        Confocal = 2,
        Ecephys = 3,
        BehaviorVideos = 4,
        Electromyography = 5,
        Fib = 6,
        Fmost = 7,
        Icephys = 8,
        Isi = 9,
        Merfish = 10,
        Mri = 11,
        POphys = 12,
        Slap = 13,
        Spim = 14,
    }
}
