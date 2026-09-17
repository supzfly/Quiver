using System.IO;
using System.Reflection;
using UnityEngine;

namespace Quiver
{
	class QUtility
	{

		public static string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "QuiverAssets");
		//public static string AssetsPath = Path.Combine(path, "QuiverAssets");
		public static Texture2D LoadTextureFromAssets(string name)
		{
			

			Texture2D result;
			try
			{
				byte[] array = File.ReadAllBytes(Path.Combine(QUtility.path, "QuiverAssets", name));
				Texture2D texture2D = new Texture2D(1, 1);
				ImageConversion.LoadImage(texture2D, array);
				result = texture2D;
			}
			catch
			{
				byte[] array2 = File.ReadAllBytes(Path.Combine(QUtility.path, name));
				Texture2D texture2D2 = new Texture2D(1, 1);
				ImageConversion.LoadImage(texture2D2, array2);
				result = texture2D2;
			}
			return result;
		}
	}
}
