using ControllerWrangler.Driver;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

namespace ControllerWrangler.Input;


public enum DriverType
{
    RadVRV1,
    OpenGlovesV1,
    OpenGlovesV2
}

public class InputManagement
{
    //mark driver type
    public DriverType CurrentDriver { get; private set; }
    //this class will handle managing the input from the controllers, such as polling for input and sending it to the driver controller to be sent to the steam VR controller driver.
    private dynamic driverController;

    private Thread inputThread;

    public InputManagement(DriverType driverType)
    {
        CurrentDriver = driverType;
        switch (driverType)
        {
            case DriverType.RadVRV1:
                driverController = new RadVRController();
                break;
            default:
                throw new NotImplementedException($"Driver type {driverType} is not implemented.");
        }
    }

    public void Start()
    {   

        LoadMappings();
        driverController.Connect();

        //create a new thread for polling input so it doesn't block the main thread
        inputThread = new Thread(() =>
        {
            while (true)
            {
                //set dictionsary values to the current state of the controller inputs
                //TODO

                //send the input to the driver controller to be sent to the steam VR controller driver
                var input = driverController.CreateInput();
                driverController.SendInput(input.Item1, input.Item2);
                Thread.Sleep(10); // Poll every 10ms (100Hz)
            }
        });
        inputThread.IsBackground = true;
        inputThread.Start();
    }

    public void Stop()
    {
        //stop the input thread
        if (inputThread != null && inputThread.IsAlive)
        {
            inputThread.Abort(); //TODO: change to so it works with modern .NET
        }

        driverController.Disconnect();
    }

    //load the input mappings from a config file
    private void LoadMappings()
    {
        //create mapping folder beside the executable if it doesn't exist
        string mappingFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mappings");
        if (!Directory.Exists(mappingFolder))
        {
            Debug.WriteLine($"Creating mapping folder at: {mappingFolder}");
            Directory.CreateDirectory(mappingFolder);
        }
        //create a mapping file for the current driver if it doesn't exist
        string mappingFile = Path.Combine(mappingFolder, $"{CurrentDriver}.json");
        if (!File.Exists(mappingFile))
        {
            Debug.WriteLine($"Creating mapping file at: {mappingFile}");
            File.WriteAllText(mappingFile, driverController.GetDefaultMappingXbox());
        }
    }
}