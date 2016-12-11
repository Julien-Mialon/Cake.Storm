#r "../../src/Cake.Storm.JsonBuildConfiguration/bin/Debug/Cake.Storm.JsonBuildConfiguration.dll"
#r "../../src/Cake.Storm.Android/bin/Debug/Cake.Storm.Android.dll"
#l "../../src/scripts/generic.cake"

const string ROOT_DIRECTORY = "./";

// === task names ===
const string TASK_CLEAN = "clean";
const string TASK_BUILD = "build";
const string TASK_REBUILD = "rebuild";
const string TASK_RELEASE = "release";

// ==== general ====
Task(TASK_CLEAN)
    .IsDependentOn(TASK_GENERIC_CLEAN)
    .Does(() => {
       CleanBinObj(ROOT_DIRECTORY);
    });

Task("default")
    .Does(() => { Information("No default task configured"); });

Task(TASK_BUILD)
    .IsDependentOn(TASK_GENERIC_ANDROID_BUILD)
    .Does(() => { });

Task(TASK_REBUILD)
    .IsDependentOn(TASK_CLEAN)
    .IsDependentOn(TASK_BUILD)
    .Does(() => { });

Task(TASK_RELEASE)
    .IsDependentOn(TASK_CLEAN)
    .IsDependentOn(TASK_GENERIC_ANDROID_RELEASE)
    .Does(() => { });

RunTarget(Argument("target", "default"));
