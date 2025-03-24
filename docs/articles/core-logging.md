Core - Logging
==========

# Examples

A simple logging pipeline can be assembled using the Core package.

The first thing that should be defined is the root where all data will be saved to. This can be done using the [`GenerateRootLoggingPath`](xref:AllenNeuralDynamics.VersionControl.GenerateRootLoggingPath). This Node will instantiate a `Subject` with a unique path in the form of ```<RootFolder>/<Subject>/<Date>```.

:::workflow
![GenerateRootLoggingPath](~/workflows/GenerateRootLoggingPath.bonsai)
:::

## Harp data
Once this `Subject` is created, other nodes can access to it. For instance, if one would like to save the data from a `Harp Device`:

### Without metadata
:::workflow
![SaveHarpData](~/workflows/SaveHarpData.bonsai)
:::

### With Metadata

Each device can be saved with metadata by providing the `device.yml` file information to the operator. This string (i.e. the literal of the [`device.yml`](https://github.com/AllenNeuralDynamics/harp.device.lickety-split/blob/main/device.yml)) can be passed manually or by using the `GetMetadata` node from the device-specific package.
For example, to log data from a `LicketySplit` device:

:::workflow
![SaveHarpDataWithMetadata](~/workflows/SaveHarpDataWithMetadata.bonsai)
:::

## Spinnaker camera
Similarly, for a `Spinnaker Camera`:

:::workflow
![SaveSpinnakerCamera](~/workflows/SaveSpinnakerCamera.bonsai)
:::

## Software events
For software events, one can use the `SoftwareEvent` class and the following pattern:

:::workflow
![SoftwareEventLogging](~/workflows/SoftwareEventLogging.bonsai)
:::