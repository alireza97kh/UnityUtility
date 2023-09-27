using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RtlTextFixer 
{
    public static string RtlFix(this string str)
    {
        if (Application.isPlaying && !IsRtl(str))
            return str;
        if (string.IsNullOrEmpty(str))
            return "";
        //ﺉﻚﻙﯤ
        str = str.Replace('ی', 'ﻱ');
        //str = str.Replace( 'ی','ﺉ');
        str = str.Replace('ک', 'ﻙ');
        //str = str.Replace('ﻚ', 'ک');

        if (str.StartsWith(":"))
        {
            str = str.Replace(":", "");
            str = $"{str} :";
        }
        if (str.StartsWith("!"))
        {
            str = str.Replace("!", "");
            str = $"{str} !";
        }
        if (str.StartsWith("?"))
        {
            str = str.Replace("?", "");
            str = $"{str} ?";
        }
        if (str.StartsWith("%"))
        {
            str = str.Replace("%", "");
            str = $"{str} %";
        }

        if (str.StartsWith(" ...") || str.StartsWith("..."))
        {
            str = str.Replace(".", "");
            str = $"{str} ...";
        }

        str = ArabicSupport.ArabicFixer.Fix(str, true, false);
        str = str.Replace('ﺃ', 'آ');
        return str;
    }
    public static bool IsRtl(this string str)
    {
        var isRtl = false;
        foreach (var _char in str)
        {
            if ((_char >= 1536 && _char <= 1791) || (_char >= 65136 && _char <= 65279))
            {
                isRtl = true;
                break;
            }
        }
        return isRtl;
    }
}
