using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace ControllerWrangler.Driver;


//base class for controller drivers
public abstract class DriverController<TData, TEnum> where TData : struct where TEnum : Enum
{
    //this class will handle sending data to and from the steam VR controller driver, such as button presses and haptic feedback.
    
    protected abstract string PipePath { get; }
    protected abstract string DriverName { get; }
    protected abstract string DriverVersion { get; }
    public Dictionary<TEnum, object> InputMappingsLeft { get; }
    public Dictionary<TEnum, object> InputMappingsRight { get; }
    protected NamedPipeClientStream PipeClientLeft { get; private set; }
    protected NamedPipeClientStream PipeClientRight { get; private set; }

    public DriverController()
    {
        Debug.WriteLine($"Initializing {DriverName} ({DriverVersion})");
        InputMappingsLeft = new Dictionary<TEnum, object>();
        InputMappingsRight = new Dictionary<TEnum, object>();
        PipeClientLeft = new NamedPipeClientStream(".", PipePath+"left", PipeDirection.Out, PipeOptions.Asynchronous);
        PipeClientRight = new NamedPipeClientStream(".", PipePath+"right", PipeDirection.Out, PipeOptions.Asynchronous);
    }

    public abstract (TData,TData) CreateInput(); //this method will be used to create a new input struct with the current state of the controller, such as which buttons are pressed and the position of the joysticks.

    public abstract String GetDefaultMappingXbox();

    public void Connect()
    {
        try
        {
            PipeClientLeft.Connect(5000); // Wait up to 5 seconds to connect
            PipeClientRight.Connect(5000); // Wait up to 5 seconds to connect
            Debug.WriteLine($"Connected to {DriverName} ({DriverVersion}) on pipe path: {PipePath}");
        }
        catch (TimeoutException)
        {
            Debug.WriteLine($"Failed to connect to {DriverName} ({DriverVersion}): Connection timed out.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to connect to {DriverName} ({DriverVersion}): {ex.Message}");
        }
    }

    public void Disconnect()
    {
        if (PipeClientLeft.IsConnected)
        {
            PipeClientLeft.Close();
        }
        if (PipeClientRight.IsConnected)
        {
            PipeClientRight.Close();
        }
        Debug.WriteLine($"Disconnected from {DriverName} ({DriverVersion})");
    }

    public void SendInput(TData inputLeft, TData inputRight)
    {
        if (PipeClientLeft.IsConnected)
        {
            try
            {
                // Serialize the input struct to bytes and send it through the pipe
                //left
                byte[] inputData = StructToBytes(inputLeft);
                PipeClientLeft.Write(inputData, 0, inputData.Length);
                PipeClientLeft.Flush();
                //right
                inputData = StructToBytes(inputRight);
                PipeClientRight.Write(inputData, 0, inputData.Length);
                PipeClientRight.Flush();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to send input to {DriverName} ({DriverVersion}): {ex.Message}");
            }
        }
        else
        {
            Debug.WriteLine($"Cannot send input: Not connected to {DriverName} ({DriverVersion}).");
        }
    }

    private byte[] StructToBytes(TData data)
    {
        int size = Marshal.SizeOf(data);
        byte[] bytes = new byte[size];
        IntPtr ptr = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.StructureToPtr(data, ptr, true);
            Marshal.Copy(ptr, bytes, 0, size);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
        return bytes;
    }

}

/*
//OpenGloveV1 driver controller
public class OpenGloveV1Controller : DriverController<DriverData, DriverInput>
{
    protected override string PipePath => "\\\\.\\pipe\\vrapplication\\input\\glove\\v1\\";
    protected override string DriverName => "OpenGlove Driver";
    protected override string DriverVersion => "v1";

    //TODO: Define the DriverInput struct for the OpenGloveV1 driver based on its expected input format.
}

//OpenGloveV2 driver controller
public class OpenGloveV2Controller : DriverController<DriverData, DriverInput>
{
    protected override string PipePath => "\\\\.\\pipe\\vrapplication\\input\\glove\\v2\\";
    protected override string DriverName => "OpenGlove Driver";
    protected override string DriverVersion => "v2";

    //TODO: Define the DriverInput struct for the OpenGloveV2 driver based on its expected input format.
}
*/