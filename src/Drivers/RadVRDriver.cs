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
    Flexion,
    Splay
}

public class RadVRController : DriverController<RadVRControllerDataV1, RadVRDriverInputV1>
{
    protected override string PipePath => @"vrapplication\input\radvr\v1\";
    protected override string DriverName => "RadVR Driver";
    protected override string DriverVersion => "v1";

    public override (RadVRControllerDataV1, RadVRControllerDataV1) CreateInput()
    {
        RadVRControllerDataV1 lefthand = new RadVRControllerDataV1
        {
            header = 0x56444152, // 'RADV' in little-endian
            triggerValue = InputMappingsLeft.ContainsKey(RadVRDriverInputV1.Trigger) ? Convert.ToSingle(InputMappingsLeft[RadVRDriverInputV1.Trigger]) : 0.0f,
            gripValue = InputMappingsLeft.ContainsKey(RadVRDriverInputV1.Grip) ? Convert.ToSingle(InputMappingsLeft[RadVRDriverInputV1.Grip]) : 0.0f,
            triggerClick = InputMappingsLeft.ContainsKey(RadVRDriverInputV1.TriggerClick) ? Convert.ToByte(InputMappingsLeft[RadVRDriverInputV1.TriggerClick]) : (byte)0,
            gripClick = InputMappingsLeft.ContainsKey(RadVRDriverInputV1.GripClick) ? Convert.ToByte(InputMappingsLeft[RadVRDriverInputV1.GripClick]) : (byte)0,
            buttonA = InputMappingsLeft.ContainsKey(RadVRDriverInputV1.ButtonA) ? Convert.ToByte(InputMappingsLeft[RadVRDriverInputV1.ButtonA]) : (byte)0,
            buttonB = InputMappingsLeft.ContainsKey(RadVRDriverInputV1.ButtonB) ? Convert.ToByte(InputMappingsLeft[RadVRDriverInputV1.ButtonB]) : (byte)0,
            buttonC = InputMappingsLeft.ContainsKey(RadVRDriverInputV1.ButtonC) ? Convert.ToByte(InputMappingsLeft[RadVRDriverInputV1.ButtonC]) : (byte)0,
            buttonD = InputMappingsLeft.ContainsKey(RadVRDriverInputV1.ButtonD) ? Convert.ToByte(InputMappingsLeft[RadVRDriverInputV1.ButtonD]) : (byte)0,
            calibrate = InputMappingsLeft.ContainsKey(RadVRDriverInputV1.Calibrate) ? Convert.ToByte(InputMappingsLeft[RadVRDriverInputV1.Calibrate]) : (byte)0,
            menu = InputMappingsLeft.ContainsKey(RadVRDriverInputV1.Menu) ? Convert.ToByte(InputMappingsLeft[RadVRDriverInputV1.Menu]) : (byte)0,
            joystickX = InputMappingsLeft.ContainsKey(RadVRDriverInputV1.JoystickX) ? Convert.ToSingle(InputMappingsLeft[RadVRDriverInputV1.JoystickX]) : 0.0f,
            joystickY = InputMappingsLeft.ContainsKey(RadVRDriverInputV1.JoystickY) ? Convert.ToSingle(InputMappingsLeft[RadVRDriverInputV1.JoystickY]) : 0.0f,
            joystickClick = InputMappingsLeft.ContainsKey(RadVRDriverInputV1.JoystickClick) ? Convert.ToByte(InputMappingsLeft[RadVRDriverInputV1.JoystickClick]) : (byte)0,
            joystick2X = InputMappingsLeft.ContainsKey(RadVRDriverInputV1.Joystick2X) ? Convert.ToSingle(InputMappingsLeft[RadVRDriverInputV1.Joystick2X]) : 0.0f,
            joystick2Y = InputMappingsLeft.ContainsKey(RadVRDriverInputV1.Joystick2Y) ? Convert.ToSingle(InputMappingsLeft[RadVRDriverInputV1.Joystick2Y]) : 0.0f,
            joystick2Click = InputMappingsLeft.ContainsKey(RadVRDriverInputV1.Joystick2Click) ? Convert.ToByte(InputMappingsLeft[RadVRDriverInputV1.Joystick2Click]) : (byte)0,
            flexion = new float[20],
            splay = new float[5]
        };
        RadVRControllerDataV1 righthand = new RadVRControllerDataV1
        {
            header = 0x56444152, // 'RADV' in little-endian
            triggerValue = InputMappingsRight.ContainsKey(RadVRDriverInputV1.Trigger) ? Convert.ToSingle(InputMappingsRight[RadVRDriverInputV1.Trigger]) : 0.0f,
            gripValue = InputMappingsRight.ContainsKey(RadVRDriverInputV1.Grip) ? Convert.ToSingle(InputMappingsRight[RadVRDriverInputV1.Grip]) : 0.0f,
            triggerClick = InputMappingsRight.ContainsKey(RadVRDriverInputV1.TriggerClick) ? Convert.ToByte(InputMappingsRight[RadVRDriverInputV1.TriggerClick]) : (byte)0,
            gripClick = InputMappingsRight.ContainsKey(RadVRDriverInputV1.GripClick) ? Convert.ToByte(InputMappingsRight[RadVRDriverInputV1.GripClick]) : (byte)0,
            buttonA = InputMappingsRight.ContainsKey(RadVRDriverInputV1.ButtonA) ? Convert.ToByte(InputMappingsRight[RadVRDriverInputV1.ButtonA]) : (byte)0,
            buttonB = InputMappingsRight.ContainsKey(RadVRDriverInputV1.ButtonB) ? Convert.ToByte(InputMappingsRight[RadVRDriverInputV1.ButtonB]) : (byte)0,
            buttonC = InputMappingsRight.ContainsKey(RadVRDriverInputV1.ButtonC) ? Convert.ToByte(InputMappingsRight[RadVRDriverInputV1.ButtonC]) : (byte)0,
            buttonD = InputMappingsRight.ContainsKey(RadVRDriverInputV1.ButtonD) ? Convert.ToByte(InputMappingsRight[RadVRDriverInputV1.ButtonD]) : (byte)0,
            calibrate = InputMappingsRight.ContainsKey(RadVRDriverInputV1.Calibrate) ? Convert.ToByte(InputMappingsRight[RadVRDriverInputV1.Calibrate]) : (byte)0,
            menu = InputMappingsRight.ContainsKey(RadVRDriverInputV1.Menu) ? Convert.ToByte(InputMappingsRight[RadVRDriverInputV1.Menu]) : (byte)0,
            joystickX = InputMappingsRight.ContainsKey(RadVRDriverInputV1.JoystickX) ? Convert.ToSingle(InputMappingsRight[RadVRDriverInputV1.JoystickX]) : 0.0f,
            joystickY = InputMappingsRight.ContainsKey(RadVRDriverInputV1.JoystickY) ? Convert.ToSingle(InputMappingsRight[RadVRDriverInputV1.JoystickY]) : 0.0f,
            joystickClick = InputMappingsRight.ContainsKey(RadVRDriverInputV1.JoystickClick) ? Convert.ToByte(InputMappingsRight[RadVRDriverInputV1.JoystickClick]) : (byte)0,
            joystick2X = InputMappingsRight.ContainsKey(RadVRDriverInputV1.Joystick2X) ? Convert.ToSingle(InputMappingsRight[RadVRDriverInputV1.Joystick2X]) : 0.0f,
            joystick2Y = InputMappingsRight.ContainsKey(RadVRDriverInputV1.Joystick2Y) ? Convert.ToSingle(InputMappingsRight[RadVRDriverInputV1.Joystick2Y]) : 0.0f,
            joystick2Click = InputMappingsRight.ContainsKey(RadVRDriverInputV1.Joystick2Click) ? Convert.ToByte(InputMappingsRight[RadVRDriverInputV1.Joystick2Click]) : (byte)0,
            flexion = new float[20],
            splay = new float[5]
        };
        return (lefthand, righthand);
    }

