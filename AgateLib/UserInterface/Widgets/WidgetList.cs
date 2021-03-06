﻿//     The contents of this file are subject to the Mozilla Public License
//     Version 1.1 (the "License"); you may not use this file except in
//     compliance with the License. You may obtain a copy of the License at
//     http://www.mozilla.org/MPL/
//
//     Software distributed under the License is distributed on an "AS IS"
//     basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
//     License for the specific language governing rights and limitations
//     under the License.
//
//     The Original Code is AgateLib.
//
//     The Initial Developer of the Original Code is Erik Ylvisaker.
//     Portions created by Erik Ylvisaker are Copyright (C) 2006-2017.
//     All Rights Reserved.
//
//     Contributor(s): Erik Ylvisaker
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AgateLib.Extensions.Collections.Generic;
using AgateLib.Geometry;

namespace AgateLib.UserInterface.Widgets
{
	public class WidgetList : IList<Widget>
	{
		List<Widget> mItems = new List<Widget>();
		Container mParent;

		public WidgetList(Container parent)
		{
			mParent = parent;
		}

		public int IndexOf(Widget item)
		{
			return mItems.IndexOf(item);
		}

        public void Sort(Comparison<Widget> comparison)
        {
            mItems.InsertionSort(comparison);
        }

		protected virtual void ValidateItem(Widget item)
		{
		}

		public void Insert(int index, Widget item)
		{
			if (item == null) throw new ArgumentNullException("item");
			if (Contains(item))
				throw new InvalidOperationException("item is already contained in this lsit!");
			if (item.Parent != null)
				throw new InvalidOperationException("item already has a parent!");

			ValidateItem(item);

			mItems.Insert(index, item);

			item.Parent = mParent;

			OnWidgetAdded(item);
		}

		public void RemoveAt(int index)
		{
			mParent.LayoutDirty = true;

			mItems[index].Parent = null;
			OnWidgetRemoved(mItems[index]);
			mItems.RemoveAt(index);
		}

		public Widget this[int index]
		{
			get
			{
				return mItems[index];
			}
			set
			{
				if (value == null) throw new ArgumentNullException("value");
				if (mItems[index] == value) return;

				ValidateItem(value);

				mItems[index].Parent = null;
				OnWidgetRemoved(mItems[index]);

				value.Parent = mParent;
				mItems[index] = value;
				OnWidgetAdded(value);
			}
		}

		public void Add(Widget item)
		{
			if (item == null) throw new ArgumentNullException("item");

			ValidateItem(item);

			mItems.Add(item);
			item.Parent = mParent;

			OnWidgetAdded(item);
		}
		public void Add(params Widget[] items)
		{
			if (items.Length == 0)
				throw new ArgumentException("No items to add.");

			AddRange(items);
		}
		public void AddRange(IEnumerable<Widget> items)
		{
			if (items == null) throw new ArgumentNullException("item");

			foreach (var item in items)
			{
				Add(item);
			}
		}

		public void Clear()
		{
			foreach (var item in mItems)
			{
				item.Parent = null;
				OnWidgetRemoved(item);
			}

			mItems.Clear();
		}

		public bool Contains(Widget item)
		{
			return mItems.Contains(item);
		}

