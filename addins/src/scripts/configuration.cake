#l "./common.cake"
#l "./android.cake"
#l "./ios.cake"
#l "./dotnet.cake"


void OutputHelp(ConfigurationEngine configuration)
{
    Information(" ============================= Help ============================== ");
    Information("");
    Information("Supported targets: ");
    Information("\tbuild");
    Information("\trebuild");
    Information("\trelease");
    Information("\tclean");

    Dictionary<string, List<string>> targetBuildTasks = new Dictionary<string, List<string>>();
    Dictionary<string, List<string>> targetReleaseTasks = new Dictionary<string, List<string>>();
    Dictionary<string, Dictionary<string, List<string>>> platformAndTargetsBuildTasks = new Dictionary<string, Dictionary<string, List<string>>>();
    Dictionary<string, Dictionary<string, List<string>>> platformAndTargetsReleaseTasks = new Dictionary<string, Dictionary<string, List<string>>>();

    Information("");
    foreach (string platform in configuration.GetPlatforms())
    {
        platformAndTargetsBuildTasks.Add(platform, new Dictionary<string, List<string>>());
        platformAndTargetsReleaseTasks.Add(platform, new Dictionary<string, List<string>>());

        foreach (string app in configuration.GetApps())
        {
            Information($"\tApp: {app}");
            foreach (string target in configuration.GetTargets(app))
            {
                string task = GetBuildTaskName(platform, app, target);
                Information($"\t\t{task}");
                
                if (!targetBuildTasks.ContainsKey(target))
                {
                    targetBuildTasks.Add(target, new List<string>());
                }
                if (!platformAndTargetsBuildTasks[platform].ContainsKey(target))
                {
                    platformAndTargetsBuildTasks[platform].Add(target, new List<string>());
                }
                targetBuildTasks[target].Add(task);
                platformAndTargetsBuildTasks[platform][target].Add(task);


                task = GetReleaseTaskName(platform, app, target);
                Information($"\t\t{task}");
                if (!targetReleaseTasks.ContainsKey(target))
                {
                    targetReleaseTasks.Add(target, new List<string>());
                }
                if (!platformAndTargetsReleaseTasks[platform].ContainsKey(target))
                {
                    platformAndTargetsReleaseTasks[platform].Add(target, new List<string>());
                }
                targetReleaseTasks[target].Add(task);
                platformAndTargetsReleaseTasks[platform][target].Add(task);
            }
        }
    }

    Information($"");
    foreach (string target in targetBuildTasks.Keys)
    {
        Information($"\tTarget: {target}");
        Information($"\t\t{GetBuildTaskNameForTarget(target)}");
        Information($"\t\t{GetReleaseTaskNameForTarget(target)}");
    }

    Information("");
    foreach (string platform in platformAndTargetsBuildTasks.Keys)
    {
        foreach(string target in platformAndTargetsBuildTasks[platform].Keys)
        {
            Information($"\t{GetBuildTaskNameForPlatformTarget(platform, target)}");
        }

        foreach(string target in platformAndTargetsReleaseTasks[platform].Keys)
        {
            Information($"\t{GetReleaseTaskNameForPlatformTarget(platform, target)}");
        }
    }

    Information("");
    Information(" ================================================================== ");
}

