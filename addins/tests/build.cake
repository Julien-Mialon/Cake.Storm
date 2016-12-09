#r "../src/Cake.Storm.JsonBuildConfiguration/bin/Debug/Cake.Storm.JsonBuildConfiguration.dll"
#r "../src/Cake.Storm.Android/bin/Debug/Cake.Storm.Android.dll"

const string OUTPUT_DIR = "output";

// JsonBuildConfiguration tasks
const string JSON_BUILD_CONFIGURATION_INPUTFILE = "build_info.json";

const string TASK_JSON_BUILD_CONFIGURATION_ANDROID = "JsonBuildConfiguration-android";
const string TASK_JSON_BUILD_CONFIGURATION_IOS = "JsonBuildConfiguration-ios";

Task(TASK_JSON_BUILD_CONFIGURATION_ANDROID)
    .Does(() => {
        var configuration = ReadJsonConfigurationFile(JSON_BUILD_CONFIGURATION_INPUTFILE);

        LogJsonBuildConfiguration(configuration.GetAndroid("myproject", "hockeyapp"));
        LogJsonBuildConfiguration(configuration.GetAndroid("myproject", "store"));

        LogJsonBuildConfiguration(configuration.GetAndroid("yourproject", "hockeyapp"));
        LogJsonBuildConfiguration(configuration.GetAndroid("yourproject", "store"));
    });

Task(TASK_JSON_BUILD_CONFIGURATION_IOS)
    .Does(() => {
        var configuration = ReadJsonConfigurationFile(JSON_BUILD_CONFIGURATION_INPUTFILE);

        LogJsonBuildConfiguration(configuration.GetiOS("myproject", "hockeyapp"));
        LogJsonBuildConfiguration(configuration.GetiOS("myproject", "store"));

        LogJsonBuildConfiguration(configuration.GetiOS("yourproject", "hockeyapp"));
        LogJsonBuildConfiguration(configuration.GetiOS("yourproject", "store"));
    });

// Android tasks
const string ANDROID_MANIFEST_INPUTFILE = "AndroidManifest.xml";

const string TASK_ANDROID_SETMANIFEST = "Android-setmanifest";
Task(TASK_ANDROID_SETMANIFEST)
    .Does(() => {
        var manifest = LoadAndroidManifest(ANDROID_MANIFEST_INPUTFILE);

        manifest.Log();

        manifest.SetPackage("fr.julien.myproject");
        manifest.SetVersionName("1.0.0-super");
        manifest.SetVersionCode("42");

        manifest.Log();
        manifest.Save(OUTPUT_DIR + "/AndroidManifest.xml");
    });

// General tasks

const string DEFAULT_TARGET = "all";

Task(DEFAULT_TARGET)
    // JsonBuildConfiguration
    .IsDependentOn(TASK_JSON_BUILD_CONFIGURATION_ANDROID)
    .IsDependentOn(TASK_JSON_BUILD_CONFIGURATION_IOS)
    // Android
    .IsDependentOn(TASK_ANDROID_SETMANIFEST)
    .Does(() => { });

Task("Default")
    .IsDependentOn(DEFAULT_TARGET)
    .Does(() => { });
	
var target = Argument("target", DEFAULT_TARGET);

//start some cleanup before launching system
DeleteDirectory(OUTPUT_DIR, true);
CreateDirectory(OUTPUT_DIR);

RunTarget(target);