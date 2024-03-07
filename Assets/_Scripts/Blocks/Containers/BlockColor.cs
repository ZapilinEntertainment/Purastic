using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZE.ServiceLocator;

namespace ZE.Purastic {
	public enum BlockColor : int
	{
		DefaultWhite, Green_Baseplate, Lavender
	}
	public static class BlockColorExtension
	{
		public static Color32 ToColor32(this BlockColor color)
		{
			switch (color)
			{
				case BlockColor.Lavender: return new Color32(160,160,250,255);
				case BlockColor.Green_Baseplate: return new Color32(14,161,92,255);
				default: return new Color32(255,255,255,255);
			}
		}
	}
}
