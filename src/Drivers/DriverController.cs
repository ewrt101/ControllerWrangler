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
    public Dictionary<TEnum, object> InputMappings { get; }
    protected NamedPipeClientStream PipeClient { get; private set; }

    public DriverController()
    {
        Debug.WriteLine($"Initializing {DriverName} ({DriverVersion})");
        InputMappings = new Dictionary<TEnum, object>();
        PipeClient = new NamedPipeClientStream(".", PipePath, PipeDirection.InOut, PipeOptions.Asynchronous);
    }

    public abstract TData CreateInput(); //this method will be used to create a new input struct with the current state of the controller, such as which buttons are pressed and the position of the joysticks.

    public void Connect()
    {
        try
        {
            PipeClient.Connect(5000); // Wait up to 5 seconds to connect
            Debug.WriteLine($"Connected to {DriverName} ({DriverVersion})");
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
        if (PipeClient.IsConnected)
        {
            PipeClient.Close();
            Debug.WriteLine($"Disconnected from {DriverName} ({DriverVersion})");
        }
    }

    public void SendInput(TData input)
    {
        if (PipeClient.IsConnected)
        {
            try
            {
                // Serialize the input struct to bytes and send it through the pipe
                byte[] inputData = StructToBytes(input);
                PipeClient.Write(inputData, 0, inputData.Length);
                PipeClient.Flush();
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