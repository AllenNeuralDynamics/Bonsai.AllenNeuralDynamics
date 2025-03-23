using System;
using System.ComponentModel;
using System.Reactive.Linq;
using YamlDotNet.Serialization;
using Bonsai;

namespace AllenNeuralDynamics.HarpUtils
{
    [Description("Parses device metadata from a device.yml string.")]
    public class ParseDeviceMetadata : Transform<string, DeviceMetadata>
    {
        public override IObservable<DeviceMetadata> Process(IObservable<string> source)
        {
            return source.Select(value =>
            {
                var deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();
                var metadata = deserializer.Deserialize<DeviceMetadata>(value);
                return metadata;
            });
        }
    }
}

public struct DeviceMetadata
{
    [YamlMember(Alias = "whoAmI")]
    public int WhoAmI { get; set; }

    [YamlMember(Alias = "device")]
    public string Device { get; set; }

    public override string ToString()
    {
        return $"Device: {Device}, WhoAmI: {WhoAmI}";
    }
}