/*
const string clientId = "288ec5dd-6736-4d4b-9b96-30e083a8cad2";
const string redirectUri = "http://localhost:29116/authentication-response";
const string scope = "XboxLive.signin offline_access";

var minecraftOAuth2 = new MicrosoftOAuth2(clientId:clientId,redirectUri:redirectUri,new RedirectMessage(),scope);
var userProfile = await minecraftOAuth2.AuthorizeAsync();
Console.WriteLine(userProfile.MinecraftToken);
*/



using ILCore.Launch;
using ILCore.Minecraft.Libraries;
using ILCore.OAuth;
using ILCore.OAuth.MinecraftOAuth;
using ILCore.OAuth.RedirectUri;

const string javaPath = @"C:\Program Files\Java\jdk1.8.0_202\jre\bin\javaw.exe";
//const string javaPath = @"C:\Users\IYO\AppData\Roaming\.minecraft\runtime\java-runtime-delta\bin\java.exe";
const string minecraftPath = @"G:\Minecraft\.minecraft";
//Fail


//Success

const string versionName = "1.12.2";


const string maxMemory = "4096";
const string jvmArgs = " -XX:+UseG1GC -XX:-UseAdaptiveSizePolicy -XX:-OmitStackTraceInFastThrow -Dfml.ignoreInvalidMinecraftCertificates=True -Dfml.ignorePatchDiscrepancies=True -Dlog4j2.formatMsgNoLookups=true";

const string clientId = "288ec5dd-6736-4d4b-9b96-30e083a8cad2";
const string redirectUri = "http://localhost:29116/authentication-response";
const string scope = "XboxLive.signin offline_access";

var minecraftOAuth2 = new MicrosoftOAuth2(clientId:clientId,redirectUri:redirectUri,new RedirectMessage());
var userProfile = await minecraftOAuth2.AuthorizeAsync();

//var userProfile = new LegacyUserProfile { Name = "IYO" };

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
    LauncherVersion = "001"
};
var launchArg = launchArgs.PrepareArguments(info);

Natives natives = new();
var libs = new Libraries().GetLibraries(minecraftPath,versionName);

await natives.Extract(minecraftPath, versionName, libs);

MinecraftProcess minecraftProcess = new();
Console.WriteLine(launchArg);
minecraftProcess.Launch(javaPath,launchArg);



