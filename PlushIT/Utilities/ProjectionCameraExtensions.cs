using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace PlushIT.Utilities
{
    public static class ProjectionCameraExtensions
    {
        public static TCamera Move<TCamera>(this TCamera camera, Vector3D axis, double step)
            where TCamera : ProjectionCamera
        {
            camera.Position += axis * step;
            return camera;
        }

        public static TCamera Rotate<TCamera>(this TCamera camera, Vector3D axis, double angle)
            where TCamera : ProjectionCamera
        {
            Matrix3D matrix3D = new();
            matrix3D.RotateAt(new(axis, angle), camera.Position);
            camera.LookDirection *= matrix3D;
            return camera;
        }

        public static Vector3D GetYawAxis(this ProjectionCamera camera) => camera.UpDirection;
        public static Vector3D GetRollAxis(this ProjectionCamera camera) => camera.LookDirection;
        public static Vector3D GetPitchAxis(this ProjectionCamera camera) => Vector3D.CrossProduct(camera.UpDirection, camera.LookDirection);

        public static PerspectiveCamera MoveBy(this PerspectiveCamera camera, Key key) => camera.MoveBy(key, camera.FieldOfView / 180d);
        public static PerspectiveCamera RotateBy(this PerspectiveCamera camera, Key key) => camera.RotateBy(key, camera.FieldOfView / 45d);

        public static TCamera MoveBy<TCamera>(this TCamera camera, Key key, double step) where TCamera : ProjectionCamera => key switch
        {
            Key.W => camera.Move(Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) ? camera.GetYawAxis() : camera.GetRollAxis(), +step),
            Key.S => camera.Move(Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) ? camera.GetYawAxis() : camera.GetRollAxis(), -step),
            Key.A => camera.Move(camera.GetPitchAxis(), +step),
            Key.D => camera.Move(camera.GetPitchAxis(), -step),

            _ => camera
        };

        public static TCamera RotateBy<TCamera>(this TCamera camera, Key key, double angle) where TCamera : ProjectionCamera => key switch
        {
            Key.Left => camera.Rotate(camera.GetYawAxis(), +angle),
            Key.Right => camera.Rotate(camera.GetYawAxis(), -angle),
            Key.Down => camera.Rotate(camera.GetPitchAxis(), +angle),
            Key.Up => camera.Rotate(camera.GetPitchAxis(), -angle),

            _ => camera
        };
    }
}
