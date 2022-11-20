using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Input
{
    public class EditorInput : Input
    {
        public Button leftClick;
        public Button rightClick;
        public Button middleClick;

        public AnalogButton mouseX;
        public AnalogButton mouseY;
        public AnalogButton mouseScroll;

        public EditorInput()
        {
            leftClick = new Button(0);
            rightClick = new Button(1);
            middleClick = new Button(2);

            mouseX = new AnalogButton("Mouse X");
            mouseY = new AnalogButton("Mouse Y");
            mouseScroll = new AnalogButton("Mouse ScrollWheel");
        }

        public override void Update()
        {
            leftClick.Update();
            rightClick.Update();
            middleClick.Update();

            mouseX.Update();
            mouseY.Update();
            mouseScroll.Update();

            SendInput();
        }
    }
}