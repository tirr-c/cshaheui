using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CShAheui.App
{
    public class Cursor : ICloneable
    {
        public enum Direction
        {
            Up, Down, Right, Left,
        }

        public class Velocity : ICloneable
        {
            public Direction Direction { get; set; }
            public int Speed { get; set; }

            public Velocity()
            {
                Direction = Cursor.Direction.Down;
                Speed = 1;
            }

            public void Reverse()
            {
                switch (Direction)
                {
                    case Cursor.Direction.Up:
                        Direction = Cursor.Direction.Down;
                        break;
                    case Cursor.Direction.Down:
                        Direction = Cursor.Direction.Up;
                        break;
                    case Cursor.Direction.Right:
                        Direction = Cursor.Direction.Left;
                        break;
                    case Cursor.Direction.Left:
                        Direction = Cursor.Direction.Right;
                        break;
                }
            }

            public void Update(char aheuiDirection, bool reversed = false)
            {
                if (aheuiDirection == 'ㅡ')
                {
                    if (Direction == Cursor.Direction.Up) Direction = Cursor.Direction.Down;
                    else if (Direction == Cursor.Direction.Down) Direction = Cursor.Direction.Up;
                }
                else if (aheuiDirection == 'ㅣ')
                {
                    if (Direction == Cursor.Direction.Right) Direction = Cursor.Direction.Left;
                    else if (Direction == Cursor.Direction.Left) Direction = Cursor.Direction.Right;
                }
                else if (aheuiDirection == 'ㅢ')
                {
                    if (Direction == Cursor.Direction.Up) Direction = Cursor.Direction.Down;
                    else if (Direction == Cursor.Direction.Down) Direction = Cursor.Direction.Up;
                    else if (Direction == Cursor.Direction.Right) Direction = Cursor.Direction.Left;
                    else if (Direction == Cursor.Direction.Left) Direction = Cursor.Direction.Right;
                }
                else
                {
                    switch (aheuiDirection)
                    {
                        case 'ㅏ':
                        case 'ㅑ':
                            Direction = Cursor.Direction.Right;
                            break;
                        case 'ㅓ':
                        case 'ㅕ':
                            Direction = Cursor.Direction.Left;
                            break;
                        case 'ㅗ':
                        case 'ㅛ':
                            Direction = Cursor.Direction.Up;
                            break;
                        case 'ㅜ':
                        case 'ㅠ':
                            Direction = Cursor.Direction.Down;
                            break;
                    }
                    switch (aheuiDirection)
                    {
                        case 'ㅏ':
                        case 'ㅓ':
                        case 'ㅗ':
                        case 'ㅜ':
                            Speed = 1;
                            break;
                        case 'ㅑ':
                        case 'ㅕ':
                        case 'ㅛ':
                        case 'ㅠ':
                            Speed = 2;
                            break;
                    }
                }
                if (reversed)
                {
                    if (Direction == Cursor.Direction.Up) Direction = Cursor.Direction.Down;
                    else if (Direction == Cursor.Direction.Down) Direction = Cursor.Direction.Up;
                    else if (Direction == Cursor.Direction.Right) Direction = Cursor.Direction.Left;
                    else if (Direction == Cursor.Direction.Left) Direction = Cursor.Direction.Right;
                }
            }

            public object Clone()
            {
                Velocity ret = new Velocity();
                ret.Direction = Direction;
                ret.Speed = Speed;
                return ret;
            }
        }

        public Velocity V { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }

        public Cursor()
        {
            X = Y = 0;
            V = new Velocity();
        }

        public void Move(string[] code, char aheui, bool reversed = false)
        {
            int height = code.Length;
            V.Update(aheui, reversed);
            int effSpeed = V.Speed;
            switch (V.Direction)
            {
                case Direction.Up:
                    effSpeed = -1;
                    break;
                case Direction.Down:
                    effSpeed = 1;
                    break;
                case Direction.Left:
                    effSpeed *= -1;
                    break;
            }
            if (V.Direction == Direction.Up || V.Direction == Direction.Down)
            {
                int width;
                for (int i = 0; i < V.Speed; i++)
                {
                    do
                    {
                        Y += effSpeed;
                        Y %= height;
                        if (Y < 0) Y += height;
                        width = code[Y].Length;
                    } while (X < 0 && X >= width);
                }
            }
            else
            {
                int width = code[Y].Length;
                X += effSpeed;
                X %= width;
                if (X < 0) X += width;
            }
        }

        public object Clone()
        {
            Cursor ret = new Cursor();
            ret.X = X;
            ret.Y = Y;
            ret.V = V.Clone() as Velocity;
            return ret;
        }
    }
}
