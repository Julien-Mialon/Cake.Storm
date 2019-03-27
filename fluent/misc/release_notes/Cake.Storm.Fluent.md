Version 0.3.3 : 
    - Add possibilities to handle multiple projects at the same time

Version 0.3.2 :
    - Add value resolvers type to easily handle value coming from string, args or environment variables

Version 0.3.1 : 
    - Implement cache mechanism to avoid rerun unneeded step
    - fix WithVersionFromArguments getting argument on parsing instead of execution

Version 0.3.0 :
    - Implement configuration switch for more flexibility
    - Fix bugs when target/platform was used but does not exists
    - Fix help displaying 'release' instead of 'deploy'
    - Add step to execute some C# code
    - Review step mechanism to have steps that can be executed on multiple step (eg. prebuild, prerelease)

Version 0.2.0 : 
    - Update to Cake 0.22.0 dependencies
    - Remove net45 framework support

Version 0.1.0 : Initial release with infrastructure to generate tasks for clean, build, release and deploy steps.
    - Base configurations to use accross projects
    - Include common steps (clean, nuget restore)