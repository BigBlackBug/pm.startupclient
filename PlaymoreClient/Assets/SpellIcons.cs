using NotMissing.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace PlaymoreClient.Assets
{
	public class SpellIcons
	{
		private const string ImagePath = "Content\\Images\\";

		private const string SpellPath = "Content\\Images\\SpellIcons\\";

		private readonly static object _sync;

		private readonly static Dictionary<int, Bitmap> _cache;

		private static Bitmap _unknown;

		static SpellIcons()
		{
			SpellIcons._sync = new object();
			SpellIcons._cache = new Dictionary<int, Bitmap>();
			SpellIcons._unknown = SpellIcons.SafeBitmap("Content\\Images\\unknown.png");
		}

		public SpellIcons()
		{
		}

		private static void AddCached(int key, Bitmap bmp)
		{
			lock (SpellIcons._sync)
			{
				SpellIcons._cache[key] = bmp;
			}
		}

		private static Bitmap FindCached(int key)
		{
			Bitmap bitmap;
			Bitmap bitmap1;
			Bitmap bitmap2;
			lock (SpellIcons._sync)
			{
				if (SpellIcons._cache.TryGetValue(key, out bitmap))
				{
					bitmap2 = bitmap;
				}
				else
				{
					bitmap2 = null;
				}
				bitmap1 = bitmap2;
			}
			return bitmap1;
		}

		public static Bitmap Get(int key)
		{
			Bitmap bitmap = SpellIcons.FindCached(key);
			if (bitmap != null)
			{
				return bitmap;
			}
			bitmap = SpellIcons.SafeBitmap(string.Format("{0}{1}.png", "Content\\Images\\SpellIcons\\", key));
			SpellIcons.AddCached(key, bitmap);
			return bitmap ?? SpellIcons._unknown;
		}

		private static Bitmap SafeBitmap(string file)
		{
			Bitmap bitmap;
			Bitmap bitmap1;
			try
			{
				if (File.Exists(file))
				{
					bitmap1 = new Bitmap(file);
				}
				else
				{
					bitmap1 = null;
				}
				bitmap = bitmap1;
			}
			catch (Exception exception)
			{
				PlaymoreClient.Gui.MainForm.LOGGER.Info(exception);
				bitmap = null;
			}
			return bitmap;
		}
	}
}