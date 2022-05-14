using UnityEngine;
using UINT32 = System.UInt32;

namespace Common.Utils
{
    public class Lang
    {
        public static readonly string No_Breaking_Space = "\u00A0";
        public static string[] ChineseChar =
        {"一","二","三","四","五","六","七","八","九","十", "十一", "十二", "十三", "十四", "十五", "十六", "十七", "十八", "十九", "二十",
    "二十一", "二十二", "二十三", "二十四", "二十五", "二十六", "二十七", "二十八", "二十九", "三十", "三十一", "三十二", "三十三", "三十四", "三十五", "三十六", "三十七", "三十八", "三十九", "四十",
    "四十一", "四十二", "四十三", "四十四", "四十五", "四十六", "四十七", "四十八", "四十九", "五十"};
        static JSONObject _tempJsonObj = null;

        public static void SetTempJsonObj(JSONObject obj)
        {
            _tempJsonObj = obj;
        }

        public static string GetByString(string sOriginalStr, object param1)
        {
            string tblStr = string.Format(sOriginalStr, param1.ToString());
            tblStr = CheckString(tblStr);
            return tblStr;
        }

        public static string GetByString(string sOriginalStr, object param1, object param2)
        {
            string tblStr = string.Format(sOriginalStr, param1.ToString(), param2.ToString());
            tblStr = CheckString(tblStr);
            return tblStr;
        }

        public static string GetByString(string sOriginalStr, params object[] args)
        {
            string tblStr = string.Format(sOriginalStr, args);
            tblStr = CheckString(tblStr);
            return tblStr;
        }

        public static string Get(string Str)
        {
            if (_tempJsonObj == null)
            {
                return "";
            }
            JSONObject jsonObj = _tempJsonObj.GetField(Str);
            if (jsonObj == null)
            {
                Debug.LogError(Str + " is not exist");
                return null;
            }
            string jsonStr = "";//JsonHelper.getString(jsonObj);
            jsonStr = CheckString(jsonStr);
            return jsonStr;
        }

        public static string Get(string Str, params object[] args)
        {
            if (_tempJsonObj == null)
            {
                return "";
            }
            JSONObject jsonObj = _tempJsonObj.GetField(Str);
            if (jsonObj == null)
            {
                Debug.LogError(Str + " is not exist");
                return null;
            }
            //string jsonStr = JsonHelper.getString(jsonObj);
            string jsonStr = "";
            string tblStr = string.Format(jsonStr, args);
            tblStr = CheckString(tblStr);
            return tblStr;
        }


        public static string Get(string Str, object param1)
        {
            if (_tempJsonObj == null)
            {
                return "";
            }
            JSONObject jsonObj = _tempJsonObj.GetField(Str);
            if (jsonObj == null)
            {
                Debug.LogError(Str + " is not exist");
                return null;
            }
            //string jsonStr = JsonHelper.getString(jsonObj);
            string tblStr = string.Format(/*jsonStr,*/param1.ToString());
            tblStr = CheckString(tblStr);
            return tblStr;
        }

        public static string Get(string Str, object param1, object param2)
        {
            if (_tempJsonObj == null)
            {
                return "";
            }
            JSONObject jsonObj = _tempJsonObj.GetField(Str);
            if (jsonObj == null)
            {
                Debug.LogError(Str + " is not exist");
                return null;
            }
            //string jsonStr = JsonHelper.getString(jsonObj);
            string tblStr = string.Format(/*jsonStr,*/ param1.ToString(), param2.ToString());
            tblStr = CheckString(tblStr);
            return tblStr;
        }

        public static string Get(string Str, object param1, object param2, object param3)
        {
            if (_tempJsonObj == null)
            {
                return "";
            }
            JSONObject jsonObj = _tempJsonObj.GetField(Str);
            if (jsonObj == null)
            {
                Debug.LogError(Str + " is not exist");
                return null;
            }
            //string jsonStr = JsonHelper.getString(jsonObj);
            string tblStr = string.Format(/*jsonStr,*/ param1.ToString(), param2.ToString(), param3.ToString());
            tblStr = CheckString(tblStr);
            return tblStr;
        }

        // 中文半角空格在多行文本中会自动换行，需要将其替换成不换行空格
        public static string CheckString(string str)
        {
            if (str == null) return "";
            if (str.Contains(" "))
            {
                str = str.Replace(" ", No_Breaking_Space);
            }
            if (str.Contains("\\n"))
            {
                str = str.Replace("\\n", "\n");
            }
            return str;
        }

        // 去除字符串中所有的 <***> 内容 （返回不包含richtext的内容） 例如 "<color=#123456>hello</color>" ===> "hello" 
        public static string DeleteAngleBrackets(string str)
        {
            char[] cArr = str.ToCharArray();
            string ans = "";
            int i = 0;
            char c;
            for (i = 0; i < cArr.Length; ++i)
            {
                c = cArr[i];
                if (c == '<')
                {
                    while (c != '>')
                    {
                        i++;
                        c = cArr[i];
                    }
                }
                else
                {
                    ans += c;
                }
            }

            return ans;
        }

        // 返回字符串中参数的个数。例如 "<color=#{0}>{1}级</color>" 返回2
        public static int GetParamCountInString(string str)
        {
            char[] cArr = str.ToCharArray();
            int count = 0;
            for (int i = 0; i < cArr.Length; ++i)
            {
                if (cArr[i] == '{') count++;
            }
            return count;
        }
    }
}