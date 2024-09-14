namespace ILCore.Minecraft.Versions;

public enum LatestType
{
    Release,
    Snapshot
}

public enum VersionType
{
    old_alpha,
    release,
    snapshot
}

public class Latest
{
    public string Release { get; set; }
    public string Snapshot { get; set; }
}

public class VersionsItem
{
    public string Id { get; set; }
    public string Type { get; set; }
    public string Url { get; set; }
    public string Time { get; set; }
    public string ReleaseTime { get; set; }
}

public class JsonVersion
{
    public Latest Latest { get; set; }
    public List<VersionsItem> Versions { get; set; }
}