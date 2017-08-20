﻿// ReSharper disable once CheckNamespace
namespace System.Collections.Generic
{
	public static class KeyValuePairExtensions
	{
		public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> source, out TKey key, out TValue value)
		{
			key = source.Key;
			value = source.Value;
		}
	}
}