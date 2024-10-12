using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathLogger;
public static class Utilities
{
    public static string ToReadable(this DateOnly date)
    {
        return $"{date.Year}.{date.Month}.{date.Day}";
    }

    public static DateOnly ParseHuman(string input)
    {
        var parts = input.Trim().Split([',', '.', '/', ' '], options: StringSplitOptions.RemoveEmptyEntries);
        return new DateOnly(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
    }
}
