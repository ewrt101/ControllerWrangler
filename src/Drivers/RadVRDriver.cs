using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace ControllerWrangler.Driver;


[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct RadVRControllerDataV1
{
    public uint header;          // Must be 0x56444152

    public float triggerValue;
    public float gripValue;
    public byte triggerClick;
    public byte gripClick;
    public byte buttonA;
    public byte buttonB;
    public byte buttonC;
    public byte buttonD;
    public byte calibrate;
    public byte menu;
    public float joystickX;
    public float joystickY;
    public byte joystickClick;
    public float joystick2X;
    public float joystick2Y;
    public byte joystick2Click;

    // flexion[5][4] flattened to 5*4 = 20 floats
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
    public float[] flexion;

    // splay[5] = 5 floats
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
    public float[] splay;
}

public enum RadVRDriverInputV1
{
    Trigger,
    TriggerClick,
    Grip,
    GripClick,
    ButtonA,
    ButtonB,
    ButtonC,
    ButtonD,
    Calibrate,
    Menu,
    JoystickX,
    JoystickY,
    JoystickClick,
    Joystick2X,
    Joystick2Y,
    Joystick2Click,
}

public class RadVRController : DriverController<RadVRControllerDataV1, RadVRDriverInputV1>
{
    protected override string PipePath => "\\\\.\\pipe\\vrapplication\\input\\radvr\\v1\\";
    protected override string DriverName => "RadVR Driver";
    protected override string DriverVersion => "v1";

    public override RadVRControllerDataV1 CreateInput()
    {
        // This method should create a new RadVRControllerData struct with the current state of the controller.
        // For now, we'll just return an empty struct. You will need to fill this in with the actual data from the controller.
        return new RadVRControllerDataV1
        {
            header = 0x56444152, // 'RADV' in little-endian
            triggerValue = InputMappings.ContainsKey(RadVRDriverInputV1.Trigger) ? Convert.ToSingle(InputMappings[RadVRDriverInputV1.Trigger]) : 0.0f,
            gripValue = InputMappings.ContainsKey(RadVRDriverInputV1.Grip) ? Convert.ToSingle(InputMappings[RadVRDriverInputV1.Grip]) : 0.0f,
            triggerClick = InputMappings.ContainsKey(RadVRDriverInputV1.TriggerClick) ? Convert.ToByte(InputMappings[RadVRDriverInputV1.TriggerClick]) : (byte)0,
            gripClick = InputMappings.ContainsKey(RadVRDriverInputV1.GripClick) ? Convert.ToByte(InputMappings[RadVRDriverInputV1.GripClick]) : (byte)0,
            buttonA = InputMappings.ContainsKey(RadVRDriverInputV1.ButtonA) ? Convert.ToByte(InputMappings[RadVRDriverInputV1.ButtonA]) : (byte)0,
            buttonB = InputMappings.ContainsKey(RadVRDriverInputV1.ButtonB) ? Convert.ToByte(InputMappings[RadVRDriverInputV1.ButtonB]) : (byte)0,
            buttonC = InputMappings.ContainsKey(RadVRDriverInputV1.ButtonC) ? Convert.ToByte(InputMappings[RadVRDriverInputV1.ButtonC]) : (byte)0,
            buttonD = InputMappings.ContainsKey(RadVRDriverInputV1.ButtonD) ? Convert.ToByte(InputMappings[RadVRDriverInputV1.ButtonD]) : (byte)0,
            calibrate = InputMappings.ContainsKey(RadVRDriverInputV1.Calibrate) ? Convert.ToByte(InputMappings[RadVRDriverInputV1.Calibrate]) : (byte)0,
            menu = InputMappings.ContainsKey(RadVRDriverInputV1.Menu) ? Convert.ToByte(InputMappings[RadVRDriverInputV1.Menu]) : (byte)0,
            joystickX = InputMappings.ContainsKey(RadVRDriverInputV1.JoystickX) ? Convert.ToSingle(InputMappings[RadVRDriverInputV1.JoystickX]) : 0.0f,
            joystickY = InputMappings.ContainsKey(RadVRDriverInputV1.JoystickY) ? Convert.ToSingle(InputMappings[RadVRDriverInputV1.JoystickY]) : 0.0f,
            joystickClick = InputMappings.ContainsKey(RadVRDriverInputV1.JoystickClick) ? Convert.ToByte(InputMappings[RadVRDriverInputV1.JoystickClick]) : (byte)0,
            joystick2X = InputMappings.ContainsKey(RadVRDriverInputV1.Joystick2X) ? Convert.ToSingle(InputMappings[RadVRDriverInputV1.Joystick2X]) : 0.0f,
            joystick2Y = InputMappings.ContainsKey(RadVRDriverInputV1.Joystick2Y) ? Convert.ToSingle(InputMappings[RadVRDriverInputV1.Joystick2Y]) : 0.0f,
            joystick2Click = InputMappings.ContainsKey(RadVRDriverInputV1.Joystick2Click) ? Convert.ToByte(InputMappings[RadVRDriverInputV1.Joystick2Click]) : (byte)0,
            flexion = new float[20],
            splay = new float[5]
        };
    }
}   