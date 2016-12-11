#load "./android.cake"

// === argument names === 
const string GENERIC_BUILDCONFIGURATION_ARGUMENTNAME = "build-configuration";
const string GENERIC_APP_ARGUMENTNAME = "build-app";
const string GENERIC_TARGET_ARGUMENTNAME = "build-target";

// === task names ===
const string TASK_GENERIC_CLEAN = "generic-clean";
const string TASK_GENERIC_BUILDCONFIGURATION_JSON = "generic-buildconfiguration-json";
const string TASK_GENERIC_ANDROID_BUILD = "generic-android-build";
const string TASK_GENERIC_ANDROID_RELEASE = "generic-android-release";

// === path ===
const string GENERIC_INTERMEDIATE_DIRECTORY = "./intermediate";
const string GENERIC_ARTIFACTS_DIRECTORY = "./artifacts";

// === configuration ===
ConfigurationEngine _genericConfigurationEngine = null;


Task(TASK_GENERIC_BUILDCONFIGURATION_JSON)
    .Does(() => {
        if(_genericConfigurationEngine == null)
        {
            string configurationFile = Argument(GENERIC_BUILDCONFIGURATION_ARGUMENTNAME, "build.config.json");
            if(string.IsNullOrEmpty(configurationFile))
            {
                ThrowError($"Missing json build configuration file");
            }

            if(!FileExists(configurationFile))
            {
                ThrowError($"Missing json build configuration file {configurationFile}");
            }

            _genericConfigurationEngine = ReadJsonConfigurationFile(configurationFile);
        }
    });

Task(TASK_GENERIC_ANDROID_BUILD)
    .IsDependentOn(TASK_GENERIC_BUILDCONFIGURATION_JSON)
    .Does(() => {
        string appArgument = Argument(GENERIC_APP_ARGUMENTNAME, string.Empty);
        string targetArgument = Argument(GENERIC_TARGET_ARGUMENTNAME, string.Empty);

        RunAndroidBuild(_genericConfigurationEngine, appArgument, targetArgument);
    });

Task(TASK_GENERIC_ANDROID_RELEASE)
    .IsDependentOn(TASK_GENERIC_BUILDCONFIGURATION_JSON)
    .Does(() => {
        string appArgument = Argument(GENERIC_APP_ARGUMENTNAME, string.Empty);
        string targetArgument = Argument(GENERIC_TARGET_ARGUMENTNAME, string.Empty);

        RunAndroidRelease(_genericConfigurationEngine, appArgument, targetArgument, GENERIC_INTERMEDIATE_DIRECTORY, GENERIC_ARTIFACTS_DIRECTORY);
    });

Task(TASK_GENERIC_CLEAN)
    .Does(() => {
        DeleteAndCreateDirectory(GENERIC_INTERMEDIATE_DIRECTORY);
        DeleteAndCreateDirectory(GENERIC_ARTIFACTS_DIRECTORY);
    });