using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Input
{
    public Button forward;
    public Button backward;
    public Button left;
    public Button right;

    public Button turnLeft;
    public Button turnRight;

    public Button confirm;
    public Button leftClick;
    public Button rightClick;

    public AnalogButton mouseX;
    public AnalogButton mouseY;
    public AnalogButton mouseScroll;

    List<Button> buttons;
    List<IInputReceiver> receivers;
    public Input()
    {
        buttons = new List<Button>();
        receivers = new List<IInputReceiver>();

        forward = new Button(KeyCode.W);
        backward = new Button(KeyCode.S);
        left = new Button(KeyCode.A);
        right = new Button(KeyCode.D);

        turnLeft = new Button(KeyCode.Q);
        turnRight = new Button(KeyCode.E);

        mouseX = new AnalogButton("Mouse X");
        mouseY = new AnalogButton("Mouse Y");
        mouseScroll = new AnalogButton("Mouse ScrollWheel");

        confirm = new Button(KeyCode.Space);
        leftClick = new Button(0);
        rightClick = new Button(1);

        buttons.Add(forward);
        buttons.Add(backward);
        buttons.Add(left);
        buttons.Add(right);
        buttons.Add(turnLeft);
        buttons.Add(turnRight);
        buttons.Add(confirm);
        buttons.Add(leftClick);
        buttons.Add(rightClick);
    }
    public void Update()
    {
        foreach (Button button in buttons)
        {
            button.Update();
        }

        mouseX.Update();
        mouseY.Update();
        mouseScroll.Update();

        // Sort input receivers by priority
        // Higher => First to get input
        receivers.Sort(delegate (IInputReceiver a, IInputReceiver b) { return b.GetInputPriority().CompareTo(a.GetInputPriority()); } );

        foreach (IInputReceiver receiver in receivers)
        {
            receiver.OnInputReceived(this);
        }
    }
    public void Subscribe(IInputReceiver receiver)
    {
        if (receivers.Contains(receiver) == false)
        {
            receivers.Add(receiver);
        }
    }
    public class Button
    {
        public Button(KeyCode key)
        {
            keyCode = key;
        }
        public Button(int mouse)
        {
            mouseIndex = mouse;
        }
        readonly public KeyCode keyCode = KeyCode.None;
        readonly public int mouseIndex = -1;
        public bool Value { private set; get; }
        public bool Pressed { private set; get; }
        public bool Released { private set; get; }
        bool oldValue;
        /// <summary>
        /// Sets the value to false and ends further invoking
        /// </summary>
        public void Consume()
        {
            Value = false;
            Released = false;
            Pressed = false;
        }
        public void Update()
        {
            oldValue = Value;
            bool keyDown = (keyCode != KeyCode.None && UnityEngine.Input.GetKey(keyCode)) || (mouseIndex != -1 && UnityEngine.Input.GetMouseButton(mouseIndex));

            Value = keyDown;

            Pressed = oldValue == false && Value == true;
            Released = oldValue == true && Value == false;
        }
    }
    public class AnalogButton
    {
        public AnalogButton(string axisName)
        {
            name = axisName;
        }
        readonly string name;
        public float Value { private set; get; }
        public void Update()
        {
            Value = UnityEngine.Input.GetAxisRaw(name);
        }
        public void Consume()
        {
            Value = 0.0f;
        }
    }
    public interface IInputReceiver
    {
        public void OnInputReceived(Input input);
        public int GetInputPriority() => 0;
    }
}