		public void CopyTo(Widget[] array, int arrayIndex)
		{
			mItems.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return mItems.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

	    /// <summary>
	    /// Removes all items that match the specified predicate
	    /// </summary>
	    /// <param name="pred"></param>
	    /// <returns></returns>
	    public int RemoveAll(Func<Widget, bool> pred)
	    {
	        var itemsToRemove = mItems.Where(pred).ToList();

	        return mItems.RemoveAll(itemsToRemove.Contains);
	    }

		public bool Remove(Widget item)
		{
			if (item == null) throw new ArgumentNullException("item");

			if (mItems.Contains(item))
			{
				item.Parent = null; 
				OnWidgetRemoved(item);
			}

			return mItems.Remove(item);
		}

		public IEnumerator<Widget> GetEnumerator()
		{
			return mItems.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public T Find<T>(string name) where T : Widget
		{
			T result = (T)mItems.SingleOrDefault(x => x.Name == name);

			if (result == null)
			{
				foreach (var child in this)
				{
					Container c = child as Container;

					if (c == null)
						continue;

					result = c.Children.Find<T>(name);

					if (result != null)
						return result;
				}
			}

			return result;
		}

		protected virtual void OnWidgetAdded(Widget widget)
		{
			widget.LayoutDirty = true;

			WidgetAdded?.Invoke(this, new WidgetEventArgs(widget));
		}
		protected virtual void OnWidgetRemoved(Widget widget)
		{
			mParent.LayoutDirty = true;

			WidgetRemoved?.Invoke(this, new WidgetEventArgs(widget));
		}
		public event EventHandler<WidgetEventArgs> WidgetAdded;
		public event EventHandler<WidgetEventArgs> WidgetRemoved;

		/// <summary>
		/// Returns the immediate child which occupies the specified client point.
		/// </summary>
		/// <param name="point">The point in client coordinates of this container to search for child widgets at.</param>
		/// <returns>The widget at the specified point, or null if nothing is found.</returns>
		/// <remarks>This method does not walk through the descendants of child controls, it will only return null or a member of this WidgetList.</remarks>
		public Widget WidgetAt(Point point)
		{
			foreach(var child in System.Linq.Enumerable.Reverse(mItems))
			{
				if (child.WidgetRect.Contains(point))
					return child;
			}

			return null;
		}

	}

	public class WidgetListOf<T> : WidgetList where T : Widget
	{
		public WidgetListOf(Container parent)
			: base(parent)
		{ }

		protected override void ValidateItem(Widget item)
		{
			if (item is T == false)
				throw new InvalidOperationException("Items added to this list must be of type " + typeof(T).Name);
		}

		public new T this[int index]
		{
			get { return (T)base[index]; }
			set { base[index] = value; }
		}
	}

	public class NewWidgetList<T> : IList<T> where T : Widget
	{
		List<T> mItems = new List<T>();

		public NewWidgetList()
		{
		}

		public int IndexOf(T item)
		{
			return mItems.IndexOf(item);
		}

		public void Sort(Comparison<T> comparison)
		{
			mItems.InsertionSort(comparison);
		}
		
		public void Insert(int index, T item)
		{
			if (item == null) throw new ArgumentNullException("item");
			if (Contains(item))
				throw new InvalidOperationException("item is already contained in this lsit!");
			if (item.Parent != null)
				throw new InvalidOperationException("item already has a parent!");

			mItems.Insert(index, item);

			OnWidgetAdded(item);
		}

		public void RemoveAt(int index)
		{
			mItems[index].Parent = null;
			OnWidgetRemoved(mItems[index]);
			mItems.RemoveAt(index);
		}

		public T this[int index]
		{
			get
			{
				return mItems[index];
			}
			set
			{
				if (value == null) throw new ArgumentNullException("value");
				if (mItems[index] == value) return;

				mItems[index].Parent = null;
				OnWidgetRemoved(mItems[index]);

				mItems[index] = value;
				OnWidgetAdded(value);
			}
		}

		public void Add(T item)
		{
			if (item == null) throw new ArgumentNullException("item");

			mItems.Add(item);

			OnWidgetAdded(item);
		}
		public void Add(params T[] items)
		{
			if (items.Length == 0)
				throw new ArgumentException("No items to add.");

			AddRange(items);
		}
		public void AddRange(IEnumerable<T> items)
		{
			if (items == null) throw new ArgumentNullException("item");

			foreach (var item in items)
			{
				Add(item);
			}
		}

		public void Clear()
		{
			foreach (var item in mItems)
			{
				item.Parent = null;
				OnWidgetRemoved(item);
			}

			mItems.Clear();
		}

		public bool Contains(T item)
		{
			return mItems.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			mItems.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return mItems.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		/// <summary>
		/// Removes all items that match the specified predicate
		/// </summary>
		/// <param name="pred"></param>
		/// <returns></returns>
		public int RemoveAll(Func<T, bool> pred)
		{
			var itemsToRemove = mItems.Where(pred).ToList();

			return mItems.RemoveAll(itemsToRemove.Contains);
		}

		public bool Remove(T item)
		{
			if (item == null) throw new ArgumentNullException("item");

			if (mItems.Contains(item))
			{
				item.Parent = null;
				OnWidgetRemoved(item);
			}

			return mItems.Remove(item);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return mItems.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public T Find(string name)
		{
			T result = mItems.SingleOrDefault(x => x.Name == name);

			if (result == null)
			{
				foreach (var child in this)
				{
					Container c = child as Container;

					if (c == null)
						continue;

					result = c.Children.Find<T>(name);

					if (result != null)
						return result;
				}
			}

			return result;
		}

		protected virtual void OnWidgetAdded(Widget widget)
		{
			widget.LayoutDirty = true;

			WidgetAdded?.Invoke(this, new WidgetEventArgs(widget));
		}

		protected virtual void OnWidgetRemoved(Widget widget)
		{
			WidgetRemoved?.Invoke(this, new WidgetEventArgs(widget));
		}

		public event EventHandler<WidgetEventArgs> WidgetAdded;
		public event EventHandler<WidgetEventArgs> WidgetRemoved;

		/// <summary>
		/// Returns the immediate child which occupies the specified client point.
		/// </summary>
		/// <param name="point">The point in client coordinates of this container to search for child widgets at.</param>
		/// <returns>The widget at the specified point, or null if nothing is found.</returns>
		/// <remarks>This method does not walk through the descendants of child controls, it will only return null or a member of this WidgetList.</remarks>
		public T WidgetAt(Point point)
		{
			foreach (var child in System.Linq.Enumerable.Reverse(mItems))
			{
				if (child.WidgetRect.Contains(point))
					return child;
			}

			return null;
		}

	}

}
