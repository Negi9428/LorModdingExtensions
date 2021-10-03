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

	public class ModGiftData
	{
		
		public string GiftId { get; }

		public string WorkshopId { get; }
		
		public string FrontSprite { get; }
		
		public string FrontBackSprite { get; }
		
		public string SideSprite { get; }
		
		public string SideBackSprite { get; }

		public ModGiftData(string giftId, string workshopId, string frontSprite, string frontBackSprite, string sideSprite, string sideBackSprite)
		{
			GiftId = giftId;
			WorkshopId = workshopId;
			FrontSprite = frontSprite;
			FrontBackSprite = frontBackSprite;
			SideSprite = sideSprite;
			SideBackSprite = sideBackSprite;
		}

	}

}