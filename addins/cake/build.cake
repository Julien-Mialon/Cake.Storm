#r "../src/Cake.Storm.JsonBuildConfiguration/bin/Debug/Cake.Storm.JsonBuildConfiguration.dll"
#r "../src/Cake.Storm.Android/bin/Debug/Cake.Storm.Android.dll"
#l "../src/scripts/generic.cake"

const string ROOT_DIRECTORY = "../";
const string INTERMEDIATE_DIRECTORY = "./intermediate";
const string ARTIFACTS_DIRECTORY = "../artifacts";

//generate tasks based on configuration
ConfigurationEngine conf = ReadJsonConfiguration(Argument("build-configuration", "build.config.json"));
GenerateTasksWithConfiguration(conf, INTERMEDIATE_DIRECTORY, ARTIFACTS_DIRECTORY);

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