    public override string GetDefaultMappingXbox()
    {
        // Return a default mapping configuration for 2 Xbox controllers (left and right) as a JSON string
        return @"[
        {
            ""LeftTrigger"": ""Trigger"",
            ""LeftGrip"": ""Grip"",
            ""LeftTriggerClick"": ""TriggerClick"",
            ""LeftGripClick"": ""GripClick"",
            ""DPadLeft"": ""ButtonA"",
            ""DPadUp"": ""ButtonB"",
            ""DPadDown"": ""ButtonC"",
            ""DPadRight"": ""ButtonD"",
            ""Back"": ""Calibrate"",
            ""Start"": ""Menu"",
            ""LeftThumbstickX"": ""JoystickX"",
            ""LeftThumbstickY"": ""JoystickY"",
            ""LeftThumbstickClick"": ""JoystickClick"",
            ""RighThumbstickX"": ""Joystick2X"",
            ""RightThumbstickY"": ""Joystick2Y"",
            ""RightThumbstickClick"": ""Joystick2Click""
            
        },
        {
            ""RightTrigger"": ""Trigger"",
            ""RightGrip"": ""Grip"",
            ""RightTriggerClick"": ""TriggerClick"",
            ""RightGripClick"": ""GripClick"",
            ""A"": ""ButtonA"",
            ""B"": ""ButtonB"",
            ""X"": ""ButtonC"",
            ""Y"": ""ButtonD"",
            ""Back"": ""Calibrate"",
            ""Start"": ""Menu"",
            ""RightThumbstickX"": ""JoystickX"",
            ""RightThumbstickY"": ""JoystickY"",
            ""RightThumbstickClick"": ""JoystickClick"",
            ""LeftThumbstickX"": ""Joystick2X"",
            ""LeftThumbstickY"": ""Joystick2Y"",
            ""LeftThumbstickClick"": ""Joystick2Click""
        }]";
    }
}   