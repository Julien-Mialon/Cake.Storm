
Configure() //this must be embedded in a script to pass needed parameters (like Task delegate to create new entry points)
			//generic configuration
	.AddConfiguration(configuration => configuration
		.WithSolution("../Geotraceur.nuget.sln")
		.WithBuildParameter("Configuration", "Release")
	)
	//platforms configuration
	.AddPlatform("android", configuration => configuration
		.WithProject("../Android/Geotraceur.Droid.csproj")
		.UseAndroidTooling()
	)
	.AddPlatform("ios", configuration => configuration
		.WithProject("../iOS/Geotraceur.iOS.csproj")
		.UseiOSTooling()
	)
	//targets configuration
	.AddTarget("hockeyapp", configuration => configuration
		.WithBuildParameter("Platform", "iPhone")
		.UseCodeTransformation("../Constants.cs", transformations => transformations
			.UpdateVariable("API_ROOT", "https://test.com")
		)
		.UsePlatform("android", androidConfiguration => androidConfiguration
			.WithManifest(/*"../Android/Properties/AndroidManifest.xml", */manifest => manifest //manifest path should be optional (project directory /Properties/AndroidManifest.xml)
				.WithPackage("fr.ideine.geotraceur")
				.WithVersionNameFromParameter()
				.WithVersionCodeFromParameter()
			)
			.WithKeystore("../ideine.keystore", keystore => keystore
				.WithAuthority("CN=Julien, OU=Julien, O=Julien, L=LILLE, C=FR")
				.WithPassword("ideine")
				.WithAlias("geotraceur", "password")
			)
		)
		.UsePlatform("ios", iOSConfiguration => iOSConfiguration
			.WithPListFile(/*"../iOS/Info.plist" ,*/ plist => plist //plist path should be optional (project directory / Info.plist)
				.WithBundle("fr.ideine.geotraceur")
				.WithShortVersionFromParameter()
				.WithBuildVersionFromParameter()
			)
			.WithCodeSignKey("iPhone Distribution: xxxxx (XXXXXXXXX)")
			.WithFastlaneProvisionningProfile(fastlane => fastlane
				.WithUserName("julien.mialon@outlook.com")
				.WithTeamName("xxxxx")
				.WithProvisionning("Geotraceur QA")
				.UseAdHoc()  //could be UseDev or UseAppStore
			)
		)
	)
	.AddApplication("geotraceur", application => application
		.UseTarget("hockeyapp", target => target
			.UsePlatform("android")
			.UsePlatform("iOS"))
	)
	.Build();