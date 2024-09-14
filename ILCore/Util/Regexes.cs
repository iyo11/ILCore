using System.Text.RegularExpressions;

namespace ILCore.Util;

public partial class Regexes
{
    [GeneratedRegex(@"(\[\d{2}:\d{2}:\d{2}\]) (\[.*\]): (.*)", RegexOptions.Compiled)]
    public static partial Regex LogRegex();

    [GeneratedRegex(@"\(([^()]*)\)", RegexOptions.Compiled)]
    public static partial Regex RoundBrackets();

    [GeneratedRegex(@"\[([^[\]]*)\]", RegexOptions.Compiled)]
    public static partial Regex SquareBrackets();
}