#addin Cake.Storm.Fluent

public ConfigurationBuilder Configure()
{
    return CreateConfigurationBuilder(Task, Setup, Teardown, TaskSetup, TaskTeardown);
}