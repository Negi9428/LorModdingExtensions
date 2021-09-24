namespace LorModdingExtensions
{

	public class MapData
	{
		/// <summary>
		/// MAP名　MAPを呼び出す時もこの名称を使う　重複不可
		/// </summary>
		public string MapName { get; }

		/// <summary>
		/// BGMのファイル名　StoryBgm内に配置
		/// </summary>
		public string BgmName { get; }
		
		/// <summary>
		/// 背景画像のファイル名　StoryBgSprite内に配置
		/// </summary>
		public string BackGround { get; }
		
		/// <summary>
		/// 床画像のファイル名　StoryBgSprite内に配置
		/// </summary>
		public string Floor { get; }
		
		public MapData(string mapName, string bgmName, string backGround, string floor)
		{
			Floor = floor;
			BackGround = backGround;
			BgmName = bgmName;
			MapName = mapName;
		}
		
	}
	
	public class EgoMapData
	{
		
		/// <summary>
		/// 背景画像のファイル名　StoryBgSprite内に配置
		/// </summary>
		public string BackGround { get; }
		
		/// <summary>
		/// 床画像のファイル名　StoryBgSprite内に配置
		/// </summary>
		public string Floor { get; }
		
		public EgoMapData(string backGround, string floor)
		{
			Floor = floor;
			BackGround = backGround;
		}
		
	}

}