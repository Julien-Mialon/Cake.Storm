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