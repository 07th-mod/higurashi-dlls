using Assets.Scripts.UI.Tips;
using MOD.Scripts.Core.Config;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace MOD.Scripts.Core.Files
{
	public static class MODFileManager
	{
		private static readonly JsonSerializerSettings Settings;

		private static readonly string BaseDirectory;

		private static readonly string ConfigFileName;

		private static readonly string TipsFileName;

		public static MODConfig ReadConfig()
		{
			return ReadObject<MODConfig>(ConfigFileName);
		}

		static MODFileManager()
		{
			Settings = new JsonSerializerSettings
			{
				DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate
			};
			BaseDirectory = Application.dataPath;
			ConfigFileName = "nipah";
			TipsFileName = "tips";
		}

		public static List<TipsDataEntry> ReadTips()
		{
			return ReadArray<TipsDataEntry>(TipsFileName);
		}

		private static T ReadObject<T>(string filename)
		{
			return ReadFile<T>(filename, "{}");
		}

		private static List<T> ReadArray<T>(string filename)
		{
			return ReadFile<List<T>>(filename, "[]");
		}

		private static T ReadFile<T>(string filename, string defaultJson)
		{
			string path = Path.Combine(BaseDirectory, filename + ".json");
			TextReader reader = (!File.Exists(path)) ? ((TextReader)new StringReader(defaultJson)) : ((TextReader)new StreamReader(path));
			JsonSerializer jsonSerializer = JsonSerializer.Create(Settings);
			using (JsonTextReader reader2 = new JsonTextReader(reader))
			{
				return jsonSerializer.Deserialize<T>(reader2);
			}
		}

		public static Dictionary<int, List<TipsDataEntry>> ReadTips2()
		{
			return ReadObject<Dictionary<int, List<TipsDataEntry>>>(TipsFileName);
		}
	}
}
