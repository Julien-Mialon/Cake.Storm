using Cake.Common.IO;
using Cake.Core;
using Cake.Storm.Fluent.Android.Commands;
using Cake.Storm.Fluent.Android.Common;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;
using Cake.Storm.Fluent.Steps;

namespace Cake.Storm.Fluent.Android.Steps
{
	[PreReleaseStep]
	internal class KeystoreValidationStep : IStep
	{
		public void Execute(IConfiguration configuration, StepType currentStep)
		{
			if (!configuration.TryGetSimple(AndroidConstants.ANDROID_KEYSTORE_FILE, out string keyStoreFile))
			{
				configuration.LogAndThrow($"Missing path for keystore");
			}
			keyStoreFile = configuration.AddRootDirectory(keyStoreFile);
			
			if (!configuration.TryGetSimple(AndroidConstants.ANDROID_KEYSTORE_PASSWORD, out string password))
			{
				configuration.LogAndThrow($"Missing password for keystore");
			}
			if (!configuration.TryGetSimple(AndroidConstants.ANDROID_KEYSTORE_KEYALIAS, out string keyAlias))
			{
				configuration.LogAndThrow($"Missing alias for keystore");
			}
			if (!configuration.TryGetSimple(AndroidConstants.ANDROID_KEYSTORE_KEYPASSWORD, out string keyPassword))
			{
				configuration.LogAndThrow($"Missing alias password for keystore");
			}

			if (!configuration.TryGetSimple(AndroidConstants.ANDROID_KEYSTORE_ALLOW_CREATE, out bool allowCreate))
			{
				allowCreate = false;
			}
			

			KeytoolCommand command = new KeytoolCommand(configuration.Context.CakeContext);
			if (configuration.Context.CakeContext.FileExists(keyStoreFile))
			{
				if (!command.IsRightPassword(keyStoreFile, password))
				{
					configuration.Context.CakeContext.LogAndThrow($"Invalid password for keystore {keyStoreFile}");
				}

				if (command.HasAlias(keyStoreFile, password, keyAlias))
				{
					return;
				}
			}
			
			if (allowCreate)
			{
				if (!configuration.TryGetSimple(AndroidConstants.ANDROID_KEYSTORE_AUTHORITY, out string authority))
				{
					configuration.Context.CakeContext.LogAndThrow($"Missing authority to create alias {keyAlias} in keystore {keyStoreFile}");
				}
				command.CreateAlias(keyStoreFile, password, keyAlias, keyPassword, authority);
			}
			else
			{
				configuration.Context.CakeContext.LogAndThrow($"Missing alias {keyAlias} for keystore {keyStoreFile} and we are not allowed to create it");
			}
		}
	}
}