#addin "Cake.Storm.Nuget"

void UseNugetBootstrapper()
{
    List<NugetAppConfiguration> nugetConfigurations = ReadNugetConfigurations(Argument("nuget-configuration", "nuget.config.json"));
    GenerateTasksForNuget(nugetConfigurations);
}

void GenerateTasksForNuget(List<NugetAppConfiguration> configurations)
{
    const string NUSPEC = "nuget-nuspec";
    const string PACK = "nuget-pack";
    const string PUSH = "nuget-push";

    const string NUSPEC_FORMAT = "nuget-nuspec-{0}";
    const string PACK_FORMAT = "nuget-pack-{0}";
    const string PUSH_FORMAT = "nuget-push-{0}";

    List<string> nuspecTasks = new List<string>();
    List<string> packTasks = new List<string>();
    List<string> pushTasks = new List<string>();

    foreach(NugetAppConfiguration configuration in configurations)
    {
        string nuspecTaskName = string.Format(NUSPEC_FORMAT, configuration.Name);
        string packTaskName = string.Format(PACK_FORMAT, configuration.Name);
        string pushTaskName = string.Format(PUSH_FORMAT, configuration.Name);

        Task(nuspecTaskName).Does(() => CreateNuspecForRelease(configuration));
        Task(packTaskName)
            .IsDependentOn(nuspecTaskName)
            .Does(() => NuGetPack(configuration));
        Task(pushTaskName)
            .IsDependentOn(packTaskName)
            .Does(() => NuGetPush(configuration));

        nuspecTasks.Add(nuspecTaskName);
        packTasks.Add(packTaskName);
        pushTasks.Add(pushTaskName);
    }

    var task = Task(NUSPEC);
    foreach(string taskName in nuspecTasks)
    {
        task.IsDependentOn(taskName);
    }
    task.Does(() => { });

    task = Task(PACK);
    foreach(string taskName in packTasks)
    {
        task.IsDependentOn(taskName);
    }
    task.Does(() => { });

    task = Task(PUSH);
    foreach(string taskName in pushTasks)
    {
        task.IsDependentOn(taskName);
    }
    task.Does(() => { });
}