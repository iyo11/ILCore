using ILCore.Launch;

namespace UnitTest.Launch
{
    public class LaunchTest
    {


        [Test]
        public void Test1()
        {
            const string javaPath = @"C:\Program Files\Java\jdk1.8.0_202\jre\bin\javaw.exe";
            //const string javaPath = @"C:\Program Files\Java\jdk-17\bin\java.exe";
            const string minecraftPath = @"G:\Minecraft\.minecraft";
            //Fail



            //Success
            //const string versionName = "1.16.5";
            //const string versionName = "1.12.2-Forge_14.23.5.2859";
            //const string versionName = "Levitated";
            const string versionName = "1.8.9-Forge_11.15.1.2318";
            //const string versionName = "1.7.10";
            //const string versionName = "1.7.10-1";
            //const string versionName = "1.6.4";
            //const string versionName = "1.6.3-Forge_9.11.0.878";
            //const string versionName = "1.7.10-Forge_10.13.4.1614";
            //const string versionName = "Compact";
            //const string versionName = "1.19.2-Forge_43.4.2";
            //const string versionName = "1.7.10-Forge_10.13.4.1614-OptiFine_E7";
            //const string versionName = "wutuobang3.0";

            const string maxMemory = "4096";
            const string jvmArgs = " -XX:+UseG1GC -XX:-UseAdaptiveSizePolicy -XX:-OmitStackTraceInFastThrow -Dfml.ignoreInvalidMinecraftCertificates=True -Dfml.ignorePatchDiscrepancies=True -Dlog4j2.formatMsgNoLookups=true";

            LaunchArgs launchArgs = new LaunchArgs();
            var args = launchArgs.PrepareArguments(versionName, minecraftPath, maxMemory, jvmArgs, "IYOII");
            StartMinecraft startMinecraft = new StartMinecraft();
            startMinecraft.Launch(javaPath,args);
        }
    }
}
