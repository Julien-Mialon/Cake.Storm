
void GenerateTasksForHockeyApp(List<HockeyAppConfiguration> configurations)
{
    const string HOCKEYAPP = "hockeyapp";
    const string HOCKEYAPP_FORMAT = "hockeyapp-{0}";
    
    List<string> hockeyAppTasks = new List<string>();
    
    foreach(HockeyAppConfiguration configuration in configurations)
    {
        string hockeyAppTaskName = string.Format(HOCKEYAPP_FORMAT, configuration.Name);

        Task(hockeyAppTaskName).Does(() => UploadToHockeyApp(configuration));
        hockeyAppTasks.Add(hockeyAppTaskName);
    }

    var task = Task(HOCKEYAPP);
    foreach(string taskName in hockeyAppTasks)
    {
        task.IsDependentOn(taskName);
    }
    task.Does(() => { });
}