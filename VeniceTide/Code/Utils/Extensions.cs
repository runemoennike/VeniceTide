using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace VeniceTide.Code.Utils
{
	static class Extensions
	{
		public static void Sort<T>(this ObservableCollection<T> collection) where T : IComparable
		{
			List<T> sorted = collection.OrderBy(x => x).ToList();
			collection.Clear();
			for (int i = 0; i < sorted.Count(); i++)
				collection.Add(sorted[i]);
		}

		public static List<T> GetChildObjects<T>(this DependencyObject obj, string name = null)
		{
			var retVal = new List<T>();
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
			{
				object c = VisualTreeHelper.GetChild(obj, i);
				if (c.GetType().FullName == typeof(T).FullName && (String.IsNullOrEmpty(name) || ((FrameworkElement)c).Name == name))
				{
					retVal.Add((T)c);
				}
				var gc = ((DependencyObject)c).GetChildObjects<T>(name);
				if (gc != null)
					retVal.AddRange(gc);
			}

			return retVal;
		}

		public static T GetChildObject<T>(this DependencyObject obj, string name = null) where T : DependencyObject
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
			{
				object c = VisualTreeHelper.GetChild(obj, i);
				if (c.GetType().FullName == typeof(T).FullName && (String.IsNullOrEmpty(name) || ((FrameworkElement)c).Name == name))
				{
					return (T)c;
				}
				object gc = ((DependencyObject)c).GetChildObject<T>(name);
				if (gc != null)
					return (T)gc;
			}

			return null;
		}
	}

	public static class VisualTreeEnumeration
	{
		public static IEnumerable<DependencyObject> Descendents(this DependencyObject root, int depth)
		{
			int count = VisualTreeHelper.GetChildrenCount(root);
			for (int i = 0; i < count; i++)
			{
				var child = VisualTreeHelper.GetChild(root, i);
				yield return child;
				if (depth > 0)
				{
					foreach (var descendent in Descendents(child, --depth))
						yield return descendent;
				}
			}
		}

		public static IEnumerable<DependencyObject> Descendents(this DependencyObject root)
		{
			return Descendents(root, Int32.MaxValue);
		}

		public static IEnumerable<DependencyObject> Ancestors(this DependencyObject root)
		{
			DependencyObject current = VisualTreeHelper.GetParent(root);
			while (current != null)
			{
				yield return current;
				current = VisualTreeHelper.GetParent(current);
			}
		}
	}
}
