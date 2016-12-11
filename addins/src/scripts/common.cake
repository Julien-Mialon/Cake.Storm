void ThrowError(string error)
{
    Error(error);
    throw new CakeException(error);
}

void ThrowError(string error, Exception ex)
{
    Error($"{error} exception: {ex.Message}");
    throw new CakeException(error, ex);
}

void ApplyConfiguration(DotNetBuildSettings settings, Dictionary<string, string> buildProperties)
{
    foreach(KeyValuePair<string, string> property in buildProperties)
    {
        if("configuration" == property.Key.ToLower())
        {
            settings.SetConfiguration(property.Value);
        }
        else
        {
            settings.WithProperty(property.Key, property.Value);
        }
    }
}

void CleanBinObj(string rootDirectory)
{
    DeleteDirectories(GetDirectories(CombinePath(rootDirectory, "**/bin")), true);
    DeleteDirectories(GetDirectories(CombinePath(rootDirectory, "**/obj")), true);
}

void DeleteAndCreateDirectory(string directory)
{
    if(DirectoryExists(directory))
    {
        DeleteDirectory(directory, true);
    }
    CreateDirectory(directory);
}

void BuildWithConfiguration(Configuration configuration)
{
    DotNetBuild(configuration.Solution, c => ApplyConfiguration(c, configuration.BuildProperties));
}

string CombinePath(params string[] path)
{
    return System.IO.Path.Combine(path);
}