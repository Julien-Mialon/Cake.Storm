#r "../src/Cake.Storm.JsonBuildConfiguration/bin/Debug/Cake.Storm.JsonBuildConfiguration.dll"


// JsonBuildConfiguration tasks
const string JSON_BUILD_CONFIGURATION_INPUTFILE = "build_info.json";

const string TASK_JSON_BUILD_CONFIGURATION_ANDROID = "JsonBuildConfiguration-android";


Task(TASK_JSON_BUILD_CONFIGURATION_ANDROID)
    .Does(() => {
        var configuration = ReadJsonConfigurationFile(JSON_BUILD_CONFIGURATION_INPUTFILE);

        LogJsonBuildConfiguration(configuration.GetAndroid("myproject", "hockeyapp"));
        LogJsonBuildConfiguration(configuration.GetAndroid("myproject", "store"));

        LogJsonBuildConfiguration(configuration.GetAndroid("yourproject", "hockeyapp"));
        LogJsonBuildConfiguration(configuration.GetAndroid("yourproject", "store"));
    });


// General tasks

const string DEFAULT_TARGET = "all";

Task(DEFAULT_TARGET)
    .IsDependentOn(TASK_JSON_BUILD_CONFIGURATION_ANDROID)
    .Does(() => { });

Task("Default")
    .IsDependentOn(DEFAULT_TARGET)
    .Does(() => { });
	
var target = Argument("target", DEFAULT_TARGET);
RunTarget(target);