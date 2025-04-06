
using Silk.NET.Maths;

namespace Szeminarium
{
    internal class CameraDescriptor
    {
        public Vector3D<float> Position { get; private set; } = new(0, 0, 3);
        private Vector3D<float> front = new(0, 0, -1);
        private Vector3D<float> up = new(0,1,0);

        private float yaw = -90.0f;
        private float pitch = 0.0f;
        private float speed = 0.15f;
        private float sensitivity = 2f; // Mouse sensitivity

        public CameraDescriptor() { }
        public Matrix4X4<float> GetViewMatrix()
        {
            return Matrix4X4.CreateLookAt(Position, Position + front, up);
        }
        public void MoveForward() => Position += speed * front;
        public void MoveBackward() => Position -= speed * front;
        public void MoveLeft() => Position -= Vector3D.Normalize(Vector3D.Cross(front, up)) * speed;
        public void MoveRight() => Position += Vector3D.Normalize(Vector3D.Cross(front, up)) * speed;

        private float ToRadians(float degrees) => (float)(degrees * Math.PI / 180.0);

        public void Rotate(float deltaX, float deltaY)
        {
            yaw += deltaX * sensitivity;
            pitch -= deltaY * sensitivity;

            if (pitch > 89.0f) pitch = 89.0f;
            if (pitch < -89.0f) pitch = -89.0f;

            Vector3D<float> direction;
            direction.X = (float)Math.Cos(ToRadians(yaw)) * (float)Math.Cos(ToRadians(pitch));
            direction.Y = (float)Math.Sin(ToRadians(pitch));
            direction.Z = (float)Math.Sin(ToRadians(yaw)) * (float)Math.Cos(ToRadians(pitch));
            front = Vector3D.Normalize(direction);
        }
    }
}
