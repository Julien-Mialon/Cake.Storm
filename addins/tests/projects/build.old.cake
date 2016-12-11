#r "../../src/Cake.Storm.JsonBuildConfiguration/bin/Debug/Cake.Storm.JsonBuildConfiguration.dll"
#r "../../src/Cake.Storm.Android/bin/Debug/Cake.Storm.Android.dll"

const string JSON_CONFIGURATION_FILE = "build_info.json";
const string ROOT_DIR = "./";
string ARTIFACTS_DIR = $"{ROOT_DIR}artifacts/";
string BUILD_DIR = $"{ROOT_DIR}build/";

// === task names ===
const string TASK_CLEAN = "clean";
const string TASK_BUILD = "build";
const string TASK_REBUILD = "rebuild";
const string TASK_RELEASE = "release";

const string TASK_READ_CONFIGURATION = "read-json-configuration";

const string TASK_CONFIGURATION_ANDROID = "configuration-android";
const string TASK_MANIFEST_ANDROID = "manifest-android";
const string TASK_BUILD_ANDROID = "build-android";
const string TASK_KEYSTORE_ANDROID = "keystore-android";
const string TASK_PACKAGE_ANDROID = "package-android";
const string TASK_FULL_ANDROID = "full-android";

// === build vars ===
ConfigurationEngine configurationEngine = null;
AndroidBuildConfiguration androidConfiguration = null;

// === argument vars ===
string appName = Argument<string>("build-app-name", null);
string targetName = Argument<string>("build-target-name", null);

// ==== configuration ====
Task(TASK_READ_CONFIGURATION)
    .Does(() => {
        if(configurationEngine == null)
        {
            if(!FileExists(JSON_CONFIGURATION_FILE))
            {
                Error($"Missing json file {JSON_CONFIGURATION_FILE}");
                return;
            }

            configurationEngine = ReadJsonConfigurationFile(JSON_CONFIGURATION_FILE);
        }
    });
// ==== android ==== 
Task(TASK_CONFIGURATION_ANDROID)
    .IsDependentOn(TASK_READ_CONFIGURATION)
    .Does(() => {
        if(androidConfiguration == null)
        {
            if(string.IsNullOrEmpty(appName) || string.IsNullOrEmpty(targetName))
            {
                Error("Missing argument build-app-name or build-target-name");
                return;
            }

            Information($"Read android configuration for app={appName}, target={targetName}");
            androidConfiguration = configurationEngine.GetAndroid(appName, targetName);

            if(androidConfiguration == null)
            {
                Error($"Invalid android configuration app={appName}, target={targetName}");
                return;
            }
        }
    });

Task(TASK_MANIFEST_ANDROID)
    .IsDependentOn(TASK_CONFIGURATION_ANDROID)
    .Does(() => {
        var manifest = LoadAndroidManifest(androidConfiguration.ManifestFile);
        manifest.Package = androidConfiguration.Package;
        manifest.VersionName = androidConfiguration.Version;
        manifest.VersionCode = androidConfiguration.VersionCode;
        manifest.Save();
    });

Task(TASK_BUILD_ANDROID)
    .IsDependentOn(TASK_MANIFEST_ANDROID)
    .Does(() => {
        DotNetBuild(androidConfiguration.Solution, c => ApplyConfiguration(c, androidConfiguration.BuildProperties));
    });

Task(TASK_KEYSTORE_ANDROID)
    .IsDependentOn(TASK_CONFIGURATION_ANDROID)
    .Does(() => {
        Keystore keystore = null;
        if(FileExists(androidConfiguration.KeystoreFile))
        {
            keystore = LoadKeystore(androidConfiguration.KeystoreFile);
        }
        else
        {
            keystore = CreateKeystore(androidConfiguration.KeystoreFile, androidConfiguration.KeystorePassword, androidConfiguration.KeystoreKeyAlias, androidConfiguration.KeystoreKeyPassword, androidConfiguration.KeystoreAuthority);
        }

        if(!keystore.IsRightPassword(androidConfiguration.KeystorePassword))
        {
            Error("Invalid password for android keystore");
        }

        if(!keystore.HasAlias(androidConfiguration.KeystorePassword, androidConfiguration.KeystoreKeyAlias))
        {
            //create alias
            keystore.CreateAlias(androidConfiguration.KeystorePassword, androidConfiguration.KeystoreKeyAlias, androidConfiguration.KeystoreKeyPassword, androidConfiguration.KeystoreAuthority);
        }
    });

Task(TASK_PACKAGE_ANDROID)
    .IsDependentOn(TASK_BUILD_ANDROID)
    .IsDependentOn(TASK_KEYSTORE_ANDROID)
    .Does(() => {
        var manifest = LoadAndroidManifest(androidConfiguration.ManifestFile);
        var rawApk = PackageForAndroid(androidConfiguration.Project, manifest, c => ApplyConfiguration(c, androidConfiguration.BuildProperties));
        CopyFileToDirectory(rawApk, BUILD_DIR);
        string copiedApk = BUILD_DIR + rawApk.GetFilename();
        string signedApk = BUILD_DIR + rawApk.GetFilenameWithoutExtension() + "-Signed" + rawApk.GetExtension();
        string alignedApk = BUILD_DIR + rawApk.GetFilenameWithoutExtension() + "-Aligned" + rawApk.GetExtension();

        string finalApk = ARTIFACTS_DIR + rawApk.GetFilename();

        SignApk(copiedApk, signedApk, androidConfiguration.KeystoreFile, androidConfiguration.KeystorePassword, androidConfiguration.KeystoreKeyAlias, androidConfiguration.KeystoreKeyPassword);
        VerifyApk(signedApk);
        AlignApk(signedApk, alignedApk);

        CopyFile(alignedApk, finalApk);
    });

Task(TASK_FULL_ANDROID)
    .IsDependentOn(TASK_PACKAGE_ANDROID)
    .Does(() => { });

// ==== general ====
Task(TASK_CLEAN)
    .Does(() => {
        if(DirectoryExists(ARTIFACTS_DIR))
        {
            DeleteDirectory(ARTIFACTS_DIR, true);
        }
        if(DirectoryExists(BUILD_DIR))
        {
            DeleteDirectory(BUILD_DIR, true);
        }
        DeleteDirectories(GetDirectories($"{ROOT_DIR}**/bin"), true);
        DeleteDirectories(GetDirectories($"{ROOT_DIR}**/obj"), true);

        if(!DirectoryExists(ARTIFACTS_DIR))
        {
            CreateDirectory(ARTIFACTS_DIR);
        }
        if(!DirectoryExists(BUILD_DIR))
        {
            CreateDirectory(BUILD_DIR);
        }
    });

Task("default")
    .Does(() => { Information("No default task configured"); });

Task(TASK_BUILD)
    .IsDependentOn(TASK_BUILD_ANDROID)
    .Does(() => { });

Task(TASK_REBUILD)
    .IsDependentOn(TASK_CLEAN)
    .IsDependentOn(TASK_BUILD)
    .Does(() => { });

Task(TASK_RELEASE)
    .IsDependentOn(TASK_REBUILD)
    .IsDependentOn(TASK_FULL_ANDROID)
    .Does(() => { });

RunTarget(Argument("target", "default"));
