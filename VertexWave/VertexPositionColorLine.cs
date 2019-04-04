#region License

/* FNA - XNA4 Reimplementation for Desktop Platforms
 * Copyright 2009-2018 Ethan Lee and the MonoGame Team
 *
 * Released under the Microsoft Public License.
 * See LICENSE for details.
 */

#endregion

#region Using Statements

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace VertexWave
{
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionColorLine : IVertexType
    {
        #region Private Properties

        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        #endregion

        #region Public Variables

        public Vector3 Position;
        public Color Color;

        #endregion

        #region Public Static Variables

        public static readonly VertexDeclaration VertexDeclaration;

        #endregion

        #region Private Static Constructor

        static VertexPositionColorLine()
        {
            VertexDeclaration = new VertexDeclaration(new VertexElement(
                0,
                VertexElementFormat.Vector3,
                VertexElementUsage.Position,
                0
            ), new VertexElement(
                12,
                VertexElementFormat.Color,
                VertexElementUsage.Color,
                0
            ));
        }

        #endregion

        #region Public Constructor

        public VertexPositionColorLine(
            Vector3 position,
            Color color
        )
        {
            Position = position;
            Color = color;
        }

        #endregion

        #region Public Static Operators and Override Methods

        public override int GetHashCode()
        {
            // TODO: Fix GetHashCode
            return 0;
        }

        public override string ToString()
        {
            return "{{Position:" + Position +
                   " Color:" + Color +
                   "}}";
        }

        public static bool operator ==(VertexPositionColorLine left, VertexPositionColorLine right)
        {
            return left.Position == right.Position &&
                   left.Color == right.Color;
        }

        public static bool operator !=(VertexPositionColorLine left, VertexPositionColorLine right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (obj.GetType() != GetType()) return false;

            return this == (VertexPositionColorLine) obj;
        }

        #endregion
    }
}