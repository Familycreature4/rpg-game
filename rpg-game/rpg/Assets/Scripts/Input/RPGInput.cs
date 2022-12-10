using System;
using System.Collections.Generic;
using UnityEngine;

namespace Input
{
    /// <summary>
    /// Input handler for the game
    /// </summary>
    public class RPGInput : Input
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
        public Button middleClick;

        public Button shift;

        public AnalogButton mouseX;
        public AnalogButton mouseY;
        public AnalogButton mouseScroll;
        public RPGInput()
        {
            forward = new Button(KeyCode.W);
            backward = new Button(KeyCode.S);
            left = new Button(KeyCode.A);
            right = new Button(KeyCode.D);

            turnLeft = new Button(KeyCode.Q);
            turnRight = new Button(KeyCode.E);

            shift = new Button(KeyCode.LeftShift);

            mouseX = new AnalogButton("Mouse X");
            mouseY = new AnalogButton("Mouse Y");
            mouseScroll = new AnalogButton("Mouse ScrollWheel");

            confirm = new Button(KeyCode.Space);
            leftClick = new Button(0);
            rightClick = new Button(1);
            middleClick = new Button(2);
        }
        public override void Update()
        {
            forward.Update();
            backward.Update();
            left.Update();
            right.Update();
            shift.Update();

            turnLeft.Update();
            turnRight.Update();

            confirm.Update();
            leftClick.Update();
            rightClick.Update();
            middleClick.Update();

            mouseX.Update();
            mouseY.Update();
            mouseScroll.Update();

            SendInput();
        }
        public override void SendInput()
        {
            // Publish OnInput event

            EventManager.OnInput.Invoke(this);
        }
    }
}