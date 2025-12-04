using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TPMapEditor.Utils
{
    public static class MathUtils
    {
        public static Vector3 Matrix33ToEulerXYZ(Vector3 X, Vector3 Y, Vector3 Z)
        {
            // Build the rotation matrix from basis vectors
            // Lines correspond to local X, Y, Z axes
            Matrix4x4 m = new Matrix4x4(
                X.X, X.Y, X.Z, 0,
                Y.X, Y.Y, Y.Z, 0,
                Z.X, Z.Y, Z.Z, 0,
                0, 0, 0, 1
            );

            // Extract angles (in radians)
            double sy = -m.M13;
            double cy = Math.Sqrt(1 - sy * sy);

            double x, y, z; // Euler angles in radians

            if (cy > 1e-6)
            {
                x = Math.Atan2(m.M23, m.M33);  // rotation around X
                y = Math.Asin(-m.M13);         // rotation around Y
                z = Math.Atan2(m.M12, m.M11);  // rotation around Z
            }
            else
            {
                // Gimbal lock case
                x = 0;
                y = Math.Asin(-m.M13);
                z = Math.Atan2(-m.M21, m.M22);
            }

            // Convert to degrees
            return new Vector3(
                (float)Math.Round(x * 180.0 / Math.PI),
                (float)Math.Round(y * 180.0 / Math.PI),
                (float)Math.Round(z * 180.0 / Math.PI)
            );
        }

        /// <summary>
        /// Build a 3x3 rotation matrix from Euler angles XYZ (degrees).
        /// </summary>
        public static double[,] EulerXYZToMatrix33(double rotXDeg, double rotYDeg, double rotZDeg)
        {
            double x = rotXDeg * Math.PI / 180;
            double y = rotYDeg * Math.PI / 180;
            double z = rotZDeg * Math.PI / 180;

            double cx = Math.Cos(x);
            double sx = Math.Sin(x);
            double cy = Math.Cos(y);
            double sy = Math.Sin(y);
            double cz = Math.Cos(z);
            double sz = Math.Sin(z);

            // Final rotation matrix R = Rz * Ry * Rx
            double[,] m = new double[3, 3];

            m[0, 0] = cz * cy;
            m[0, 1] = cz * sy * sx - sz * cx;
            m[0, 2] = cz * sy * cx + sz * sx;

            m[1, 0] = sz * cy;
            m[1, 1] = sz * sy * sx + cz * cx;
            m[1, 2] = sz * sy * cx - cz * sx;

            m[2, 0] = -sy;
            m[2, 1] = cy * sx;
            m[2, 2] = cy * cx;

            return m;
        }

        /// <summary>
        /// Y forward and Z up (just to remember)
        /// </summary>
        public static (int yaw, int pitch) Vector3ToYawPitch(Vector3 dir, bool clampPitch = true)
        {
            if (dir == Vector3.Zero)
                throw new ArgumentException("dir must not be the zero vector", nameof(dir));

            Vector3 n = Vector3.Normalize(dir);

            // yaw = atan2(X, Y)
            double yawRad = Math.Atan2(n.X, n.Y);

            // pitch = asin(-Z)
            double zClamped = Clamp(-n.Z, -1.0, 1.0);
            double pitchRad = Math.Asin(zClamped);

            double yawDeg = yawRad * 180.0 / Math.PI;
            double pitchDeg = pitchRad * 180.0 / Math.PI;

            if (clampPitch)
            {
                if (pitchDeg > 89.999) pitchDeg = 89.999;
                if (pitchDeg < -89.999) pitchDeg = -89.999;
            }

            yawDeg = NormalizeAngle(yawDeg);

            int yawInt = (int)Math.Round(yawDeg);
            int pitchInt = (int)Math.Round(pitchDeg);

            return (yawInt, pitchInt);
        }

        private static double NormalizeAngle(double angle)
        {
            angle = angle % 360f;
            if (angle >= 180f) angle -= 360f;
            if (angle < -180f) angle += 360f;
            return angle;
        }

        private static double Clamp(double v, double min, double max)
        {
            if (v < min) return min;
            if (v > max) return max;
            return v;
        }

        /// <summary>
        /// Y forward Z up
        /// </summary>
        /// <returns></returns>
        public static Vector3 YawPitchToVector3(int yawDeg, int pitchDeg)
        {
            // Convert to radians
            float yaw = yawDeg * (float)Math.PI / 180f;
            float pitch = pitchDeg * (float)Math.PI / 180f;

            // Precompute cos/sin
            float cosPitch = (float)Math.Cos(pitch);
            float sinPitch = (float)Math.Sin(pitch);
            float sinYaw = (float)Math.Sin(yaw);
            float cosYaw = (float)Math.Cos(yaw);

            // Build vector according to your coordinate system
            Vector3 dir = new Vector3(
                sinYaw * cosPitch,  // X = right
                cosYaw * cosPitch,  // Y = forward
                -sinPitch           // Z = up  (negative = pitch up)
            );

            return Vector3.Normalize(dir);
        }
    }
}
