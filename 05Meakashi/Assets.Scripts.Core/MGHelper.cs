using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Core
{
	public static class MGHelper
	{
		public delegate bool InputHandler();

		private static readonly byte[] Key = new byte[9]
		{
			229,
			99,
			174,
			4,
			45,
			166,
			127,
			158,
			69
		};

		private static string _savepath = string.Empty;

		private static bool isD3d9;

		private static bool typeCheck;

		public static Mesh CreateMeshWithOrigin(int width, int height, Vector2 origin)
		{
			Mesh mesh = new Mesh();
			float num = Mathf.Round((float)width / 2f);
			float num2 = Mathf.Round((float)height / 2f);
			origin.x -= num;
			origin.y -= num2;
			Vector3 vector = new Vector3(0f - num - origin.x, num2 + origin.y, 0f);
			Vector3 vector2 = new Vector3(num - origin.x, num2 + origin.y, 0f);
			Vector3 vector3 = new Vector3(num - origin.x, 0f - num2 + origin.y, 0f);
			Vector3 vector4 = new Vector3(0f - num - origin.x, 0f - num2 + origin.y, 0f);
			Vector3[] vertices = new Vector3[4]
			{
				vector2,
				vector3,
				vector,
				vector4
			};
			Vector2[] uv = new Vector2[4]
			{
				new Vector2(1f, 1f),
				new Vector2(1f, 0f),
				new Vector2(0f, 1f),
				new Vector2(0f, 0f)
			};
			int[] triangles = new int[6]
			{
				0,
				1,
				2,
				2,
				1,
				3
			};
			mesh.vertices = vertices;
			mesh.uv = uv;
			mesh.triangles = triangles;
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			return mesh;
		}

		public static Mesh CreateMesh(int width, int height, LayerAlignment alignment)
		{
			Mesh mesh = new Mesh();
			float num = Mathf.Round((float)width / 2f);
			float num2 = Mathf.Round((float)height / 2f);
			Vector3 vector;
			Vector3 vector2;
			Vector3 vector3;
			Vector3 vector4;
			switch (alignment)
			{
			case LayerAlignment.AlignTopleft:
				vector = new Vector3(0f, 0f, 0f);
				vector2 = new Vector3((float)width, 0f, 0f);
				vector3 = new Vector3((float)width, (float)(-height), 0f);
				vector4 = new Vector3(0f, (float)(-height), 0f);
				break;
			case LayerAlignment.AlignBottomCenter:
				vector = new Vector3(0f - num, (float)height, 0f);
				vector2 = new Vector3(num, (float)height, 0f);
				vector3 = new Vector3(num, 0f, 0f);
				vector4 = new Vector3(0f - num, 0f, 0f);
				break;
			case LayerAlignment.AlignCenter:
				vector = new Vector3(0f - num, num2, 0f);
				vector2 = new Vector3(num, num2, 0f);
				vector3 = new Vector3(num, 0f - num2, 0f);
				vector4 = new Vector3(0f - num, 0f - num2, 0f);
				break;
			default:
				Logger.LogError("Could not CreateMesh, unexpected alignment " + alignment);
				return null;
			}
			Vector3[] vertices = new Vector3[4]
			{
				vector2,
				vector3,
				vector,
				vector4
			};
			Vector2[] uv = new Vector2[4]
			{
				new Vector2(1f, 1f),
				new Vector2(1f, 0f),
				new Vector2(0f, 1f),
				new Vector2(0f, 0f)
			};
			int[] triangles = new int[6]
			{
				0,
				1,
				2,
				2,
				1,
				3
			};
			mesh.vertices = vertices;
			mesh.uv = uv;
			mesh.triangles = triangles;
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			return mesh;
		}

		public static void KeyEncode(byte[] b)
		{
			byte[] array = (byte[])Key.Clone();
			for (int i = 0; i < b.Length; i++)
			{
				b[i] = (byte)(b[i] ^ array[i % Key.Length]);
				array[i % array.Length] += 27;
			}
		}

		public static void WriteVector3(BinaryWriter br, Vector3 v)
		{
			br.Write(v.x);
			br.Write(v.y);
			br.Write(v.z);
		}

		public static Vector3 ReadVector3(BinaryReader br)
		{
			Vector3 result = default(Vector3);
			result.x = br.ReadSingle();
			result.y = br.ReadSingle();
			result.z = br.ReadSingle();
			return result;
		}

		public static void WriteColor(BinaryWriter br, Color c)
		{
			br.Write(c.r);
			br.Write(c.g);
			br.Write(c.b);
			br.Write(c.a);
		}

		public static Color ReadColor(BinaryReader br)
		{
			Color result = default(Color);
			result.r = br.ReadSingle();
			result.g = br.ReadSingle();
			result.b = br.ReadSingle();
			result.a = br.ReadSingle();
			return result;
		}

		public static string GetSavePath()
		{
			if (Application.platform == RuntimePlatform.OSXPlayer)
			{
				return Application.persistentDataPath;
			}
			if (Application.platform == RuntimePlatform.LinuxPlayer)
			{
				return Application.persistentDataPath;
			}
			if (_savepath == string.Empty)
			{
				string text = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
				if (!Directory.Exists(text) || text == string.Empty)
				{
					text = Environment.ExpandEnvironmentVariables("%appdata%");
				}
				_savepath = Path.Combine(text, "Mangagamer\\higurashi05");
				Directory.CreateDirectory(_savepath);
			}
			return _savepath;
		}

		public static string GetDataPath()
		{
			if (Application.platform == RuntimePlatform.OSXPlayer)
			{
				return "Resources/GameData";
			}
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				return "ExternalAssets/Archives";
			}
			return "GameData";
		}

		public static Vector3 GetReverseOffsetPosition(Vector3 pos)
		{
			return pos;
		}

		public static Vector3 GetOffsetPosition(Vector3 pos)
		{
			return pos;
		}

		public static int GetLoopPoint(string bgm)
		{
			string text = bgm;
			if (Path.HasExtension(bgm))
			{
				text = Path.GetFileNameWithoutExtension(bgm);
			}
			if (text != null)
			{
				if (_003C_003Ef__switch_0024map5 == null)
				{
					Dictionary<string, int> dictionary = new Dictionary<string, int>(100);
					dictionary.Add("bgm_dd_011", 0);
					dictionary.Add("bgm_dd_020", 1);
					dictionary.Add("bgm_dd_030", 2);
					dictionary.Add("bgm_dd_040", 3);
					dictionary.Add("bgm_dd_050", 4);
					dictionary.Add("bgm_dd_060", 5);
					dictionary.Add("bgm_dd_061", 6);
					dictionary.Add("bgm_dd_070", 7);
					dictionary.Add("bgm_dd_080", 8);
					dictionary.Add("bgm_dd_081", 9);
					dictionary.Add("bgm_dd_090", 10);
					dictionary.Add("bgm_dd_100", 11);
					dictionary.Add("bgm_dd_130", 12);
					dictionary.Add("bgm_dd_160", 13);
					dictionary.Add("bgm_dd_170", 14);
					dictionary.Add("bgm_dd_190", 15);
					dictionary.Add("bgm_dd_200", 16);
					dictionary.Add("bgm_kk_010", 17);
					dictionary.Add("bgm_kk_020", 18);
					dictionary.Add("bgm_kk_021", 19);
					dictionary.Add("bgm_kk_030", 20);
					dictionary.Add("bgm_kk_040", 21);
					dictionary.Add("bgm_kk_050", 22);
					dictionary.Add("bgm_kk_070", 23);
					dictionary.Add("bgm_kk_080", 24);
					dictionary.Add("bgm_kk_090", 25);
					dictionary.Add("bgm_kk_091", 26);
					dictionary.Add("bgm_kk_110", 27);
					dictionary.Add("bgm_kk_120", 28);
					dictionary.Add("bgm_kk_130", 29);
					dictionary.Add("bgm_kk_140", 30);
					dictionary.Add("bgm_kk_210", 31);
					dictionary.Add("bgm_kk_220", 32);
					dictionary.Add("bgm_kk_240", 33);
					dictionary.Add("bgm_kk_250", 34);
					dictionary.Add("bgm_kk_260", 35);
					dictionary.Add("bgm_kk_280", 36);
					dictionary.Add("bgm_sys_dd_01", 37);
					dictionary.Add("bgm_sys_dd_02", 38);
					dictionary.Add("bgm_sys_dd_03", 39);
					dictionary.Add("bgm_sys_dd_03_2", 40);
					dictionary.Add("bgm_sys_dd_04", 41);
					dictionary.Add("bgm_sys_dd_05", 42);
					dictionary.Add("bgm_sys_dded_01", 43);
					dictionary.Add("bgm_sys_dded_02", 44);
					dictionary.Add("bgm_sys_dded_03", 45);
					dictionary.Add("bgm_sys_dded_04", 46);
					dictionary.Add("bgm_sys_kk_01", 47);
					dictionary.Add("bgm_sys_kk_02", 48);
					dictionary.Add("bgm_sys_kk_03", 49);
					dictionary.Add("bgm_sys_kk_04", 50);
					dictionary.Add("bgm_sys_kk_05", 51);
					dictionary.Add("bgm_sys_kked_01", 52);
					dictionary.Add("bgm_sys_kked_02", 53);
					dictionary.Add("bgm_sys_kked_03", 54);
					dictionary.Add("bgm_sys_kked_04", 55);
					dictionary.Add("bgm_title", 56);
					dictionary.Add("bsel0051", 57);
					dictionary.Add("happycrossing", 58);
					dictionary.Add("kankyo_25_d", 59);
					dictionary.Add("se1011", 60);
					dictionary.Add("se1015_d", 61);
					dictionary.Add("se2020_d", 62);
					dictionary.Add("se2110_d", 63);
					dictionary.Add("se2111_d", 64);
					dictionary.Add("sel0120_k", 65);
					dictionary.Add("sel2040_d", 66);
					dictionary.Add("sel2041", 67);
					dictionary.Add("sel3010_d", 68);
					dictionary.Add("sel_0120_d", 69);
					dictionary.Add("sel_0220_d", 70);
					dictionary.Add("sel_4050_d", 71);
					dictionary.Add("sel_ashi_11_s", 72);
					dictionary.Add("sel_dd_0010", 73);
					dictionary.Add("sel_dd_0021", 74);
					dictionary.Add("sel_dd_0030", 75);
					dictionary.Add("sel_dd_0033", 76);
					dictionary.Add("sel_dd_0035", 77);
					dictionary.Add("sel_dd_0240", 78);
					dictionary.Add("sel_dd_0245", 79);
					dictionary.Add("sel_dd_1010", 80);
					dictionary.Add("sel_dd_1021", 81);
					dictionary.Add("sel_dd_2030", 82);
					dictionary.Add("sel_dd_2700", 83);
					dictionary.Add("sel_dvd_0080", 84);
					dictionary.Add("sel_dvd_0100_02", 85);
					dictionary.Add("sel_dvd_0110", 86);
					dictionary.Add("sel_dvd_0120", 87);
					dictionary.Add("sel_dvd_0160", 88);
					dictionary.Add("sel_dvd_0170", 89);
					dictionary.Add("sel_dvd_0180", 90);
					dictionary.Add("sel_dvd_0190", 91);
					dictionary.Add("sel_dvd_0250", 92);
					dictionary.Add("sel_kk_0090", 93);
					dictionary.Add("sel_ot_21_0010", 94);
					dictionary.Add("sel_ot_21_0030", 95);
					dictionary.Add("sel_ot_26_0010", 96);
					dictionary.Add("selp_dd_0050", 97);
					dictionary.Add("selp_dd_0060", 98);
					dictionary.Add("selp_dd_0070", 99);
					_003C_003Ef__switch_0024map5 = dictionary;
				}
				if (_003C_003Ef__switch_0024map5.TryGetValue(text, out int value))
				{
					switch (value)
					{
					case 0:
						return 1152298;
					case 1:
						return 194763;
					case 2:
						return 1637632;
					case 3:
						return 782606;
					case 4:
						return 287424;
					case 5:
						return 1360950;
					case 6:
						return 1988958;
					case 7:
						return 673727;
					case 8:
						return 314858;
					case 9:
						return 126326;
					case 10:
						return 1849827;
					case 11:
						return 267264;
					case 12:
						return 243138;
					case 13:
						return 0;
					case 14:
						return 0;
					case 15:
						return 724;
					case 16:
						return 1210327;
					case 17:
						return 1615391;
					case 18:
						return 2118652;
					case 19:
						return 188346;
					case 20:
						return 1442255;
					case 21:
						return 644216;
					case 22:
						return 4498436;
					case 23:
						return 269311;
					case 24:
						return 0;
					case 25:
						return 175389;
					case 26:
						return 104493;
					case 27:
						return 151411;
					case 28:
						return 555457;
					case 29:
						return 251693;
					case 30:
						return 826890;
					case 31:
						return 3611530;
					case 32:
						return 2618304;
					case 33:
						return 4814598;
					case 34:
						return 376703;
					case 35:
						return 366437;
					case 36:
						return 1144466;
					case 37:
						return 2429215;
					case 38:
						return 2508024;
					case 39:
						return 665967;
					case 40:
						return 1163094;
					case 41:
						return 3040630;
					case 42:
						return 787193;
					case 43:
						return 1177981;
					case 44:
						return 1225806;
					case 45:
						return 4014307;
					case 46:
						return 709553;
					case 47:
						return 149919;
					case 48:
						return 418378;
					case 49:
						return 4174863;
					case 50:
						return 655403;
					case 51:
						return 1819877;
					case 52:
						return 364179;
					case 53:
						return 2429792;
					case 54:
						return 1893150;
					case 55:
						return 508168;
					case 56:
						return 4905424;
					case 57:
						return 3053760;
					case 58:
						return 5844753;
					case 59:
						return 0;
					case 60:
						return 0;
					case 61:
						return 0;
					case 62:
						return 0;
					case 63:
						return 0;
					case 64:
						return 0;
					case 65:
						return 0;
					case 66:
						return 0;
					case 67:
						return 0;
					case 68:
						return 0;
					case 69:
						return 0;
					case 70:
						return 0;
					case 71:
						return 0;
					case 72:
						return 0;
					case 73:
						return 0;
					case 74:
						return 0;
					case 75:
						return 0;
					case 76:
						return 0;
					case 77:
						return 0;
					case 78:
						return 0;
					case 79:
						return 0;
					case 80:
						return 0;
					case 81:
						return 0;
					case 82:
						return 0;
					case 83:
						return 0;
					case 84:
						return 0;
					case 85:
						return 0;
					case 86:
						return 0;
					case 87:
						return 0;
					case 88:
						return 0;
					case 89:
						return 0;
					case 90:
						return 0;
					case 91:
						return 0;
					case 92:
						return 0;
					case 93:
						return 0;
					case 94:
						return 0;
					case 95:
						return 0;
					case 96:
						return 0;
					case 97:
						return 88692;
					case 98:
						return 417357;
					case 99:
						return 71684;
					}
				}
			}
			return 0;
		}
	}
}
