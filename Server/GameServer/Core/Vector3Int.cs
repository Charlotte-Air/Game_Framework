using System;
using UnityEngine;
using Common.Utils;
using Charlotte.Proto;

namespace GameServer.Core
{
    /// <summary>
    /// 客户端进行实体同步工具类
    /// </summary>
    public struct Vector3Int : IEquatable<Vector3Int>
    {
        private int m_X;

        private int m_Y;

        private int m_Z;

        private static readonly Vector3Int s_Zero = new Vector3Int(0, 0, 0);

        private static readonly Vector3Int s_One = new Vector3Int(1, 1, 1);

        private static readonly Vector3Int s_Up = new Vector3Int(0, 1, 0);

        private static readonly Vector3Int s_Down = new Vector3Int(0, -1, 0);

        private static readonly Vector3Int s_Left = new Vector3Int(-1, 0, 0);

        private static readonly Vector3Int s_Right = new Vector3Int(1, 0, 0);

        /// <summary>
        /// <para>Component 向量X</para>
        /// </summary>
        public int x { get => m_X; set => m_X = value; }

        /// <summary>
        /// <para>Component 向量Y</para>
        /// </summary>
        public int y { get => m_Y; set => m_Y = value; }

        /// <summary>
        /// <para>Component 向量Z</para>
        /// </summary>
        public int z { get => m_Z; set => m_Z = value; }


        /// <summary>
        /// <para>返回该向量的长度(只读)</para>
        /// </summary>
        public float magnitude => (float)Math.Sqrt(x * x + y * y + z * z);

        /// <summary>
        /// <para>返回该向量长度的平方(只读)</para>
        /// </summary>
        public int sqrMagnitude => x * x + y * y + z * z;

        /// <summary>
        /// <para>Vector3Int (0, 0, 0)</para>
        /// </summary>
        public static Vector3Int zero => s_Zero;

        /// <summary>
        /// <para>Vector3Int (1, 1, 1)</para>
        /// </summary>
        public static Vector3Int one => s_One;

        /// <summary>
        /// <para>Vector3Int (0, 1, 0)</para>
        /// </summary>
        public static Vector3Int up => s_Up;

        /// <summary>
        /// <para>Vector3Int (0, -1, 0)</para>
        /// </summary>
        public static Vector3Int down => s_Down;

        /// <summary>
        /// <para>Vector3Int (-1, 0, 0)</para>
        /// </summary>
        public static Vector3Int left => s_Left;

        /// <summary>
        /// <para>Vector3Int (1, 0, 0)</para>
        /// </summary>
        public static Vector3Int right => s_Right;

        /// <summary>
        /// <para>返回A和B之间的距离</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static float Distance(Vector3Int a, Vector3Int b) => (a - b).magnitude;

        /// <summary>
        /// <para>返回一个由两个向量的最小分量组成的向量</para>
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public static Vector3Int Min(Vector3Int lhs, Vector3Int rhs) => new Vector3Int(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y), Math.Min(lhs.z, rhs.z));

