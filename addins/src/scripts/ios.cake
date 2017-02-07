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

string iOSGetCertificate(iOSBuildConfiguration configuration)
{
    if(!configuration.Fastlane.IsEnabled)
    {
        return null;
    }

    CertificateType type;
    if(configuration.Fastlane.AdHoc)
    {
        type = CertificateType.AdHoc;
    }
    else if(configuration.Fastlane.AppStore)
    {
        type = CertificateType.AppStore;
    }
    else if(configuration.Fastlane.Development)
    {
        type = CertificateType.Development;
    }
    else
    {
        throw new CakeException($"Fastlane certificate type is not specified for app {configuration.Bundle}");
    }

    return FastlaneGetCertificate(
        configuration.Bundle,
        configuration.Fastlane.UserName,
        configuration.Fastlane.TeamName,
        type
    );
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
    string codeSignProvision = iOSGetCertificate(iOSConfiguration) ?? iOSConfiguration.CodesignProvision;
    Information($"Sign ios app with certificate uuid: {codeSignProvision}");
    //BuildWithConfiguration(iOSConfiguration);

    string outputPath = CombinePath(output, iOSConfiguration.PlatformName, iOSConfiguration.TargetName, iOSConfiguration.AppName);
    EnsureDirectoryExists(outputPath);
    CreateIpaFileWithSignature(iOSConfiguration.Solution, outputPath, iOSConfiguration.CodesignKey, codeSignProvision, c => ApplyConfiguration(c, iOSConfiguration.BuildProperties));
    CopyDSymToOutputDirectory(iOSConfiguration.Solution, outputPath);

}
