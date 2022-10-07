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

    List<Button> buttons;
    public Input()
    {
        buttons = new List<Button>();

        forward = new Button(KeyCode.W);
        backward = new Button(KeyCode.S);
        left = new Button(KeyCode.A);
        right = new Button(KeyCode.D);

        turnLeft = new Button(KeyCode.Q);
        turnRight = new Button(KeyCode.E);

        confirm = new Button(KeyCode.Space);

        buttons.Add(forward);
        buttons.Add(backward);
        buttons.Add(left);
        buttons.Add(right);
        buttons.Add(turnLeft);
        buttons.Add(turnRight);
        buttons.Add(confirm);
    }
    public void Update()
    {
        foreach (Button button in buttons)
        {
            button.Update();
        }
    }
    public class Button
    {
        public Button(KeyCode key, bool wait = false)
        {
            keyCode = key;
            waitForConsume = wait;
        }
        KeyCode keyCode;
        public bool waitForConsume = false;
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
            if (UnityEngine.Input.GetKey(keyCode) || (Value == true && waitForConsume))
            {
                Value = true;
            }
            else
            {
                Value = false;
            }

            Pressed = oldValue == false && Value == true;
            Released = oldValue == true && Value == false;
        }
    }
}
