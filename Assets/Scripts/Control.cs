using UnityEngine;

public class Controls
{
    public string horizontalAxis;
    public string verticalAxis;
    public string[] attackButtons;

    public ControllerType controllerType;

    public Controls(ControllerType _ct)
    {
        controllerType = _ct;
        horizontalAxis = "Horizontal-" + controllerType.ToString();
        verticalAxis = "Vertical-" + controllerType.ToString();
        attackButtons = new string[] { "Fire1-" + controllerType.ToString(), "Fire2-" + controllerType.ToString(), "Fire3-" + controllerType.ToString() };
    }

}

public enum ControllerType
{
    keyboard, keyboard_alt, joystick1, joystick2
};