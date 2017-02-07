#l "./common.cake"

iOSBuildConfiguration iOSReadConfiguration(ConfigurationEngine configuration, string appName, string targetName)
{
    if(configuration == null)
    {
        ThrowError("Configuration can not be null");
    }

    if(string.IsNullOrEmpty(appName) || string.IsNullOrEmpty(targetName))
    {
        ThrowError($"Invalid value for appName: {appName} or targetName: {targetName}");
    }

    Information($"Read ios configuration for app={appName}, target={targetName}");
    var result = configuration.GetiOS(appName, targetName);

    if(result == null)
    {
        ThrowError($"iOS configuration for app={appName}, target={targetName} not found");
    }

    return result;
}

void iOSTransformPList(iOSBuildConfiguration configuration)
{
    var plist = LoadPListFile(configuration.PListFile);
    plist.BundleId = configuration.Bundle;
    plist.Version = configuration.Version;
    plist.BuildVersion = configuration.BuildVersion;
    plist.Save();
}

void RuniOSBuild(ConfigurationEngine configuration, string app, string target)
{
    iOSBuildConfiguration iOSConfiguration = iOSReadConfiguration(configuration, app, target);
    iOSTransformPList(iOSConfiguration);

    BuildWithConfiguration(iOSConfiguration);
}

void RuniOSRelease(ConfigurationEngine configuration, string app, string target, string intermediate, string output)
{
    iOSBuildConfiguration iOSConfiguration = iOSReadConfiguration(configuration, app, target);
    iOSTransformPList(iOSConfiguration);

    //BuildWithConfiguration(iOSConfiguration);

    string outputPath = CombinePath(output, iOSConfiguration.PlatformName, iOSConfiguration.TargetName, iOSConfiguration.AppName);
    EnsureDirectoryExists(outputPath);
    CreateIpaFileWithSignature(iOSConfiguration.Solution, outputPath, iOSConfiguration.CodesignKey, iOSConfiguration.CodesignProvision, c => ApplyConfiguration(c, iOSConfiguration.BuildProperties));
}
