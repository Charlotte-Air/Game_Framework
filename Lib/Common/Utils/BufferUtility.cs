namespace Common
{
    public class BufferUtility
    {
        public static int GetInt32(byte[] buffer,int offset) => System.BitConverter.ToInt32(buffer, offset);
    }
}
