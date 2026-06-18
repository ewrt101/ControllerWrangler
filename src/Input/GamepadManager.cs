using System;
using System.Collections.Generic;
using System.Linq;
using Silk.NET.SDL;

namespace ControllerWrangler.Input;

sealed class GamepadManager : IDisposable
{
    public record struct GamepadEvent(uint Id, string Name, GamepadEventKind Kind, object? Value);
    public enum GamepadEventKind { Connected, Disconnected, ButtonDown, ButtonUp, AxisMoved }

    readonly Sdl _sdl = Sdl.GetApi();
    readonly Dictionary<uint, nint> _pads = new();

    public GamepadManager()
    {
        if (_sdl.Init(Sdl.InitGamecontroller) < 0)
            throw new Exception(_sdl.GetErrorS());
    }

    public IEnumerable<(uint Id, string Name)> Gamepads() =>
        _pads.Select(kv => (kv.Key, GetName(kv.Value)));

    // Collect into a list first — yield return can't be inside unsafe blocks
    public IEnumerable<GamepadEvent> NextEvents()
    {
        var results = new List<GamepadEvent>();
        Event e = default;

        while (_sdl.PollEvent(ref e) != 0)
        {
            switch ((EventType)e.Type)
            {
                case EventType.Controllerdeviceadded:
                {
                    unsafe
                    {
                        var id  = (uint)e.Cdevice.Which;
                        var pad = _sdl.GameControllerOpen(e.Cdevice.Which);
                        _pads[id] = (nint)pad;
                        results.Add(new(id, GetName((nint)pad), GamepadEventKind.Connected, null));
                    }
                    break;
                }
                case EventType.Controllerdeviceremoved:
                {
                    var id = (uint)e.Cdevice.Which;
                    if (_pads.Remove(id, out var ptr))
                    {
                        unsafe { _sdl.GameControllerClose((GameController*)ptr); }
                        results.Add(new(id, "", GamepadEventKind.Disconnected, null));
                    }
                    break;
                }
                case EventType.Controllerbuttondown:
                case EventType.Controllerbuttonup:
                {
                    var id   = (uint)e.Cbutton.Which;
                    var btn  = (GameControllerButton)e.Cbutton.Button;
                    var kind = (EventType)e.Type == EventType.Controllerbuttondown
                        ? GamepadEventKind.ButtonDown : GamepadEventKind.ButtonUp;
                    results.Add(new(id, "", kind, btn));
                    break;
                }
                case EventType.Controlleraxismotion:
                {
                    var id   = (uint)e.Caxis.Which;
                    var axis = (GameControllerAxis)e.Caxis.Axis;
                    var norm = e.Caxis.Value / 32767f;
                    results.Add(new(id, "", GamepadEventKind.AxisMoved, (axis, norm)));
                    break;
                }
            }
        }

        return results;
    }

    public bool IsPressed(uint id, GameControllerButton button)
    {
        if (!_pads.TryGetValue(id, out var ptr)) return false;
        unsafe { return _sdl.GameControllerGetButton((GameController*)ptr, button) != 0; }
    }

    public float AxisValue(uint id, GameControllerAxis axis)
    {
        if (!_pads.TryGetValue(id, out var ptr)) return 0f;
        unsafe { return _sdl.GameControllerGetAxis((GameController*)ptr, axis) / 32767f; }
    }

    string GetName(nint ptr)
    {
        unsafe { return _sdl.GameControllerNameS((GameController*)ptr) ?? "Unknown"; }
    }

    public void Dispose()
    {
        foreach (var ptr in _pads.Values)
            unsafe { _sdl.GameControllerClose((GameController*)ptr); }
        _sdl.Quit();
        _sdl.Dispose();
    }
}