using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Mole
{
    class PlayerCharacter
    {

        private int stepCount = 0;
        private int x = 0, y = 0;

        public PlayerCharacter()
        {
            CharacterDirection = Direction.Right; ;
        }

        public int X
        {
            get
            {
                return x;
            }
            set
            {
                if (X > value)
                {
                    CharacterDirection = Direction.Left;
                }
                else
                {
                    CharacterDirection = Direction.Right;
                }

                StepCount++;
                x = value;
            }
        }

        public int Y
        {
            get
            {
                return y;
            }
            set
            {
                if (Y > value)
                {
                    CharacterDirection = Direction.Up;
                }
                else
                {
                    CharacterDirection = Direction.Down;
                }

                StepCount++;
                y = value;
            }
        }

        public int StepCount
        {
            get
            {
                return stepCount;
            }
            set
            {
                if (StepCount == 50)
                {
                    Ending("You loose!");
                }
                stepCount = value;
            }
        }        
        
        public Direction CharacterDirection
        {
            get;
            private set;
        }                  

        public Image Graphic
        {
            get;
            private set;
        }

        public delegate void delegateEndGame(string text);

        public event delegateEndGame Ending;
    }
}
