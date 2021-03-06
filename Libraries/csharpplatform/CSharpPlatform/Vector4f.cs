﻿using System.Runtime.InteropServices;

namespace CSharpPlatform
{
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
    public unsafe struct Vector4f
    {
        public float X, Y, Z, W;

        public static Vector4f Zero => new Vector4f(0, 0, 0, 0);

        public float this[int Index]
        {
            get
            {
                fixed (float* ValuesPtr = &X) return ValuesPtr[Index];
            }
            set
            {
                fixed (float* ValuesPtr = &X) ValuesPtr[Index] = value;
            }
        }

        public Vector4f(float X, float Y, float Z, float W)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.W = W;
        }

        public static Vector4f operator *(Vector4f Left, float Right)
        {
            return new Vector4f(
                Left.X * Right,
                Left.Y * Right,
                Left.Z * Right,
                Left.W * Right
            );
        }

        public static Vector4f operator +(Vector4f Left, Vector4f Right)
        {
            return new Vector4f(
                Left.X + Right.X,
                Left.Y + Right.Y,
                Left.Z + Right.Z,
                Left.W + Right.W
            );
        }

        public static Vector4f operator -(Vector4f Right)
        {
            return new Vector4f(
                -Right.X,
                -Right.Y,
                -Right.Z,
                -Right.W
            );
        }

        public static Vector4f Create(params float[] Values)
        {
            var Vector = default(Vector4f);
            for (int n = 0; n < 4; n++) Vector[n] = Values[n];
            return Vector;
        }

        public void FixValues(CallbackFloatPointer Callback)
        {
            fixed (float* Pointer = &this.X) Callback(Pointer);
        }

        //static public void Add(ref Vector4f Left, ref Vector4f Right, ref Vector4f Destination)
        //{
        //	for (int n = 0; n < 4; n++) Destination[n] = Left[n] + Right[n];
        //}

        public override string ToString()
        {
            return $"Vector4({X}, {Y}, {Z}, {W})";
        }
    }
}