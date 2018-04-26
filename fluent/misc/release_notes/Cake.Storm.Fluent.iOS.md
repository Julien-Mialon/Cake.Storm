Version 0.3.2 :
    - Add plist transformation for url scheme array

Version 0.3.1 : 
    - Fix fastlane command when do not have admin access to apple developer account

Version 0.3.0 :
    - Fix fastlane command missing quote around team name

Version 0.2.0 : 
    - Update to Cake 0.22.0 dependencies
    - Remove net45 framework support

Version 0.1.1 : Fix release step not building project instead of solution

Version 0.1.0 : Initial release with tasks to build, package and sign your iOS application.
    - Transform content of Info.plist before build to include version and bundle id
    - Build your iOS application and create an ipa at the end
    - Synchronize provisionning profile using fastlane and sign the package with it