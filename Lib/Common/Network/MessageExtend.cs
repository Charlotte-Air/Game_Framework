namespace Charlotte.Extend
{
    public static class MessageExtend
    {
        public static string String(this Charlotte.Proto.NVector3 self)
        {
            return string.Format("({0},{1},{2})", self.X, self.Y, self.Z);
        }

        public static string String(this Charlotte.Proto.NEntity self)
        {
            return string.Format("({0}:pos:{1},dir:{2},spd:{3}", self.Id, self.Position.String(), self.Direction.String(), self.Speed);
        }


        public static bool Equal(this Charlotte.Proto.NVector3 self, Charlotte.Proto.NVector3 rhs)
        {
            if (rhs == null) return false;
            return self.X == rhs.X && self.Y == rhs.Y && self.Z == rhs.Z;
        }

        public static bool Equal(this Charlotte.Proto.NEntity self, Charlotte.Proto.NEntity rhs)
        {
            if (self.Id != rhs.Id) return false;
            if (self.Position == null)
            {
                if (rhs.Position != null) return false;
            }
            else
                if (!self.Position.Equal(rhs.Position)) return false;

            if (self.Direction == null)
            {
                if (rhs.Direction != null) return false;
            }
            else
                 if (!self.Direction.Equal(rhs.Direction)) return false;

            return true;
        }

        public static UnityEngine.Vector3Int FromNVector3(this UnityEngine.Vector3Int self, Charlotte.Proto.NVector3 nVector)
        {
            return new UnityEngine.Vector3Int(nVector.X, nVector.Y, nVector.Z);
        }

        public static Charlotte.Proto.NVector3 FromVector3Int(this Charlotte.Proto.NVector3 self, UnityEngine.Vector3Int nVector)
        {
            self.X = nVector.x;
            self.Y = nVector.y;
            self.Z = nVector.z;
            return self;
        }
    }
}
