
DotNetBuildConfiguration DotNetReadConfiguration(ConfigurationEngine configuration, string appName, string targetName)
{
    if(configuration == null)
    {
        ThrowError("Configuration can not be null");
    }

    if(string.IsNullOrEmpty(appName) || string.IsNullOrEmpty(targetName))
    {
        ThrowError($"Invalid value for appName: {appName} or targetName: {targetName}");
    }

    Information($"Read dotnet configuration for app={appName}, target={targetName}");
    var result = configuration.GetDotNet(appName, targetName);

    if(result == null)
    {
        ThrowError($"DotNet configuration for app={appName}, target={targetName} not found");
    }

    return result;
}

void DotNetPackage(DotNetBuildConfiguration configuration, string intermediateDirectory, string outputDirectory)
{
    //Copy generated dll to artifacts/dotnet/[targets]/[app]
    FilePath csproj = File(configuration.Project);
    FilePath dllFile = GetFiles(CombinePath(csproj.GetDirectory().ToString(), "bin", "**", csproj.GetFilenameWithoutExtension() + ".dll"))
                            .OrderBy(f => new FileInfo(f.FullPath).LastWriteTimeUtc)
                            .FirstOrDefault();
    string dir = CombinePath(outputDirectory, "dotnet", configuration.TargetName, configuration.AppName);
    EnsureDirectoryExists(dir);

    CopyFileToDirectory(dllFile, dir);

    //Copy to output path
    if(!string.IsNullOrEmpty(configuration.OutputPath))
    {
        EnsureDirectoryExists(configuration.OutputPath);
        CopyFileToDirectory(dllFile, configuration.OutputPath);
    }
}

void RunDotNetBuild(ConfigurationEngine configuration, string app, string target)
{
    DotNetBuildConfiguration dotConfiguration = DotNetReadConfiguration(configuration, app, target);

    BuildWithConfiguration(dotConfiguration);
}

void RunDotNetRelease(ConfigurationEngine configuration, string app, string target, string intermediate, string output)
{
    DotNetBuildConfiguration dotConfiguration = DotNetReadConfiguration(configuration, app, target);

    BuildWithConfiguration(dotConfiguration);

    DotNetPackage(dotConfiguration, intermediate, output);
}