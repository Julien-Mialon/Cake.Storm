#l "./common.cake"

AndroidBuildConfiguration AndroidReadConfiguration(ConfigurationEngine configuration, string appName, string targetName)
{
    if(configuration == null)
    {
        ThrowError("Configuration can not be null");
    }

    if(string.IsNullOrEmpty(appName) || string.IsNullOrEmpty(targetName))
    {
        ThrowError($"Invalid value for appName: {appName} or targetName: {targetName}");
    }

    Information($"Read android configuration for app={appName}, target={targetName}");
    var result = configuration.GetAndroid(appName, targetName);

    if(result == null)
    {
        ThrowError($"Android configuration for app={appName}, target={targetName} not found");
    }

    return result;
}

void AndroidTransformManifest(AndroidBuildConfiguration configuration)
{
    var manifest = LoadAndroidManifest(configuration.ManifestFile);
    manifest.Package = configuration.Package;
    manifest.VersionName = configuration.Version;
    manifest.VersionCode = configuration.VersionCode;
    manifest.Save();
}

void AndroidEnsureKeystoreExists(AndroidBuildConfiguration configuration)
{
    Keystore keystore = null;
    if(FileExists(configuration.KeystoreFile))
    {
        keystore = LoadKeystore(configuration.KeystoreFile);

        if(!keystore.IsRightPassword(configuration.KeystorePassword))
        {
            ThrowError($"Invalid password for android keystore {configuration.KeystoreFile}");
        }

        if(!keystore.HasAlias(configuration.KeystorePassword, configuration.KeystoreKeyAlias))
        {
            keystore.CreateAlias(configuration.KeystorePassword, configuration.KeystoreKeyAlias, configuration.KeystoreKeyPassword, configuration.KeystoreAuthority);
        }
    }
    else
    {
        keystore = CreateKeystore(configuration.KeystoreFile, configuration.KeystorePassword, configuration.KeystoreKeyAlias, configuration.KeystoreKeyPassword, configuration.KeystoreAuthority);
    }
}

void AndroidPackage(AndroidBuildConfiguration configuration, string intermediateDirectory, string outputDirectory)
{
    AndroidManifest manifest = LoadAndroidManifest(configuration.ManifestFile);

    FilePath builtApk = PackageForAndroid(configuration.Project, manifest, c => ApplyConfiguration(c, configuration.BuildProperties));
    string intermediatePath = CombinePath(intermediateDirectory, configuration.PlatformName, configuration.TargetName, configuration.AppName);
    string outputPath = CombinePath(outputDirectory, configuration.PlatformName, configuration.TargetName, configuration.AppName);

    //create directory if needed
    EnsureDirectoryExists(intermediatePath);
    EnsureDirectoryExists(outputPath);
    
    string apkName = builtApk.GetFilename().ToString();
    string apkNameWithoutExtension = builtApk.GetFilenameWithoutExtension().ToString();
    string apkExtension = builtApk.GetExtension();

    string sourceApk = CombinePath(intermediatePath, apkName);
    CopyFile(builtApk, sourceApk);

    string signedApk = CombinePath(intermediatePath, $"{apkNameWithoutExtension}-Signed{apkExtension}");
    SignApk(sourceApk, signedApk, configuration.KeystoreFile, configuration.KeystorePassword, configuration.KeystoreKeyAlias, configuration.KeystoreKeyPassword);
    VerifyApk(signedApk);

    string alignedApk = CombinePath(intermediatePath, $"{apkNameWithoutExtension}-Aligned{apkExtension}");
    AlignApk(signedApk, alignedApk);

    string resultApk = CombinePath(outputPath, apkName);
    CopyFile(alignedApk, resultApk);
}

void RunAndroidBuild(ConfigurationEngine configuration, string app, string target)
{
    AndroidBuildConfiguration androidConfiguration = AndroidReadConfiguration(configuration, app, target);
    AndroidTransformManifest(androidConfiguration);

    BuildWithConfiguration(androidConfiguration);
}

void RunAndroidRelease(ConfigurationEngine configuration, string app, string target, string intermediate, string output)
{
    AndroidBuildConfiguration androidConfiguration = AndroidReadConfiguration(configuration, app, target);
    AndroidTransformManifest(androidConfiguration);

    BuildWithConfiguration(androidConfiguration);

    AndroidEnsureKeystoreExists(androidConfiguration);
    AndroidPackage(androidConfiguration, intermediate, output);
}
