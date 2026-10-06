AIND Manipulator
=================

# Assembly instructions

The manipulator is composed of two main components:

- The physical manipulator composed of multiple axis that makes up a configuration (e.g. a 4 axis manipulator)
- A controller board that interfaces with the manipulator and the computer

## Physical manipulator

The physical manipulator is expected to be assembled as per the following figure. The color of the axis is indicative of the color of the cable that should be connected to the corresponding port on the controller board.

![Manipulator](~/images/aind_manipulator_3d.png)

| Motor | Axis                                                  | Cable Color        |
|-------|-------------------------------------------------------|--------------|
| 0     | X (lateral)                                           | Blue         |
| 1     | Y1 (forward, "port", motor's left mouse's right)      | Red          |
| 2     | Y2 (forward, "starboard", motor's right mouse's left) | Green        |
| 3     | Z (vertical)                                          | White/Silver |


Each motor should also be wired to its respective cable following the color code below:

- Green (B+)
- Green-White (B-)
- Red (A+)
- Red-White (A-)

Finally, each end-of-travel switch follows the color code below:

- Blue (GND)
- White (Signal)
- Brown (+V)


## Controller board

### Bonsai Interface

To interface with the motors, the user can choose to use the Harp package for the [Harp.StepperDriver](https://github.com/harp-tech/device.stepperdriver). For simplicity, we maintain our own wrapper with core functionality and user-interface. The user can install it from the [Aind Nuget feed](https://www.nuget.org/packages/AllenNeuralDynamics.AindManipulator/).

The interface with the manipulator is handled by two operators:

- `CreateAindManipulator`, which sets the initial settings of the manipulator and implements the `StopMotors`, `HomeMotors`, `MoveTo` and `MoveBy` operations.
- `AindManipulatorGui`, which implements a front-end interface for the user to interact with the manipulator.

#### Load a configuration
`CreateAindManipulator` expects settings to be passed via a `AindManipulatorCalibration` class. This class can defined using the respective schema in [`Aind.Behavior.Services` repository](https://github.com/AllenNeuralDynamics/Aind.Behavior.Services). An example on how to generate a configuration file [can be found here](https://github.com/AllenNeuralDynamics/Aind.Behavior.Services/blob/main/examples/aind_manipulator.py). The Json file can be passed to the operator using the following pattern:

:::workflow
![AindManipulatorFromJson](~/workflows/AindManipulatorFromJson.bonsai)
:::

Alternatively, an operator is provided for quickly defining the settings in the workflow directly. This can be done using the following pattern:

:::workflow
![AindManipulatorFromDefault](~/workflows/AindManipulatorFromDefault.bonsai)
:::

#### Front-end interface

The front-end interface for the user to interact with the manipulator can be accessed by double-clicking `AindManipulatorGui` while the workflow is running.

#### Controlling the manipulator

The easiest way to control the manipulator without using the user-interface is to directly interact with the  `StopMotors`, `HomeMotors`, `MoveTo` and `MoveBy` subjects.

If the user wants to control the manipulator using the Harp interface, `StepperMotorCommands` and `StepperMotorEvents` can be used to send and receive commands and events respectively.