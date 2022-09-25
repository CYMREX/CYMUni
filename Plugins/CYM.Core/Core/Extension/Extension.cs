using UnityEngine;

namespace CYM
{
    public static class Extension
    {
        public static bool IsInv(this long i) => i == Const.LONG_Inv;
        public static bool IsInv(this int i) => i == Const.INT_Inv;
        public static bool IsInv(this float f) => f == Const.FLOAT_Inv;
        public static bool IsValid(this long i) => i != Const.LONG_Inv;
        public static bool IsValid(this int i) => i != Const.INT_Inv;
        public static bool IsValid(this float f) => f != Const.FLOAT_Inv;

        public static bool IsNone(this string str)
        {
            if (str == null) return true;
            return str == Const.STR_None;
        }
        public static bool IsUnknow(this string str)
        {
            if (str == null) return true;
            return str == Const.STR_Unkown;
        }
        public static bool IsInv(this string str)
        {
            if (str == null) return true;
            return str == Const.STR_Inv || str == Const.STR_None || str == Const.STR_Unkown || str == string.Empty || str == "";
        }
        public static bool IsValid(this string str) => !IsInv(str);
        public static bool IsInv(this Vector3 pos)
        {
            if (pos == Const.VEC_GlobalPos ||
                pos == Const.VEC_FarawayPos)
                return true;
            return false;
        }
    }
}
