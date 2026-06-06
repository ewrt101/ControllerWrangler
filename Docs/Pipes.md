# info for the named pipes of the various drivers
## [Open Gloves VR](https://github.com/LucidVR/opengloves-driver/wiki/Driver-Input)
```
// "\\.\pipe\vrapplication\input\glove\v1\<left/right>"
struct InputData {
  const std::array<std::array<float, 4>, 5> flexion;
  const std::array<float, 5> splay;
  const float joyX;
  const float joyY;
  const bool joyButton;
  const bool trgButton;
  const bool aButton;
  const bool bButton;
  const bool grab;
  const bool pinch;
  const bool menu;
  const bool calibrate;
};

// "\\.\pipe\vrapplication\input\glove\v2\<left/right>"
struct InputData {
  const std::array<std::array<float, 4>, 5> flexion;
  const std::array<float, 5> splay;
  const float joyX;
  const float joyY;
  const bool joyButton;
  const bool trgButton;
  const bool aButton;
  const bool bButton;
  const bool grab;
  const bool pinch;
  const bool menu;
  const bool calibrate;

  const float trgValue;
};
```

## RadVR driver
```
// "\\.\pipe\vrapplication\input\radvr\v1\<left/right>"
#pragma pack(push, 1)
struct RadVRInputData
{
    uint32_t header;          // Must be 0x56444152 ('R', 'A', 'D', 'V')

    float    triggerValue;    // 0.0 (released) to 1.0 (pulled)
    float    gripValue;       // 0.0 (open) to 1.0 (squeezed)
    uint8_t  triggerClick;    // 0 = released, 1 = pressed
    uint8_t  gripClick;       // 0 = released, 1 = pressed
    uint8_t  buttonA;
    uint8_t  buttonB;
    uint8_t  buttonC;
    uint8_t  buttonD;
    uint8_t  calibrate;       // reserved (read but not used yet)
    uint8_t  menu;            // SteamVR overlay / system button
    float    joystickX;       // -1.0 (left) to 1.0 (right)
    float    joystickY;       // -1.0 (down) to 1.0 (up)
    uint8_t  joystickClick;
    float    joystick2X;      // -1.0 (left) to 1.0 (right)
    float    joystick2Y;      // -1.0 (down) to 1.0 (up)
    uint8_t  joystick2Click;

    float    flexion[5][4];   // finger curl: [thumb..pinky][joint0..3], 0=open 1=fist
    float    splay[5];        // finger splay: [thumb..pinky], 0.5=neutral
};
#pragma pack(pop)
```