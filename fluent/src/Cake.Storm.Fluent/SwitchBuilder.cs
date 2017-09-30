using System;
using System.Collections.Generic;
using Cake.Core;
using Cake.Storm.Fluent.Interfaces;
using Cake.Storm.Fluent.Models;

namespace Cake.Storm.Fluent
{
	internal class SwitchBuilder : ISwitchBuilder
	{
		private readonly IFluentContext _context;
		private readonly string _switchName;
		
		private readonly Dictionary<string, ISwitchConfiguration> _values = new Dictionary<string, ISwitchConfiguration>();
		private string _selectedValueName;
		private bool _selectedValueDefined;
		
		public SwitchBuilder(IFluentContext context, string switchName)
		{
			_context = context;
			_switchName = switchName;
		}

		public ISwitchBuilder Add(string name, Action<ISwitchConfiguration> action = null)
		{
			if (_values.ContainsKey(name))
			{
				_context.CakeContext.LogAndThrow($"Value name {name} already exists for switch {_switchName}");
			}
			
			ISwitchConfiguration switchConfiguration = new SwitchConfiguration(_context);
			action?.Invoke(switchConfiguration);
			_values.Add(name, switchConfiguration);

			return this;
		}

		public ISwitchBuilder Use(string name)
		{
			if (_selectedValueDefined)
			{
				_context.CakeContext.LogAndThrow($"Selected value has alreay been defined for switch {_switchName}");
			}

			_selectedValueDefined = true;
			_selectedValueName = name;
			return this;
		}

		public ISwitchConfiguration GetSelectedConfiguration()
		{
			if (!_selectedValueDefined)
			{
				_context.CakeContext.LogAndThrow($"Value for switch {_switchName} has not been selected");
			}
			
			if (!_values.TryGetValue(_selectedValueName, out ISwitchConfiguration configuration))
			{
				_context.CakeContext.LogAndThrow($"Value {_selectedValueName} does not exists for switch {_switchName}");
			}
			return configuration;
		}
	}
}