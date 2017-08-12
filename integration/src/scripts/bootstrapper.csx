#r "../Cake.Storm.Fluent/bin/Debug/net46/Cake.Storm.Fluent.dll"
#r "../Cake.Storm.Fluent.DotNetCore/bin/Debug/net46/Cake.Storm.Fluent.DotNetCore.dll"
#r "../Cake.Storm.Fluent.Transformations/bin/Debug/net46/Cake.Storm.Fluent.Transformations.dll"
#r "../Cake.Storm.Fluent.iOS/bin/Debug/net46/Cake.Storm.Fluent.iOS.dll"

public ConfigurationBuilder Configure()
{
    ImportDotNetCore();
    ImportTransformations();
    ImportiOS();

    return CreateConfigurationBuilder(Task, Setup, Teardown, TaskSetup, TaskTeardown);
}