void GenerateTasksWithConfiguration(ConfigurationEngine configuration, string intermediate, string artifacts)
{
    List<string> buildTasks = new List<string>();
    List<string> releaseTasks = new List<string>();

    Dictionary<string, List<string>> targetBuildTasks = new Dictionary<string, List<string>>();
    Dictionary<string, List<string>> targetReleaseTasks = new Dictionary<string, List<string>>();
    Dictionary<string, Dictionary<string, List<string>>> platformAndTargetsBuildTasks = new Dictionary<string, Dictionary<string, List<string>>>();
    Dictionary<string, Dictionary<string, List<string>>> platformAndTargetsReleaseTasks = new Dictionary<string, Dictionary<string, List<string>>>();

    foreach (string platform in configuration.GetPlatforms())
    {
        platformAndTargetsBuildTasks.Add(platform, new Dictionary<string, List<string>>());
        platformAndTargetsReleaseTasks.Add(platform, new Dictionary<string, List<string>>());

        foreach (string app in configuration.GetApps())
        {
            foreach (string target in configuration.GetTargets(app))
            {
                string task = GetBuildTaskName(platform, app, target);
                switch(platform)
                {
                    case "android":
                        Task(task)
                            .Does(() =>
                            {
                                RunAndroidBuild(configuration, app, target);
                            });
                        break;
                    case "ios":
                        break;
                    case "dotnet":
                        Task(task)
                            .Does(() =>
                            {
                                RunDotNetBuild(configuration, app, target);
                            });
                        break;
                }
                buildTasks.Add(task);
                if (!targetBuildTasks.ContainsKey(target))
                {
                    targetBuildTasks.Add(target, new List<string>());
                }
                if (!platformAndTargetsBuildTasks[platform].ContainsKey(target))
                {
                    platformAndTargetsBuildTasks[platform].Add(target, new List<string>());
                }
                targetBuildTasks[target].Add(task);
                platformAndTargetsBuildTasks[platform][target].Add(task);


                task = GetReleaseTaskName(platform, app, target);
                switch(platform)
                {
                    case "android":
                        Task(task)
                            .IsDependentOn("clean")
                            .Does(() =>
                            {
                                RunAndroidRelease(configuration, app, target, intermediate, artifacts);
                            });
                        break;
                    case "ios":
                        break;
                    case "dotnet":
                        Task(task)
                            .Does(() =>
                            {
                                RunDotNetRelease(configuration, app, target, intermediate, artifacts);
                            });
                        break;
                }
                
                releaseTasks.Add(task);
                if (!targetReleaseTasks.ContainsKey(target))
                {
                    targetReleaseTasks.Add(target, new List<string>());
                }
                if (!platformAndTargetsReleaseTasks[platform].ContainsKey(target))
                {
                    platformAndTargetsReleaseTasks[platform].Add(target, new List<string>());
                }
                targetReleaseTasks[target].Add(task);
                platformAndTargetsReleaseTasks[platform][target].Add(task);
            }
        }
    }

    foreach (string target in targetBuildTasks.Keys)
    {
        string name = GetBuildTaskNameForTarget(target);
        buildTasks.Add(name);
        var task = Task(name);
        foreach(string taskName in targetBuildTasks[target])
        {
            task.IsDependentOn(taskName);
        }
        task.Does(() => { });

        task = Task(GetReleaseTaskNameForTarget(target))
                    .IsDependentOn("clean");
        foreach(string taskName in targetReleaseTasks[target])
        {
            task.IsDependentOn(taskName);
        }
        task.Does(() => { });
    }

    foreach (string platform in platformAndTargetsBuildTasks.Keys)
    {
        foreach(string target in platformAndTargetsBuildTasks[platform].Keys)
        {
            string name = GetBuildTaskNameForPlatformTarget(platform, target);
            buildTasks.Add(name);
            var task = Task(GetBuildTaskNameForPlatformTarget(platform, target));
            foreach(string taskName in platformAndTargetsBuildTasks[platform][target])
            {
                task.IsDependentOn(taskName);
            }
            task.Does(() => { });
        }

        foreach(string target in platformAndTargetsReleaseTasks[platform].Keys)
        {
            var task = Task(GetReleaseTaskNameForPlatformTarget(platform, target))
                            .IsDependentOn("clean");
            foreach(string taskName in platformAndTargetsReleaseTasks[platform][target])
            {
                task.IsDependentOn(taskName);
            }
            task.Does(() => { });
        }
    }

    var buildTask = Task("build");
    var rebuildTask = Task("rebuild").IsDependentOn("clean");
    foreach(string buildTaskName in buildTasks)
    {
        Task(buildTaskName.Replace("build", "rebuild"))
            .IsDependentOn("clean")
            .IsDependentOn(buildTaskName)
            .Does(() => { });

        buildTask.IsDependentOn(buildTaskName);
        rebuildTask.IsDependentOn(buildTaskName);
    }
    buildTask.Does(() => { });
    rebuildTask.Does(() => { });

    var releaseTask = Task("release").IsDependentOn("clean");
    foreach(string releaseTaskName in releaseTasks)
    {
        releaseTask.IsDependentOn(releaseTaskName);
    }
    releaseTask.Does(() => { });
}

ConfigurationEngine ReadJsonConfiguration(string configurationFile)
{
    if(string.IsNullOrEmpty(configurationFile))
    {
        ThrowError($"Missing json build configuration file");
    }

    if(!FileExists(configurationFile))
    {
        ThrowError($"Missing json build configuration file {configurationFile}");
    }

    return ReadJsonConfigurationFile(configurationFile);
}

string GetBuildTaskNameForTarget(string target)
{
    return $"build-{target}";
}

string GetReleaseTaskNameForTarget(string target)
{
    return $"release-{target}";
}

string GetBuildTaskNameForPlatformTarget(string platform, string target)
{
    return $"{platform}-build-{target}";
}

string GetReleaseTaskNameForPlatformTarget(string platform, string target)
{
    return $"{platform}-release-{target}";
}

string GetBuildTaskName(string platform, string app, string target)
{
    const string CONFIG_ANDROID_BUILD_TASK_FORMAT = "android-build-{0}-{1}";
    const string CONFIG_IOS_BUILD_TASK_FORMAT = "ios-build-{0}-{1}";
    const string CONFIG_DOTNET_BUILD_TASK_FORMAT = "dotnet-build-{0}-{1}";
    
    switch(platform)
    {
        case "android":
            return string.Format(CONFIG_ANDROID_BUILD_TASK_FORMAT, app, target);
        case "ios":
            return string.Format(CONFIG_IOS_BUILD_TASK_FORMAT, app, target);
        case "dotnet":
            return string.Format(CONFIG_DOTNET_BUILD_TASK_FORMAT, app, target);
        default:
            ThrowError($"Not supported platform: {platform}");
            break;
    }
    return null;
}

string GetReleaseTaskName(string platform, string app, string target)
{
    const string CONFIG_ANDROID_RELEASE_TASK_FORMAT = "android-release-{0}-{1}";
    const string CONFIG_IOS_RELEASE_TASK_FORMAT = "ios-release-{0}-{1}";
    const string CONFIG_DOTNET_RELEASE_TASK_FORMAT = "dotnet-release-{0}-{1}";

    switch(platform)
    {
        case "android":
            return string.Format(CONFIG_ANDROID_RELEASE_TASK_FORMAT, app, target);
        case "ios":
            return string.Format(CONFIG_IOS_RELEASE_TASK_FORMAT, app, target);
        case "dotnet":
            return string.Format(CONFIG_DOTNET_RELEASE_TASK_FORMAT, app, target);
        default:
            ThrowError($"Not supported platform: {platform}");
            break;
    }
    return null;
}