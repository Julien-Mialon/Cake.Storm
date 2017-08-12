using Cake.Core.IO;
using Cake.Storm.Fluent.Interfaces;

namespace Cake.Storm.Fluent.Android.Interfaces
{
	public interface IKeystore
	{
		IKeystore WithAuthority(string authority);

		IKeystore WithPassword(string password);

		IKeystore WithKeyAlias(string alias);

		IKeystore WithKeyPassword(string password);
		
		IKeystore WithKey(string alias, string password);

		IKeystore WithAllowCreate(bool allowCreate);
	}

	internal interface IKeystoreAction : IKeystore
	{
		void Execute(FilePath keyStoreFile, IConfiguration configuration);

		void Sign(FilePath keyStoreFile, IConfiguration configuration, string sourceApk, string destinationApk);
	}
}