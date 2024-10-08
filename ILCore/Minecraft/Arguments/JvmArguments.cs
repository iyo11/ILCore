﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ILCore.Minecraft.Arguments;

public class JvmArguments
{
    public string ToLaunchArguments(JObject versionObj)
    {
        var jArgumentsToken = versionObj["arguments"];
        string[] argumentsStrings = [];

        if (jArgumentsToken is null) return string.Join(" ", argumentsStrings);
        var arguments = JsonConvert.DeserializeObject<JsonArguments>(jArgumentsToken.ToString());
        argumentsStrings = arguments.Jvm.OfType<string>().ToArray();


        return string.Join(" ", Array.ConvertAll(argumentsStrings, argument => $"\"{argument}\""));
    }
}