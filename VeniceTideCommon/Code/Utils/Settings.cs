using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;

namespace VeniceTideCommon.Code.Utils
{
	public class Settings
	{
		public enum SettingKey
		{
			LastDataFetch,
			Data,
			FavoriteLocations
		}

		static private Dictionary<SettingKey, object> defaultValues = new Dictionary<SettingKey, object> { 
			{ SettingKey.LastDataFetch, DateTime.Now.Subtract(TimeSpan.FromDays(14)) },
			{ SettingKey.Data, null },
			{ SettingKey.FavoriteLocations, null },
		};

		static private IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;

		static public object Get(SettingKey key)
		{
			if (defaultValues.ContainsKey(key))
			{
				return Get(key, defaultValues[key]);
			}
			else
			{
				throw new ArgumentException("Default value for key " + key + " is not defined in Settings.", "key");
			}
		}

		static public object Get(SettingKey key, object defaultValue)
		{
			if (settings.Contains(key.ToString()))
			{
				return settings[key.ToString()];
			}
			else
			{
				return defaultValue;
			}
		}

		static public void Set(SettingKey key, object value)
		{
			if (settings.Contains(key.ToString()))
			{
				settings[key.ToString()] = value;
			}
			else
			{
				settings.Add(key.ToString(), value);
			}
		}

		static public void Remove(SettingKey key)
		{
			if (settings.Contains(key.ToString()))
			{
				settings.Remove(key.ToString());
			}
		}

		static public bool Exists(SettingKey key)
		{
			return settings.Contains(key.ToString());
		}

		static public void Save()
		{
			settings.Save();
		}
	}
}
