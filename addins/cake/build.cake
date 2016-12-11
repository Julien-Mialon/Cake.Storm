#addin "Newtonsoft.Json"
#addin "Cake.Storm.Android"
#addin "Cake.Storm.JsonBuildConfiguration"
#addin "Cake.Storm.Nuget"

#l "../src/scripts/generic.cake"
#l "../src/scripts/nuget.cake"

const string ROOT_DIRECTORY = "../";
const string INTERMEDIATE_DIRECTORY = "./intermediate";
const string ARTIFACTS_DIRECTORY = "../artifacts";

//generate tasks based on configuration
ConfigurationEngine conf = ReadJsonConfiguration(Argument("build-configuration", "build.config.json"));
GenerateTasksWithConfiguration(conf, INTERMEDIATE_DIRECTORY, ARTIFACTS_DIRECTORY);

List<NugetAppConfiguration> nugetConfigurations = ReadNugetConfigurations(Argument("nuget-configuration", "nuget.config.json"));
GenerateTasksForNuget(nugetConfigurations);

// === task names ===
const string TASK_CLEAN = "clean";

// ==== general ====
Task(TASK_CLEAN)
    .IsDependentOn(TASK_GENERIC_CLEAN)
    .Does(() => {
        DeleteAndCreateDirectory(INTERMEDIATE_DIRECTORY);
        DeleteAndCreateDirectory(ARTIFACTS_DIRECTORY);
        CleanBinObj(ROOT_DIRECTORY);
    });

Task("default")
    .IsDependentOn(TASK_GENERIC_HELP)
    .Does(() => { });

RunTarget(Argument("target", "default"));
