using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LF2.Client
{
    public static class InputSerialization
    {

        public enum DirectionalInput  //Basic numpad directional notation
        { 
            DINPUT_UNKNOWN, //used when we don't recieve any input data from remote player for this frame
            DINPUT_DOWN_BACK, 
            DINPUT_DOWN,
            DINPUT_DOWN_FWD,
            DINPUT_BACK,
            DINPUT_NEUTRAL,
            DINPUT_FWD,
            DINPUT_UP_BACK,
            DINPUT_UP,
            DINPUT_UP_FWD

            /*
            * Numpad Notation:
            * 
            * 7 8 9 
            * 4 5 6 
            * 1 2 3 
            * 
            * 5 is neutral/no input, numbers about neutral denote direction (e.g 2 = DOWN, 9 = UP_RIGHT, 6 = FWD)
            */
        }
        



        public static DirectionalInput ConvertInputAxisToDirectionalInput(sbyte horizontal, sbyte vertical)
        {
            //if the values are not -1, 0, or 1 something has gone terribly wrong
            if (horizontal > 1 || horizontal < -1 || vertical > 1 || vertical < -1) return DirectionalInput.DINPUT_UNKNOWN;
            return DirectionalInput.DINPUT_NEUTRAL + horizontal + (3 * vertical); //see numpad notation above
            //e.g. if horizontal is -1 (back) and vertical is 1 (up) result = 5 - 1 + 3 = 7 (up+back)
        }

        public static (sbyte horizontal, sbyte vertical) ConvertDirectionalInputToAxis(DirectionalInput dir)
        {
            //reverse of above method, converts numpad notation to positive + vertical axis
            int d = (int)dir;
            int h = (d % 3) - 2; //horizontal
            if (System.Math.Abs(h) > 1) h = -System.Math.Sign(h); //"overflow" (2 should be -1, -2 should be 1)
            int v = (d - (int)DirectionalInput.DINPUT_NEUTRAL - h) / 3; //vertical
            return ((sbyte)h, (sbyte)v); //return as tuple

            /*
            *  example:
            *  d = 3 (down + forward)
            *  horizontal = (3 % 3) -2 = -2
            *  horizontal gets corrected to 1 (line 48)
            *  vertical = (3 - 5 - 1) / 3 = -3/3 = -1
            *  
            *  returns horizontal 1, vertical -1 
            *  we can check this against the reverse method, 5 + h + 3v = 5 + 1 - 3 = 3 which is the direction input we started with
            */
        }


        public static DirectionalInput FlipHorizonal(DirectionalInput d) //flips back inputs to fwd inputs and vice-versa - neutral input not affected
        {
            int i = (int)d;
            if (i % 3 == 1) i += 2;
            else if (i % 3 == 0) i -= 2;
            return (DirectionalInput)i;
        }

        public enum ButtonInputType
        {
            BINPUT_NONE, //no input for this button this frame
            BINPUT_PRESSED, //button was pressed this frame
            BINPUT_HELD, //button was pressed in a previous frame, and has not yet been released
            BINPUT_RELEASED, //button was released this frame (useful for negative edge)
            BINPUT_COUNT
        }

        public enum ButtonID
        {
            BUTTON_PUNCH,
            BUTTON_KICK,
            BUTTON_SLASH,
            BUTTON_HSLASH,
            BUTTON_COUNT
        }


        public class Inputs
        {
            public ushort FrameID;
            public DirectionalInput dir;
            public ButtonInputType[] buttons = new ButtonInputType[(int)ButtonInputType.BINPUT_COUNT];

            private string ButtonsToString()
            {
                string result = "";
                for(int i = 0; i < buttons.Length; ++i)
                {
                    result += "\n" + ((ButtonID)i).ToString() + " " + buttons[i].ToString();
                }
                return result;
            }

            enum INPUT_OFFSETS
            { 
                ID,
                BUTTONS = 2,
                DIRECTIONAL
            }

            public Inputs() { }
            public Inputs(ushort ID)
            {
                FrameID = ID;
                dir = DirectionalInput.DINPUT_UNKNOWN;
                buttons = new ButtonInputType[(int)ButtonID.BUTTON_COUNT];
            }

        }
    }

}
