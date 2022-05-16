class EnumUtil
{
    public static string GetEnumDescription(System.Enum enumValue)
    {
        string str = enumValue.ToString();
        System.Reflection.FieldInfo field = enumValue.GetType().GetField(str);
        object[] objs = field.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
        if (objs == null || objs.Length == 0) return str;
        System.ComponentModel.DescriptionAttribute da = (System.ComponentModel.DescriptionAttribute)objs[0];
        return da.Description;
    }
}