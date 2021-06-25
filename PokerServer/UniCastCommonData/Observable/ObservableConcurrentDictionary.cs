using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UniCastCommonData.Network;

namespace UniCastCommonData.Observable
{
	/// <summary>
	/// List changed event handler.
	/// </summary>
	/// <typeparam name="T">List type.</typeparam>
	/// <param name="sender">Sender.</param>
	/// <param name="e">List changed even arguments.</param>
	public delegate void ConcurrentDictionaryChangedEventHandler<TKey, TValue>(ObservableConcurrentDictionary<TKey, TValue> sender, ConcurrentDictionaryChangedEventArgs<TKey, TValue> e);

	public class ObservableConcurrentDictionary<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IDictionary<TKey, TValue>, IReadOnlyCollection<KeyValuePair<TKey, TValue>>, IReadOnlyDictionary<TKey, TValue>, ICollection, IDictionary where TKey : notnull
	{
		private ConcurrentDictionary<TKey, TValue> _dictionary;


		IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => ((IReadOnlyDictionary<TKey, TValue>)_dictionary).Keys;

		ICollection IDictionary.Keys => ((IDictionary)_dictionary).Keys;

		public ICollection<TKey> Keys => ((IDictionary<TKey, TValue>)_dictionary).Keys;


		IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => ((IReadOnlyDictionary<TKey, TValue>)_dictionary).Values;

		ICollection IDictionary.Values => ((IDictionary)_dictionary).Values;

		public ICollection<TValue> Values => ((IDictionary<TKey, TValue>)_dictionary).Values;


		public bool IsSynchronized => ((ICollection)_dictionary).IsSynchronized;

		public object SyncRoot => ((ICollection)_dictionary).SyncRoot;

		public bool IsFixedSize => ((IDictionary)_dictionary).IsFixedSize;

		public int Count => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Count;

		public bool IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).IsReadOnly;


		/// <summary>
		/// Added item event.
		/// </summary>
		public event ConcurrentDictionaryChangedEventHandler<TKey, TValue> ItemAdded;
		/// <summary>
		/// Removed item event.
		/// </summary>
		public event ConcurrentDictionaryChangedEventHandler<TKey, TValue> ItemRemoved;

		void OnEvent(ConcurrentDictionaryChangedEventHandler<TKey, TValue> e, TKey key, TValue value)
		{
			if (e != null)
			{
				e(this, new ConcurrentDictionaryChangedEventArgs<TKey, TValue>(key, value));
			}
		}


		public TValue this[TKey key]
		{
			get
			{
				return _dictionary[key];
			}
			set
			{
				OnEvent(ItemRemoved, key, _dictionary[key]);
				_dictionary[key] = value;
				OnEvent(ItemAdded, key, value);
			}
		}

		object? IDictionary.this[object key]
		{
			get
			{
				return ((IDictionary)_dictionary)[key];
			}
			set
			{
				OnEvent(ItemRemoved, (TKey)key, _dictionary[(TKey)key]);
				((IDictionary)_dictionary)[key] = value;
				OnEvent(ItemAdded, (TKey)key, (TValue)value);
			}
		}

