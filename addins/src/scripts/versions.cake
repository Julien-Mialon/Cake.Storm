
void UpdateVersions(string jsonFile)
{
    string buildId = Argument<string>("BuildId", null);

    string major = Argument("version_major", "1");
    string minor = Argument("version_minor", "0");
    string build = Argument("version_build", "0");

    if(buildId != null)
    {
        major = major.Replace("{build}", buildId);
        minor = minor.Replace("{build}", buildId);
        build = build.Replace("{build}", buildId);
    }

    string iOSShortVersion = $"{major}.{minor}";
    string iOSBuildVersion = $"{major}.{minor}.{build}";
    
    string fileContent = System.IO.File.ReadAllText(jsonFile);

    fileContent = fileContent.Replace("{iOSShortVersion}", iOSShortVersion);
    Information($"Using iOS Short Version '{iOSShortVersion}'");

    fileContent = fileContent.Replace("{iOSBuildVersion}", iOSBuildVersion);
    Information($"Using iOS Build Version '{iOSBuildVersion}'");

    System.IO.File.WriteAllText(jsonFile, fileContent);
}