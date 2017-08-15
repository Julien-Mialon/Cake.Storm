using Cake.Storm.Fluent.Android.Interfaces;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.InternalExtensions;
using Cake.Storm.Fluent.Steps;

namespace Cake.Storm.Fluent.Android.Steps
{
	[PreReleaseStep]
	internal class KeystoreValidationStep : IStep
	{
		private readonly string _keyStoreFile;
		private readonly IKeystoreAction _keyStore;

		public KeystoreValidationStep(string keyStoreFile, IKeystoreAction keyStore)
		{
			_keyStoreFile = keyStoreFile;
			_keyStore = keyStore;
		}

		public void Execute(IConfiguration configuration)
		{
			_keyStore.Execute(configuration.AddRootDirectory(_keyStoreFile), configuration);
		}
	}
}