﻿using System.Collections;
using System.Collections.Generic;

namespace UniCastCommonData.Observable
{
	/// <summary>
	/// List changed event handler.
	/// </summary>
	/// <typeparam name="T">List type.</typeparam>
	/// <param name="sender">Sender.</param>
	/// <param name="e">List changed even arguments.</param>
	public delegate void ListChangedEventHandler<T>(ObservableList<T> sender, ListChangedEventArgs<T> e);

	/// <summary>
	/// Observable list.
	/// </summary>
	/// <typeparam name="T">Type of the list.</typeparam>
	public class ObservableList<T> : IList<T>
	{
		IList<T> _list;

		/// <summary>
		/// Added item event.
		/// </summary>
		public event ListChangedEventHandler<T> ItemAdded;
		/// <summary>
		/// Removed item event.
		/// </summary>
		public event ListChangedEventHandler<T> ItemRemoved;

		/// <summary>
		/// Accessor.
		/// </summary>
		/// <param name="index">Item index.</param>
		/// <returns>The item at the provided index.</returns>
		public T this[int index]
		{
			get
			{
				return _list[index];
			}
			set
			{
				OnEvent(ItemRemoved, index, _list[index]);
				_list[index] = value;
				OnEvent(ItemAdded, index, value);
			}
		}

		/// <summary>
		/// Number of elements in the list.
		/// </summary>
		public int Count => _list.Count;

		/// <summary>
		/// Is the list read only?
		/// </summary>
		public bool IsReadOnly => false;

		/// <summary>
		/// Default Constructor.
		/// </summary>
		public ObservableList() : this(0) { }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="capacity">Allocation size.</param>
		public ObservableList(int capacity)
		{
			_list = new List<T>(capacity);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="collection">Input list.</param>
		public ObservableList(IEnumerable<T> collection)
		{
			_list = new List<T>(collection);
		}

		void OnEvent(ListChangedEventHandler<T> e, int index, T item)
		{
			if (e != null)
			{
				e(this, new ListChangedEventArgs<T>(index, item));
			}
		}

		/// <summary>
		/// Check if an element is present in the list.
		/// </summary>
		/// <param name="item">Item to test against.</param>
		/// <returns>True if the item is in the list.</returns>
		public bool Contains(T item)
		{
			return _list.Contains(item);
		}

		/// <summary>
		/// Get the index of an item.
		/// </summary>
		/// <param name="item">The object to locate in the list.</param>
		/// <returns>The index of the item in the list if it exists, -1 otherwise.</returns>
		public int IndexOf(T item)
		{
			return _list.IndexOf(item);
		}

		/// <summary>
		/// Add an item to the list.
		/// </summary>
		/// <param name="item">Item to add to the list.</param>
		public void Add(T item)
		{
			_list.Add(item);
			OnEvent(ItemAdded, _list.IndexOf(item), item);
		}

		/// <summary>
		/// Add multiple objects to the list.
		/// </summary>
		/// <param name="items">Items to add to the list.</param>
		public void Add(params T[] items)
		{
			foreach (var i in items)
				Add(i);
		}

		/// <summary>
		/// Insert an item in the list.
		/// </summary>
		/// <param name="index">Index at which to insert the new item.</param>
		/// <param name="item">Item to insert in the list.</param>
		public void Insert(int index, T item)
		{
			_list.Insert(index, item);
			OnEvent(ItemAdded, index, item);
		}

		/// <summary>
		/// Remove an item from the list.
		/// </summary>
		/// <param name="item">Item to remove from the list.</param>
		/// <returns>True if the item was successfuly removed. False otherise.</returns>
		public bool Remove(T item)
		{
			int index = _list.IndexOf(item);
			bool ret = _list.Remove(item);
			if (ret)
				OnEvent(ItemRemoved, index, item);
			return ret;
		}

		/// <summary>
		/// Remove multiple items from the list.
		/// </summary>
		/// <param name="items">Items to remove from the list.</param>
		/// <returns>The number of removed items.</returns>
		public int Remove(params T[] items)
		{
			if (items == null)
				return 0;

			int count = 0;

			foreach (var i in items)
				count += Remove(i) ? 1 : 0;

			return count;
		}

		/// <summary>
		/// Remove an item at a specific index.
		/// </summary>
		/// <param name="index">Index of the item to remove.</param>
		public void RemoveAt(int index)
		{
			var item = _list[index];
			_list.RemoveAt(index);
			OnEvent(ItemRemoved, index, item);
		}

		/// <summary>
		/// Clear the list.
		/// </summary>
		public void Clear()
		{
			for (int i = 0; i < Count; i++)
				RemoveAt(i);
		}

		/// <summary>
		/// Copy items in the list to an array.
		/// </summary>
		/// <param name="array">Destination array.</param>
		/// <param name="arrayIndex">Starting index.</param>
		public void CopyTo(T[] array, int arrayIndex)
		{
			_list.CopyTo(array, arrayIndex);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
