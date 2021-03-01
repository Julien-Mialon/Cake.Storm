#r "../Cake.Storm.Fluent/bin/Debug/netstandard2.0/Cake.Storm.Fluent.dll"
#r "../Cake.Storm.Fluent.Android/bin/Debug/netstandard2.0/Cake.Storm.Fluent.Android.dll"
#r "../Cake.Storm.Fluent.DotNetCore/bin/Debug/netstandard2.0/Cake.Storm.Fluent.DotNetCore.dll"
#r "../Cake.Storm.Fluent.iOS/bin/Debug/netstandard2.0/Cake.Storm.Fluent.iOS.dll"
#r "../Cake.Storm.Fluent.Transformations/bin/Debug/netstandard2.0/Cake.Storm.Fluent.Transformations.dll"
#r "../Cake.Storm.Fluent.NuGet/bin/Debug/netstandard2.0/Cake.Storm.Fluent.NuGet.dll"

public ConfigurationBuilder Configure()
{
    ImportDotNetCore();
    ImportTransformations();
    ImportiOS();
    ImportAndroid();
    ImportNuget();

    return CreateConfigurationBuilder(Task, Setup, Teardown, TaskSetup, TaskTeardown);
}