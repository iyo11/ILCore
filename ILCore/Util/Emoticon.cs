namespace ILCore.Util;

public static class Emoticon
{
    private static readonly string[] Emoticons = 
    [
        "(^^ゞ",
        "(^_^;)",
        "(-_-;)",
        "(~_~;)",
        "(・。・;)",
        "(・_・;)",
        "(・・;)",
        "^^;",
        "^_^;",
        "(#^.^#)",
        "(^ ^;)"
    ];

    public static string GetRandomEmoticon()
    {
        return Emoticons[new Random().Next(0, Emoticons.Length)];
    }
    
}