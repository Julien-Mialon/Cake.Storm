using System;
using System.Collections.Generic;
using Cake.Storm.Fluent.Steps;

namespace Cake.Storm.Fluent.Interfaces
{
	public interface IConfiguration : IContextAware
	{
		IEnumerable<IStep> Steps { get; }

		IEnumerable<IStep> StepsOf<TStepAttribute>() where TStepAttribute : Attribute;

		void AddStep(IStep step);

		void Add(string key, IConfigurationItem item);

		TConfigurationItem Get<TConfigurationItem>(string key) where TConfigurationItem : class, IConfigurationItem;

		IConfigurationItem Get(string key);

		bool Has(string key);

		bool TryGet(string key, out IConfigurationItem item);

		bool TryGet<TConfigurationItem>(string key, out TConfigurationItem item) where TConfigurationItem : class, IConfigurationItem;

		IConfiguration Merge(IConfiguration other);
	}
}