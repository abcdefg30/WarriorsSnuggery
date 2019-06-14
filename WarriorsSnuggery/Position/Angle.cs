﻿using System;

namespace WarriorsSnuggery
{
	public struct VAngle
	{
		public readonly float X;
		public readonly float Y;
		public readonly float Z;

		public static readonly VAngle Zero = new VAngle();

		public VAngle(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public VAngle(int xDeg, int yDeg, int zDeg)
		{
			const float u = (float) (Math.PI / 180);
			X = u * xDeg;
			Y = u * yDeg;
			Z = u * zDeg;
		}

		public static VAngle operator +(VAngle lhs, VAngle rhs) { return new VAngle(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z); }

		public static VAngle operator -(VAngle lhs, VAngle rhs) { return new VAngle(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z); }

		public static VAngle operator *(VAngle lhs, VAngle rhs) { return new VAngle(lhs.X * rhs.X, lhs.Y * rhs.Y, lhs.Z * rhs.Z); }

		public static VAngle operator /(VAngle lhs, VAngle rhs) { return new VAngle(lhs.X / rhs.X, lhs.Y / rhs.Y, lhs.Z / rhs.Z); }

		public static VAngle operator -(VAngle pos) { return new VAngle(-pos.X, -pos.Y, -pos.Z); }

		public static bool operator ==(VAngle lhs, VAngle rhs) { return lhs.X == rhs.X && lhs.Y == rhs.Y && lhs.Z == rhs.Z; }

		public static bool operator !=(VAngle lhs, VAngle rhs) { return !(lhs == rhs); }

		public bool Equals(VAngle pos) { return pos == this; }
		public override bool Equals(object obj) { return obj is VAngle && Equals((VAngle)obj); }

		public override int GetHashCode() { return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode(); }

		public override string ToString() { return X + "," + Y + "," + Z + " [Radians]"; }

		public CPos ToDegree()
		{
			const float u = (float)(180 / Math.PI);
			var x = (int)(u * X);
			var y = (int)(u * Y);
			var z = (int)(u * Z);

			return new CPos(x, y, z);
		}

		public static implicit operator OpenTK.Vector4(VAngle angle)
		{
			return new OpenTK.Vector4(angle.X, angle.Y, angle.Z, 0f);
		}
	}
}
