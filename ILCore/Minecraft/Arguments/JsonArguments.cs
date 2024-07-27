namespace ILCore.Minecraft.Arguments;

public class Os
{
    public string Name { get; set; }
}

public class RulesItem
{
    public string Action { get; set; }

    public Os Os { get; set; }
}

public class JsonArguments
{
    public List<object> Game { get; set; }

    public List<object> Jvm { get; set; }
}