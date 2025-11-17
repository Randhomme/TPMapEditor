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
        public static Vector3 GetEulerXYZ(Vector3 X, Vector3 Y, Vector3 Z)
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
        /// Y forward and Z up (just to remember)
        /// </summary>
        public static (int yaw, int pitch) GetYawPitch(Vector3 dir)
        {
            dir = Vector3.Normalize(dir);

            double yaw = Math.Atan2(dir.X, dir.Y);  // rotate around Z (horizontal)
            double pitch = Math.Asin(-dir.Z);         // rotate around X (vertical)

            yaw *= 180f / Math.PI;
            pitch *= 180f / Math.PI;

            yaw = NormalizeAngle(yaw);

            int yawInt = (int)Math.Round(yaw);
            int pitchInt = (int)Math.Round(pitch);

            return (yawInt, pitchInt);
        }

        private static double NormalizeAngle(double angle)
        {
            angle = angle % 360f;
            if (angle >= 180f) angle -= 360f;
            if (angle < -180f) angle += 360f;
            return angle;
        }
    }
}
