using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mole
{
    enum Direction
    {
        Right, Left, Up, Down
    }

    public partial class GameWindow : Form
    {
        PlayerCharacter Player;
        Timer timer = new Timer();


        public GameWindow()
        {
            InitializeComponent();

            Player = new PlayerCharacter();

            timer.Interval = 100;
            timer.Tick += TimerTick;
            timer.Start();

            Player.Ending += this.GameEnd;
        }
       
        Image G_Character_Right = Mole.Properties.Resources.Character_right;
        Image G_Character_Left = Mole.Properties.Resources.Character_left;
        Image G_Character_Up = Mole.Properties.Resources.Character_up;
        Image G_Character_Down = Mole.Properties.Resources.Character_down;
        Image G_Object_1 = Mole.Properties.Resources.Object_1;
        Image G_Object_2 = Mole.Properties.Resources.Object_2;
        Image G_Mud_digged = Mole.Properties.Resources.Mud_digged;
        Image G_Mud_undigged = Mole.Properties.Resources.Mud_undigged;
        Image G_Barrier = Mole.Properties.Resources.Barrier;
        Image G_Exit = Mole.Properties.Resources.Exit;

        const int FieldWidth = 40;
        const int FieldHeight = 40;

        const int MAP_WIDTH = 20;
        const int MAP_HEIGHT = 14;
        
        int ObjectCount = 0;

        enum FieldStates
        {
            Undigged, Digged, Barrier, Object_1, Object_2, Exit
        }                

        FieldStates[,] Map = new FieldStates[MAP_WIDTH, MAP_HEIGHT];                

        private void MapGenerate()
        {
            Random numberGenerator = new Random();

            for(int X = 0; X < MAP_WIDTH; X++)
            {
                for(int Y = 0; Y < MAP_HEIGHT; Y++)
                {
                    Map[X, Y] = FieldStates.Undigged;
                }
            }

            int BarrierPlaced = 0;

            while(BarrierPlaced < 30)
            {
                int x = numberGenerator.Next(0, MAP_WIDTH);
                int y = numberGenerator.Next(0, MAP_HEIGHT);

                if(Map[x, y] == FieldStates.Undigged && (x != 0 && y != 0))
                {
                    BarrierPlaced++;
                    Map[x, y] = FieldStates.Barrier;
                }
            }

            Map[0, 0] = FieldStates.Digged;

            int ObjectPlaced = 0;

            while(ObjectPlaced < 10)
            {
                int x = numberGenerator.Next(0, MAP_WIDTH);
                int y = numberGenerator.Next(0, MAP_HEIGHT);

                if (Map[x, y] == FieldStates.Undigged && (x != 0 && y != 0))
                {
                    ObjectPlaced++;
                    ObjectCount++;

                    int ObjectType = numberGenerator.Next(0, 2);

                    if(ObjectType == 0)
                    {
                        Map[x, y] = FieldStates.Object_1;
                    }
                    else
                    {
                        Map[x, y] = FieldStates.Object_2;
                    }
                }
            }

            bool Searching = true;

            while (Searching)
            {
                int x = numberGenerator.Next(0, MAP_WIDTH -1);
                int y = numberGenerator.Next(0, MAP_HEIGHT - 1);

                if(Map[x, y] == FieldStates.Undigged && (Map[x -1, y] == FieldStates.Undigged || Map[x +1, y] == FieldStates.Undigged))
                {
                    Map[x, y] = FieldStates.Exit;
                    Searching = false;
                }
            }
        }

        private void MapDraw(Graphics g)
        {
            for (int X = 0; X < MAP_WIDTH; X++)
            {
                for (int Y = 0; Y < MAP_HEIGHT; Y++)
                {
                    switch(Map[X, Y])
                    {
                        case FieldStates.Undigged:
                            g.DrawImage(G_Mud_undigged, X * FieldWidth, Y * FieldHeight); break;
                        case FieldStates.Digged:
                            g.DrawImage(G_Mud_digged, X * FieldWidth, Y * FieldHeight); break;
                        case FieldStates.Barrier:
                            g.DrawImage(G_Barrier, X * FieldWidth, Y * FieldHeight); break;
                        case FieldStates.Exit:
                            g.DrawImage(G_Exit, X * FieldWidth, Y * FieldHeight); break;
                        case FieldStates.Object_1:
                            g.DrawImage(G_Object_1, X * FieldWidth, Y * FieldHeight); break;
                        case FieldStates.Object_2:
                            g.DrawImage(G_Object_2, X * FieldWidth, Y * FieldHeight); break;
                    }
                }
            }
        }

        private void PlayerDraw(Graphics g)
        {
            Image image;

            switch (Player.CharacterDirection)
            {
                case Direction.Right:
                    image = G_Character_Right;
                    break;
                case Direction.Left:
                    image = G_Character_Left;
                    break;
                case Direction.Up:
                    image = G_Character_Up;
                    break;
                case Direction.Down:
                    image = G_Character_Down;
                    break;
                default:
                    image = G_Character_Right;
                    break;
            }

            Bitmap bitmap = (Bitmap)image;
            bitmap.MakeTransparent(Color.White);

            g.DrawImage(image, Player.X * FieldWidth, Player.Y * FieldHeight);
        }

        private void GameWindow_Load(object sender, EventArgs e)
        {
            this.ClientSize = new Size(800, 600);
            this.BackColor = Color.Black;

            MapGenerate();
        }

        private void GameWindow_Paint(object sender, PaintEventArgs e)
        {
            MapDraw(e.Graphics);
            PlayerDraw(e.Graphics);
            TextDraw(e.Graphics);
        }

        private bool FieldCheck(int x, int y)
        {
            return Map[x, y] != FieldStates.Barrier;
        }

        private void GameEnd(string message)
        {            
            MessageBox.Show(message, "Game Labyrints");
            Application.Exit();
            timer.Stop();
        }

        private void TextDraw(Graphics g)
        {
            Font font = new Font("Arial", 9);

            g.DrawString("Steps: " + Player.StepCount, font, Brushes.White, 5, 560);
            g.DrawString("Object to pick: " + ObjectCount, font, Brushes.White, 5, 575);
        }

        private void GameWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.Right:
                        if (Player.X + 2 < MAP_WIDTH
                            && Map[Player.X + 1, Player.Y] == FieldStates.Barrier
                            && Map[Player.X + 2, Player.Y] == FieldStates.Digged)
                        {
                            Map[Player.X + 1, Player.Y] = FieldStates.Digged;
                            Map[Player.X + 2, Player.Y] = FieldStates.Barrier;
                        }
                        else if(Player.X + 1 < MAP_WIDTH
                            && Map[Player.X + 1, Player.Y] == FieldStates.Undigged)
                        {
                            Map[Player.X + 1, Player.Y] = FieldStates.Digged;
                        }
                        break;

                    case Keys.Left:
                        if (Player.X - 2 >= 0
                            && Map[Player.X - 1, Player.Y] == FieldStates.Barrier
                            && Map[Player.X - 2, Player.Y] == FieldStates.Digged)
                        {
                            Map[Player.X - 1, Player.Y] = FieldStates.Digged;
                            Map[Player.X - 2, Player.Y] = FieldStates.Barrier;
                        }
                        else if (Player.X - 1 < MAP_WIDTH
                            && Map[Player.X - 1, Player.Y] == FieldStates.Undigged)
                        {
                            Map[Player.X - 1, Player.Y] = FieldStates.Digged;
                        }
                        break;
                }
                this.Invalidate();
            }
            else
            {
                switch (e.KeyCode)
                {
                    case Keys.Right:
                        if (Player.X + 1 < MAP_WIDTH && FieldCheck(Player.X, Player.Y))
                        {
                            Player.X++;
                        }
                        break;

                    case Keys.Left:
                        if (Player.X - 1 >= 0 && FieldCheck(Player.X - 1, Player.Y))
                        {
                            Player.X--;
                        }
                        break;

                    case Keys.Up:
                        if (Player.Y - 1 >= 0 && FieldCheck(Player.X, Player.Y - 1))
                        {
                            Player.Y--;
                        }
                        break;

                    case Keys.Down:
                        if (Player.Y + 1 < MAP_HEIGHT && FieldCheck(Player.X, Player.Y + 1))
                        {
                            Player.Y++;
                        }
                        break;
                }

                if (Map[Player.X, Player.Y] == FieldStates.Undigged)
                {
                    Map[Player.X, Player.Y] = FieldStates.Digged;
                }

                if (Map[Player.X, Player.Y] == FieldStates.Object_1 || Map[Player.X, Player.Y] == FieldStates.Object_2)
                {
                    Map[Player.X, Player.Y] = FieldStates.Digged;
                    ObjectCount--;
                }

                if (Map[Player.X, Player.Y] == FieldStates.Exit && ObjectCount == 0)
                {
                    GameEnd("You win!");
                }

                this.Invalidate();
            }

            
        }

        private void TimerTick(object o, EventArgs e)
        {
            ObjectFallingCheck();
        }

        private void ObjectFallingCheck()
        {
            bool[,] fieldUsed = new bool[MAP_WIDTH, MAP_HEIGHT];

            for (int X = 0; X < MAP_WIDTH; X++)
            {
                for (int Y = 0; Y < MAP_HEIGHT; Y++)
                {
                    if (Y + 1 < MAP_HEIGHT)
                    {
                        if (Map[X, Y] == FieldStates.Barrier
                            && Map[X, Y + 1] == FieldStates.Digged
                            && fieldUsed[X, Y] == false)
                        {
                            fieldUsed[X, Y + 1] = true;
                            Map[X, Y] = FieldStates.Digged;
                            Map[X, Y + 1] = FieldStates.Barrier;
                            this.Invalidate();

                            if (Player.X == X && Player.Y == Y)
                            {
                                GameEnd("You lose!");
                                return;
                            }
                        }                        
                    }
                }

            }
        }
    }
}
