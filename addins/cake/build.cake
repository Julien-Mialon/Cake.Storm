#l "../src/scripts/bootstrapper.cake"
#l "../src/scripts/nuget.cake"
#l "../src/scripts/versions.cake"

const string ROOT = "../";
const string INTERMEDIATE = "../build";
const string ARTIFACTS = "../artifacts";

string jsonFile = Argument("build-configuration", "build.config.json");
string configurationFile = jsonFile + ".transformed";
System.IO.File.Copy(jsonFile, configurationFile, true);

UseNugetBootstrapper();
UseBootstrapper(configurationFile, ROOT, INTERMEDIATE, ARTIFACTS);

System.IO.File.Delete(configurationFile);