        /// <summary>
        /// <para>返回一个由两个向量的最大Component组成的向量</para>
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        public static Vector3Int Max(Vector3Int lhs, Vector3Int rhs) => new Vector3Int(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y), Math.Max(lhs.z, rhs.z));

        /// <summary>
        /// <para>将两个向量Component相乘</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public static Vector3Int Scale(Vector3Int a, Vector3Int b) => new Vector3Int(a.x* b.x, a.y* b.y, a.z* b.z);

        public static implicit operator NVector3(Vector3Int v) => new NVector3() { X = v.x, Y = v.y, Z = v.z };

        public static implicit operator Vector3Int(NVector3 v) => new Vector3Int(v.X, v.Y, v.Z);


        /// <summary>
        /// <para>将一个Vector3转换为一个Vector3Int</para>
        /// </summary>
        /// <param name="v"></param>
        public static Vector3Int FloorToInt(Vector3 v) => new Vector3Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y), Mathf.FloorToInt(v.z));

        /// <summary>
        /// <para>将一个Vector3转换为一个Vector3Int</para>
        /// </summary>
        /// <param name="v"></param>
        public static Vector3Int CeilToInt(Vector3 v) => new Vector3Int(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y), Mathf.CeilToInt(v.z));

        /// <summary>
        /// <para>将一个Vector3转换为一个Vector3Int</para>
        /// </summary>
        /// <param name="v"></param>
        public static Vector3Int RoundToInt(Vector3 v) => new Vector3Int(MathUtil.RoundToInt(v.x), MathUtil.RoundToInt(v.y), MathUtil.RoundToInt(v.z));

        public static Vector3Int operator +(Vector3Int a, Vector3Int b) => new Vector3Int(a.x + b.x, a.y + b.y, a.z + b.z);

        public static Vector3Int operator -(Vector3Int a, Vector3Int b) => new Vector3Int(a.x - b.x, a.y - b.y, a.z - b.z);

        public static Vector3Int operator *(Vector3Int a, Vector3Int b) => new Vector3Int(a.x * b.x, a.y * b.y, a.z * b.z);

        public static Vector3Int operator *(Vector3Int a, int b) => new Vector3Int(a.x * b, a.y * b, a.z * b);

        public static bool operator ==(Vector3Int lhs, Vector3Int rhs) => lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;

        public static bool operator !=(Vector3Int lhs, Vector3Int rhs) => !(lhs == rhs);

        /// <summary>
        /// 比较实体对象是否相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns>相等则返回True 不相同返回False</returns>
        public override bool Equals(object other) => other is Vector3Int == true ? Equals((Vector3Int)other) : false;

        /// <summary>
        /// 比较Vector3Int是否相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Vector3Int other) => this == other;

        /// <summary>
        /// <para>获取Vector3Int哈希码</para>
        /// </summary>
        /// <returns>
        /// <para>The hash code of the Vector3Int.</para>
        /// </returns>
        public override int GetHashCode() => x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);

        /// <summary>
        /// <para>返回该向量数据为字符串类型</para>
        /// </summary>
        /// <param name="format"></param>
        public override string ToString() => string.Format("({0}, {1}, {2})", x, y, z);

        /// <summary>
        /// <para>返回该向量数据为字符串类型</para>
        /// </summary>
        /// <param name="format"></param>
        public string ToString(string format) => string.Format("({0}, {1}, {2})", x.ToString(format), y.ToString(format), z.ToString(format));


        public int this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    default: throw new IndexOutOfRangeException(string.Format("Invalid Vector3Int index addressed: {0}!", index));
                }
            }
            set
            {
                switch (index)
                {
                    case 0: x = value; break;
                    case 1: y = value; break;
                    case 2: z = value; break;
                    default: throw new IndexOutOfRangeException(string.Format("Invalid Vector3Int index addressed: {0}!", index));
                }
            }
        }

        #region 接口方法
        public Vector3Int(int x, int y, int z)
        {
            m_X = x;
            m_Y = y;
            m_Z = z;
        }

        /// <summary>
        /// <para>设置现有组件Vector3Int X、Y、Z</para>
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Set(int x, int y, int z)
        {
            m_X = x;
            m_Y = y;
            m_Z = z;
        }

        /// <summary>
        /// <para>将这个向量的每个分量乘以相同的比例分量</para>
        /// </summary>
        /// <param name="scale"></param>
        public void Scale(Vector3Int scale)
        {
            x *= scale.x;
            y *= scale.y;
            z *= scale.z;
        }

        /// <summary>
        /// <para>将Vector3Int固定到min和max给出的边界</para>
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void Clamp(Vector3Int min, Vector3Int max)
        {
            x = Math.Max(min.x, x);
            x = Math.Min(max.x, x);
            y = Math.Max(min.y, y);
            y = Math.Min(max.y, y);
            z = Math.Max(min.z, z);
            z = Math.Min(max.z, z);
        }

        #endregion
    }

}
