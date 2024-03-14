using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace ZE.Purastic {
    [System.Serializable]
    public struct VirtualPoint
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Forward => Rotation * Vector3.forward;
        public Vector3 Up => Rotation * Vector3.up;
        public override string ToString() => $"[p:{Position}, r:{Rotation.eulerAngles}]";

        public VirtualPoint(Transform point)
        {
            Position = point.position;
            Rotation = point.rotation;
        }
        public VirtualPoint(Vector3 pos, Quaternion rot)
        {
            Position = pos;
            Rotation = rot;
        }
        public VirtualPoint Move(float step)
        {
            return new VirtualPoint()
            {
                Position = Position + step * Forward,
                Rotation = Rotation
            };
        }
        public VirtualPoint Move(Vector3 sumVector) => new VirtualPoint(Position + sumVector, Rotation);
        public VirtualPoint Steer(Quaternion rotation)
        {
            return new VirtualPoint()
            {
                Position = Position,
                Rotation = rotation
            };
        }
        public Matrix4x4 ToTransformMatrix() => Matrix4x4.TRS(Position, Rotation, Vector3.one);
        

        public string Encode()
        {
            var data = new float[7];
            data[0] = Position.x;
            data[1] = Position.y;
            data[2] = Position.z;
            data[3] = Rotation.x;
            data[4] = Rotation.y;
            data[5] = Rotation.z;
            data[6] = Rotation.w;
            return JsonConvert.SerializeObject(data);
        }
        public static VirtualPoint Decode(string data)
        {
            var numbers = JsonConvert.DeserializeObject<float[]>(data);
            return new VirtualPoint()
            {
                Position = new Vector3(numbers[0], numbers[1], numbers[2]),
                Rotation = new Quaternion(numbers[3], numbers[4], numbers[5], numbers[6])
            };
        }
    }
}
