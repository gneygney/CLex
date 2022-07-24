using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLex;

public static class StringExtention
{
    public static string SubStartEnd(this string str, int start, int end)
    {
        var length = end - start;
        return str.Substring(start, length);
    }
}
