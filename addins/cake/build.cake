#l "../src/scripts/bootstrapper.cake"
#l "../src/scripts/nuget.cake"

const string ROOT = "../";
const string INTERMEDIATE = "../build";
const string ARTIFACTS = "../artifacts";

UseNugetBootstrapper();
UseBootstrapper(ROOT, INTERMEDIATE, ARTIFACTS);