		public ObservableConcurrentDictionary()
		{
			_dictionary = new ConcurrentDictionary<TKey, TValue>();
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2" /> class that contains elements copied from the specified <see cref="T:System.Collections.Generic.IEnumerable`1" />, has the default concurrency level, has the default initial capacity, and uses the default comparer for the key type.</summary>
		/// <param name="collection">The <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements are copied to the new <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2" />.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="collection" /> or any of its keys is  <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="collection" /> contains one or more duplicate keys.</exception>
		public ObservableConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
		{
			_dictionary = new ConcurrentDictionary<TKey, TValue>(collection);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2" /> class that contains elements copied from the specified <see cref="T:System.Collections.IEnumerable" /> has the default concurrency level, has the default initial capacity, and uses the specified  <see cref="T:System.Collections.Generic.IEqualityComparer`1" />.</summary>
		/// <param name="collection">The <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements are copied to the new <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2" />.</param>
		/// <param name="comparer">The <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> implementation to use when comparing keys.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="collection" /> or <paramref name="comparer" /> is <see langword="null" />.</exception>
		public ObservableConcurrentDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey>? comparer)
		{
			_dictionary = new ConcurrentDictionary<TKey, TValue>(collection, comparer);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2" /> class that is empty, has the default concurrency level and capacity, and uses the specified <see cref="T:System.Collections.Generic.IEqualityComparer`1" />.</summary>
		/// <param name="comparer">The equality comparison implementation to use when comparing keys.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="comparer" /> is <see langword="null" />.</exception>
		public ObservableConcurrentDictionary(IEqualityComparer<TKey>? comparer)
		{
			_dictionary = new ConcurrentDictionary<TKey, TValue>(comparer);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2" /> class that contains elements copied from the specified <see cref="T:System.Collections.IEnumerable" />, and uses the specified <see cref="T:System.Collections.Generic.IEqualityComparer`1" />.</summary>
		/// <param name="concurrencyLevel">The estimated number of threads that will update the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2" /> concurrently.</param>
		/// <param name="collection">The <see cref="T:System.Collections.Generic.IEnumerable`1" /> whose elements are copied to the new <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2" />.</param>
		/// <param name="comparer">The <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> implementation to use when comparing keys.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="collection" /> or <paramref name="comparer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="concurrencyLevel" /> is less than 1.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="collection" /> contains one or more duplicate keys.</exception>
		public ObservableConcurrentDictionary(int concurrencyLevel, IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey>? comparer)
		{
			_dictionary = new ConcurrentDictionary<TKey, TValue>(concurrencyLevel, collection, comparer);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2" /> class that is empty, has the specified concurrency level and capacity, and uses the default comparer for the key type.</summary>
		/// <param name="concurrencyLevel">The estimated number of threads that will update the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2" /> concurrently.</param>
		/// <param name="capacity">The initial number of elements that the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2" /> can contain.</param>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="concurrencyLevel" /> is less than 1.  
		///
		///  -or-  
		///
		///  <paramref name="capacity" /> is less than 0.</exception>
		public ObservableConcurrentDictionary(int concurrencyLevel, int capacity)
		{
			_dictionary = new ConcurrentDictionary<TKey, TValue>(concurrencyLevel, capacity);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2" /> class that is empty, has the specified concurrency level, has the specified initial capacity, and uses the specified <see cref="T:System.Collections.Generic.IEqualityComparer`1" />.</summary>
		/// <param name="concurrencyLevel">The estimated number of threads that will update the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2" /> concurrently.</param>
		/// <param name="capacity">The initial number of elements that the <see cref="T:System.Collections.Concurrent.ConcurrentDictionary`2" /> can contain.</param>
		/// <param name="comparer">The <see cref="T:System.Collections.Generic.IEqualityComparer`1" /> implementation to use when comparing keys.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="comparer" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="concurrencyLevel" /> or <paramref name="capacity" /> is less than 1.</exception>
		public ObservableConcurrentDictionary(int concurrencyLevel, int capacity, IEqualityComparer<TKey>? comparer)
		{
			_dictionary = new ConcurrentDictionary<TKey, TValue>(concurrencyLevel, capacity, comparer);
		}

		public void CopyTo(Array array, int index)
		{
			((ICollection)_dictionary).CopyTo(array, index);
		}

		public void Add(object key, object value)
		{
			((IDictionary)_dictionary).Add(key, value);
			OnEvent(ItemAdded, (TKey)key, (TValue)value);
		}

		public bool Contains(object key)
		{
			return ((IDictionary)_dictionary).Contains(key);
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return ((IDictionary)_dictionary).GetEnumerator();
		}

		public void Remove(object key)
		{
			((IDictionary)_dictionary).Remove(key);
			OnEvent(ItemRemoved, (TKey)key, _dictionary[(TKey)key]);
		}

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Add(item);
			OnEvent(ItemAdded, item.Key, item.Value);
		}

		public void Clear()
		{
			((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Clear();
		}

		public void TryRemove(TKey id, out TValue temp)
		{
			_dictionary.TryRemove(id, out temp);
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Contains(item);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			bool isRemoved = ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).Remove(item);

			if (isRemoved)
			{
				OnEvent(ItemRemoved, item.Key, item.Value);
			}

			return isRemoved;
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return ((IEnumerable<KeyValuePair<TKey, TValue>>)_dictionary).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)_dictionary).GetEnumerator();
		}

		public void Add(TKey key, TValue value)
		{
			((IDictionary<TKey, TValue>)_dictionary).Add(key, value);
			OnEvent(ItemAdded, key, value);
		}

		public bool ContainsKey(TKey key)
		{
			return ((IDictionary<TKey, TValue>)_dictionary).ContainsKey(key);
		}

		public bool Remove(TKey key)
		{
			TValue cachedValue = _dictionary[key];

			bool isRemoved = ((IDictionary<TKey, TValue>)_dictionary).Remove(key);

			if (isRemoved)
			{
				OnEvent(ItemRemoved, key, cachedValue);
			}

			return isRemoved;
		}

		public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
		{
			return ((IDictionary<TKey, TValue>)_dictionary).TryGetValue(key, out value);
		}
	}
}
