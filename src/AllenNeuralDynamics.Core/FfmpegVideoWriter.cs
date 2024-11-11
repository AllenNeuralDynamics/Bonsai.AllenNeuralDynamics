﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Bonsai;
using OpenCV.Net;
using Bonsai.Vision;
using Bonsai.IO;
using System.IO;

namespace AllenNeuralDynamics.Core
{
    [DefaultProperty("FileName")]
    [Description("Writes the input image sequence to an FFmpeg process.")]
    public class FfmpegVideoWriter : Sink<IplImage>
    {
        [Description("The name of the output file.")]
        [Editor("Bonsai.Design.SaveFileNameEditor, Bonsai.Design", DesignTypes.UITypeEditor)]
        public string FileName { get; set; }

        [Description("Indicates whether the output file should be overwritten if it already exists.")]
        public bool Overwrite { get; set; } = false;

        [Description("The playback frame rate of the image sequence.")]
        public int FrameRate { get; set; }

        [Description("Verbosity")]
        public Verbosity Verbosity { get; set; } = Verbosity.Verbose;

        [Editor(DesignTypes.MultilineStringEditor, DesignTypes.UITypeEditor)]
        [Description("The optional set of command-line arguments to use for configuring the video codec.")]
        public string OutputArguments { get; set; } = @"-vf ""scale=out_color_matrix=bt709:out_range=full"" -c:v h264_nvenc -pix_fmt nv12 -color_range full -colorspace bt709 -color_trc linear -tune hq -preset p4 -rc vbr -cq 12 -b:v 0M -metadata author=""Allen Institute for Neural Dynamics"" -maxrate 700M -bufsize 350M";

        [Editor(DesignTypes.MultilineStringEditor, DesignTypes.UITypeEditor)]
        [Description("The optional set of command-line arguments to use for configuring the input video stream.")]
        public string InputArguments { get; set; } = "-colorspace bt709 -color_primaries bt709 -color_range full -color_trc linear";

        public override IObservable<IplImage> Process(IObservable<IplImage> source)
        {
            return source.Publish(ps =>
            {
                var fileName = PathHelper.AppendSuffix(FileName, PathSuffix.None);
                var overwrite = Overwrite;
                if (File.Exists(fileName) && !overwrite)
                {
                    throw new InvalidOperationException(string.Format("The file '{0}' already exists.", fileName));
                }

                PathHelper.EnsureDirectory(fileName);
                var pipe = @"\\.\pipe\" + Path.GetFileNameWithoutExtension(fileName) + "_" + ((uint)fileName.GetHashCode()).ToString();
                var writer = new ImageWriter { Path = pipe };
                return writer.Process(ps).Merge(ps.Take(1).Delay(TimeSpan.FromSeconds(1)).SelectMany(image =>
                {
                    var inputArguments = string.Format("-v {0} {1}", Verbosity.ToString().ToLower(), InputArguments);
                    var args = string.Format("-f rawvideo -vcodec rawvideo {0}-s {1}x{2} -r {3} -pix_fmt {4} {5} -i {6} {7} {8}",
                        overwrite ? "-y " : string.Empty,
                        image.Width,
                        image.Height,
                        FrameRate,
                        image.Channels == 1 ? "gray" : "bgr24",
                        inputArguments,
                        pipe,
                        OutputArguments,
                        fileName);
                    var ffmpeg = new StartProcess();
                    ffmpeg.Arguments = args;
                    ffmpeg.FileName = "ffmpeg.exe";
                    return ffmpeg.Generate().IgnoreElements().Select(x => default(IplImage));
                }));
            });
        }
    }

    public enum Verbosity
    {
        Quiet,
        Panic,
        Fatal,
        Error,
        Warning,
        Info,
        Verbose,
        Debug,
        Trace
    }
}

/*
This file was adapted from https://github.com/bonsai-rx/ffmpeg/blob/main/Bonsai.FFmpeg/VideoWriter.cs under the following license:


MIT License

Copyright (c) 2021 Gonçalo Lopes

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
