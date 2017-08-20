using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.Storm.Fluent.Common;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.Steps;
using System.Reflection;
using System.Text;

namespace Cake.Storm.Fluent.Models
{
	internal class Configuration : ContextAwareBase, IConfiguration
	{
		private readonly Dictionary<string, IConfigurationItem> _configurationItems = new Dictionary<string, IConfigurationItem>();
		private readonly List<IStep> _steps = new List<IStep>();

		public IEnumerable<IStep> Steps => _steps;

		public Configuration(IFluentContext context) : base(context)
		{
		}

		private Configuration(IFluentContext context, Dictionary<string, IConfigurationItem> configurationItems, IEnumerable<IStep> steps) : this(context)
		{
			_configurationItems = configurationItems;
			_steps = new List<IStep>(steps);
		}

		public IEnumerable<IStep> StepsOf<TStepAttribute>() where TStepAttribute : Attribute
		{
			return _steps.Where(x => x.GetType().GetTypeInfo().GetCustomAttribute<TStepAttribute>(true) != null);
		}

		public void AddStep(IStep step)
		{
			_steps.Add(step);
		}

		public void Add(string key, IConfigurationItem item)
		{
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			if (_configurationItems.ContainsKey(key))
			{
				Context.CakeContext.LogAndThrow($"Key {key} has already been added in the configuration");
			}

			_configurationItems.Add(key, item);
		}

		public TConfigurationItem Get<TConfigurationItem>(string key) where TConfigurationItem : class, IConfigurationItem
		{
			switch (Get(key))
			{
				case TConfigurationItem expectedItem:
					return expectedItem;
				case var item:
					Context.CakeContext.LogAndThrow($"IConfiguration: can not get item with key {key}, expecting type {typeof(TConfigurationItem).Name}, got {item.GetType().Name}");
					break;
			}
			throw new Exception(); //only to make compiler happy, should not happen
		}

		public IConfigurationItem Get(string key) => _configurationItems[key];

		public bool Has(string key) => _configurationItems.ContainsKey(key);

		public bool TryGet(string key, out IConfigurationItem item) => _configurationItems.TryGetValue(key, out item);

		public bool TryGet<TConfigurationItem>(string key, out TConfigurationItem item) where TConfigurationItem : class, IConfigurationItem
		{
			if (TryGet(key, out var existingItem) && existingItem is TConfigurationItem result)
			{
				item = result;
				return true;
			}
			item = null;
			return false;
		}

		public IConfiguration Merge(IConfiguration other)
		{
			if (other is Configuration otherConfiguration)
			{
				Dictionary<string, IConfigurationItem> result = new Dictionary<string, IConfigurationItem>(_configurationItems);
				foreach (var item in otherConfiguration._configurationItems)
				{
					if (result.TryGetValue(item.Key, out var currentItem))
					{
						result[item.Key] = currentItem.Merge(item.Value);
					}
					else
					{
						result.Add(item.Key, item.Value);
					}
				}
				return new Configuration(Context, result, MergeSteps(Steps, otherConfiguration.Steps));
			}

			throw new CakeException($"IConfiguration.Merge {other.GetType().FullName} into {GetType().Name} is not possible");
		}

		protected IEnumerable<IStep> MergeSteps(IEnumerable<IStep> current, IEnumerable<IStep> other)
		{
			List<IStep> steps = new List<IStep>();
			HashSet<Type> encounteredTypes = new HashSet<Type>();
			HashSet<Type> multiTypes = new HashSet<Type>();

			foreach (IStep step in current.Concat(other))
			{
				Type stepType = step.GetType();
				if (encounteredTypes.Contains(stepType))
				{
					if (multiTypes.Contains(stepType))
					{
						steps.Add(step);
					}
				}
				else
				{
					steps.Add(step);

					encounteredTypes.Add(stepType);
					if (stepType.GetTypeInfo().GetCustomAttribute<MultiStepAttribute>(true) != null)
					{
						multiTypes.Add(stepType);
					}
				}
			}
			
			return steps;
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendLine("---- Dump configuration ----");
			foreach (var item in _configurationItems)
			{
				builder.AppendLine($"{item.Key}:");
				builder.AppendLine(item.Value.ToString());
			}
			builder.AppendLine("----------------------------");
			return builder.ToString();
		}
	}
}