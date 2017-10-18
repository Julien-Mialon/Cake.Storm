namespace Cake.Storm.Fluent.Resolvers
{
	public interface IValueResolver<TValue>
	{
		TValue Resolve();
	}
}