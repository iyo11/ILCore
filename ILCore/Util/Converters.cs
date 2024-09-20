namespace ILCore.Util;

public static class Converters
{
    public static string ConvertBitsToBestUnit(long bits)
    {
        return bits switch
        {
            < 1024 => $"{bits} bit{(bits != 1 ? "s" : "")}",
            < 1024 * 1024 => $"{(double)bits / 1024:0.00}kb",
            < 1024 * 1024 * 1024 => $"{(double)bits / (1024 * 1024):0.00}Mb",
            _ => $"{(double)bits / (1024 * 1024 * 1024):0.00}Gb"
        };
    }
}