#addin "Newtonsoft.Json"
#addin "Cake.Storm.JsonBuildConfiguration"
#addin "Cake.Storm.Android"

#load "./configuration.cake"

void UseBootstrapper(string rootDirectory, string intermediateDirectory, string artifactsDirectory, string defaultTarget = "default")
{
    //generate tasks based on configuration
    ConfigurationEngine conf = ReadJsonConfiguration(Argument("build-configuration", "build.config.json"));
    GenerateTasksWithConfiguration(conf, intermediateDirectory, artifactsDirectory);

    // === task names ===
    const string TASK_CLEAN = "clean";

    // ==== general ====
    Task(TASK_CLEAN)
        .Does(() => {
            string noClean = Argument("no-clean", "false");
            int noCleanLevel = noClean == "true" ? 1 : noClean == "all" ? 2 : 0;
            if(noCleanLevel == 0)
            {
                DeleteAndCreateDirectory(intermediateDirectory);
                DeleteAndCreateDirectory(artifactsDirectory);
            }
            else
            {
                Information("Bypass clean of artifacts and build directory");
            }

            if(noCleanLevel < 2)
            {
                CleanBinObj(rootDirectory);
            }
            else
            {
                Information("Bypass clean of bin & obj directories");
            }
        });

    Task(defaultTarget)
        .Does(() => { 
            OutputHelp(conf);
        });

    RunTarget(Argument("target", defaultTarget));
}

