namespace ILCore.Minecraft.Libraries;

public class JsonLibrary
{
    public JsonLibraryDownloads Downloads;
    public string Name;
    public Dictionary<string, string> Natives;
    public JsonExtract Extract { get; set; }
    public JsonRule[] Rules { get; set; }
    public string Url { get; set; }
}

public class JsonLibraryDownloads
{
    public JsonDownloadFile Artifact { get; set; }

    public Dictionary<string, JsonDownloadFile> Classifiers { get; set; }
}

public class JsonDownloadFile
{
    public string Id { get; set; }

    public string Sha1 { get; set; }

    public int Size { get; set; }

    public string Path { get; set; }

    public string Url { get; set; }

    public int TotalSize { get; set; }
}

public class JsonExtract
{
    public string[] Exclude { get; set; }
}

public class JsonRule
{
    public string Action { get; set; }

    public JsonOs Os { get; set; }
}

public class JsonOs
{
    public string Name { get; set; }
}