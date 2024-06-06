﻿using System.Reactive.Linq;
using System.Runtime.CompilerServices;

namespace AllenNeuralDynamics.AindManipulator {
    public partial class ManipulatorPosition
    {
        public static ManipulatorPosition operator +(ManipulatorPosition el1, ManipulatorPosition el2)
        {
            return new ManipulatorPosition()
            {
                X = el1.X + el2.X,
                Y1 = el1.Y1 + el2.Y1,
                Y2 = el1.Y2 + el2.Y2,
                Z = el1.Z + el2.Z
            };
        }

        public static ManipulatorPosition operator -(ManipulatorPosition el1, ManipulatorPosition el2)
        {
            return new ManipulatorPosition()
            {
                X = el1.X - el2.X,
                Y1 = el1.Y1 - el2.Y1,
                Y2 = el1.Y2 - el2.Y2,
                Z = el1.Z - el2.Z
            };
        }

        public static ManipulatorPosition operator *(ManipulatorPosition el1, float gain)
        {
            return new ManipulatorPosition()
            {
                X = el1.X * gain,
                Y1 = el1.Y1 * gain,
                Y2 = el1.Y2 * gain,
                Z = el1.Z * gain
            };
        }
        public static ManipulatorPosition operator *(ManipulatorPosition el1, ManipulatorPosition el2)
        {
            return new ManipulatorPosition()
            {
                X = el1.X * el2.X,
                Y1 = el1.Y1 * el2.Y1,
                Y2 = el1.Y2 * el2.Y2,
                Z = el1.Z * el2.Z
            };

        }
        public override bool Equals(object obj)
        {
            if (obj is ManipulatorPosition)
            {
                var other = obj as ManipulatorPosition;
                return X == other.X && Y1 == other.Y1 && Y2 == other.Y2 && Z == other.Z;
            }
            else
            {
                return base.Equals(obj);
            }
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y1.GetHashCode() ^ Y2.GetHashCode() ^ Z.GetHashCode();
        }

        public static bool operator ==(ManipulatorPosition x, ManipulatorPosition y)
        {
            return Equals(x, y);
        }

        public static bool operator !=(ManipulatorPosition x, ManipulatorPosition y)
        {
            return !Equals(x, y);
        }

        public static bool Equals(ManipulatorPosition el1, ManipulatorPosition el2)
        {
            return el1.X == el2.X && el1.Y1 == el2.Y1 && el1.Y2 == el2.Y2 && el1.Z == el2.Z;
        }
    }

    public class AindManipulatorPosition
    {
        public double X;
        public double Y1;
        public double Y2;
        public double Z;

        public AindManipulatorPosition()
        {
            X = 0;
            Y1 = 0;
            Y2 = 0;
            Z = 0;
        }

        public AindManipulatorPosition(double y1, double y2, double x, double z)
        {
            X = x;
            Y1 = y1;
            Y2 = y2;
            Z = z;
        }

        public AindManipulatorPosition(ManipulatorPosition pos)
        {
            X = pos.X;
            Y1 = pos.Y1;
            Y2 = pos.Y2;
            Z = pos.Z;
        }

        public ManipulatorPosition ToManipulatorPosition()
        {
            return new ManipulatorPosition()
            {
                X = X,
                Y1 = Y1,
                Y2 = Y2,
                Z = Z
            };
        }

        public override bool Equals(object obj)
        {
            if (obj is AindManipulatorPosition)
            {
                var other = obj as AindManipulatorPosition;
                return X == other.X && Y1 == other.Y1 && Y2 == other.Y2 && Z == other.Z;
            }
            else {
                return base.Equals(obj); 
            }
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y1.GetHashCode() ^ Y2.GetHashCode() ^ Z.GetHashCode() ;
        }

        public static bool operator ==(AindManipulatorPosition x, AindManipulatorPosition y)
        {
            return Equals(x,y);
        }

        public static bool operator !=(AindManipulatorPosition x, AindManipulatorPosition y)
        {
            return !Equals(x, y);
        }

        public static bool Equals(AindManipulatorPosition el1, AindManipulatorPosition el2)
        {
            return el1.X == el2.X && el1.Y1 == el2.Y1 && el1.Y2 == el2.Y2 && el1.Z == el2.Z;
        }

    }
}