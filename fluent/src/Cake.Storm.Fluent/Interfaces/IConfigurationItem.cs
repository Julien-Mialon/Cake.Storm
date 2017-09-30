namespace Cake.Storm.Fluent.Interfaces
{
	public interface IConfigurationItem
	{
		//The idea is to merge the parameter with the current one. With the parameter be the most priority guy
		IConfigurationItem Merge(IConfigurationItem other);
	}
}