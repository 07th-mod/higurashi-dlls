using Assets.Scripts.Core.Buriko;
using Assets.Scripts.UI.Tips;
using MOD.Scripts.Core.Config;
using MOD.Scripts.Core.Files;
using MOD.Scripts.Core.Scene;
using MOD.Scripts.Core.TextWindow;
using MOD.Scripts.UI;
using System.Collections.Generic;
using System.Linq;

namespace MOD.Scripts.Core
{
	public class MODSystem
	{
		public readonly MODMainUIController modMainUIController = new MODMainUIController();

		public readonly MODSceneController modSceneController = new MODSceneController();

		public readonly MODTextController modTextController = new MODTextController();

		public readonly MODTextureController modTextureController;

		private static MODTextureController fixedMODTextureControllerInstance;

		public readonly MODConfig modConfig;

		private static MODConfig fixedMODConfigInstance;

		private static bool initialized;

		private static Dictionary<int, List<TipsDataEntry>> fixedTips;

		public static MODSystem instance
		{
			get
			{
				Initialize();
				return new MODSystem();
			}
		}

		public List<TipsDataEntry> Tips
		{
			get
			{
				if (fixedTips.Any())
				{
					fixedTips.TryGetValue(BurikoMemory.Instance.GetFlag("GArc").IntValue(), out List<TipsDataEntry> value);
					return value ?? new List<TipsDataEntry>();
				}
				return TipsData.Tips;
			}
		}

		public MODSystem()
		{
			modTextureController = fixedMODTextureControllerInstance;
			modConfig = fixedMODConfigInstance;
		}

		static MODSystem()
		{
		}

		public static void Initialize()
		{
			if (!initialized)
			{
				initialized = true;
				fixedMODTextureControllerInstance = new MODTextureController();
				fixedMODConfigInstance = MODFileManager.ReadConfig();
				fixedTips = MODFileManager.ReadTips2();
			}
		}
	}
}
