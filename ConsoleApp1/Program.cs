/*
const string clientId = "288ec5dd-6736-4d4b-9b96-30e083a8cad2";
const string redirectUri = "http://localhost:29116/authentication-response";
const string scope = "XboxLive.signin offline_access";

var minecraftOAuth2 = new MicrosoftOAuth2(clientId:clientId,redirectUri:redirectUri,new RedirectMessage(),scope);
var userProfile = await minecraftOAuth2.AuthorizeAsync();
Console.WriteLine(userProfile.MinecraftToken);
*/


using ILCore.Analyzer.Minecraft;using ILCore.Languages;
using ILCore.Launch;
using ILCore.Minecraft.Libraries;
using ILCore.Minecraft.Options;
using ILCore.OAuth;
using ILCore.OAuth.MinecraftOAuth;
using ILCore.OAuth.RedirectUri;
using ILCore.Util;

const string javaPath = @"C:\Program Files\Java\jdk1.8.0_202\jre\bin\javaw.exe";
//const string javaPath = @"C:\Users\IYO\AppData\Roaming\.minecraft\runtime\java-runtime-delta\bin\javaw.exe";
const string minecraftPath = @"G:\Minecraft\.minecraft";
//Fail


//Success

const string versionName = "1.20.2";


const string maxMemory = "2048";
const string jvmArgs = " -XX:+UseG1GC -XX:-UseAdaptiveSizePolicy -XX:-OmitStackTraceInFastThrow -Dfml.ignoreInvalidMinecraftCertificates=True -Dfml.ignorePatchDiscrepancies=True -Dlog4j2.formatMsgNoLookups=true";

const string clientId = "288ec5dd-6736-4d4b-9b96-30e083a8cad2";
const string redirectUri = "http://localhost:29116/authentication-response";
const string scope = "XboxLive.signin offline_access";

var minecraftOAuth2 = new MicrosoftOAuth2(clientId:clientId,uri:redirectUri,new RedirectMessage
{
   TitleSuccess = Language.GetValue("Success"),
   ContentSuccess = Language.GetValue("Success")
   //...
});
//var userProfile = await minecraftOAuth2.AuthorizeAsync();

var userProfile = new LegacyUserProfile("IYO");


LaunchArgs launchArgs = new();
var info = new LaunchInfo
{
    VersionName = versionName,
    AbsoluteVersion = true,
    MinecraftPath = minecraftPath,
    MaxMemory = maxMemory,
    JvmArgs = jvmArgs,
    UserProfile = userProfile,
    LauncherName = "ILCore",
    LauncherVersion = "001",
    CustomArgs = "ILCore",
    Fullscreen = false,
    WindowWidth = 1600,
    WindowHeight = 960
};

var launchArg = await launchArgs.PrepareArguments(info);

await new Natives().Extract(minecraftPath, versionName);
await new Options().SetOptions(minecraftPath, versionName, new Dictionary<string, string>()
{
    {"lang","zh_cn"}
});

var minecraftProcess = new MinecraftProcessBuilder().BuildProcess(javaPath,launchArg);

minecraftProcess.MinecraftLogOutPut += (minecraftLog) =>
{
    Console.WriteLine(minecraftLog.ToString());
};

minecraftProcess.MinecraftLogCrash += (errors) => 
{
    foreach (var log in errors)
    {
        Console.WriteLine(log);
    }
};

minecraftProcess.MinecraftLaunchSuccess += () =>
{
    Console.WriteLine("启动成功");
};

minecraftProcess.MinecraftLaunchFailed += () =>
{
    Console.WriteLine("启动失败");
};

await minecraftProcess.Start();

