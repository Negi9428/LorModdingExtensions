using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Mod;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LorModdingExtensions
{

	public class EnemyTeamStageManager_ModdingBase : EnemyTeamStageManager
	{
		/// <summary>
		/// MAP情報配列　override必須
		/// </summary>
		protected virtual MapData[] mapDataAlly => new MapData[] { };
		
		/// <summary>
		/// workshopId override必須
		/// </summary>
		protected virtual string workshopId => "";
		
		private readonly Dictionary<string, MapData> mapDictionary = new Dictionary<string, MapData>();
		private readonly Dictionary<string, AudioClip> bgm = new Dictionary<string, AudioClip>();

		private static readonly string bgmDir = "Resource\\StoryBgm";

		/// <summary>
		/// MAP初期化処理が走る
		/// override時もBaseを必ず呼ぶこと
		/// </summary>
		public override void OnWaveStart()
		{
			base.OnWaveStart();
			Initialize();
		}

		/// <summary>
		/// MAP初期化処理
		/// 設定されているBGM毎に分類しBGMを読み込む
		/// MAP名が重複している場合は最後のもののみ有効
		/// </summary>
		private void Initialize()
		{
			foreach (MapData data in mapDataAlly)
			{
				mapDictionary.Add(data.MapName, data);
			}

			List<string> bgmList = new List<string>();
			foreach (KeyValuePair<string, MapData> pair in mapDictionary.Where(pair => !bgmList.Contains(pair.Value.BgmName)))
			{
				bgmList.Add(pair.Value.BgmName);
			}
			
			foreach (string name in bgmList)
			{
				LoadAudioClip(name);
			}
		}

		/// <summary>
		/// BGMを読み込み、そのBGMに対応しているMAPを読み込ませる
		/// BGMの読み込みが一番遅いのでBGMの読み込み終了を待つ必要がある
		/// </summary>
		/// <param name="bgmName"></param>
		private void LoadAudioClip(string bgmName)
		{
			string filePath = Path.Combine(bgmDir, bgmName);
			string modPath = ModContentManager.Instance.GetModPath(workshopId);
			if (!File.Exists(Path.Combine(modPath, filePath))) return;
			CoroutineHandler.StartStaticCoroutine(AudioUtil.LoadAudioCoroutine(Path.Combine(modPath, filePath), clip =>
			{
				bgm.Add(bgmName, clip);
				foreach (KeyValuePair<string,MapData> keyValuePair in mapDictionary.Where(pair => pair.Value.BgmName == bgmName))
				{
					LoadMap(keyValuePair.Value);
				}
			}));
		}

		private const int screenX = 1920;

		/// <summary>
		/// MAP情報生成
		/// 既存MAP（Circus）のPrefabを読み込みそれを上書きする形で新しいMapManagerを作成する
		/// </summary>
		/// <param name="mapData"></param>
		private void LoadMap(MapData mapData)
		{
			GameObject gameObject = Util.LoadPrefab("InvitationMaps/InvitationMap_Circus", SingletonBehavior<BattleSceneRoot>.Instance.transform);
			MapManager mapManager = gameObject.GetComponent<MapManager>();
			mapManager.name = mapData.MapName;
			mapManager.mapBgm[0] = bgm[mapData.BgmName];
			
			foreach (Component componentsInChild in gameObject.GetComponentsInChildren<Component>())
			{
				if (componentsInChild is SpriteRenderer && componentsInChild.name == "BG")
				{
					Sprite bgSprite = Extension.LoadModSprite(workshopId, mapData.BackGround);
					(componentsInChild as SpriteRenderer).sprite = bgSprite;
					if (bgSprite == null) continue;
					float ratio = screenX / bgSprite.rect.width; 
					componentsInChild.gameObject.transform.localScale = new Vector3(ratio, ratio, ratio);
				}
				else if (componentsInChild is SpriteRenderer && componentsInChild.name == "Floor")
				{
					Sprite floorSprite = Extension.LoadModSprite(workshopId, mapData.Floor);
					(componentsInChild as SpriteRenderer).sprite = floorSprite;
					componentsInChild.gameObject.transform.localScale = new Vector3(8, 24, 8);
				}
				else if (componentsInChild.name != gameObject.name && !componentsInChild.name.Contains("BackgroundRoot") && (!componentsInChild.name.Contains("Frame") && componentsInChild.name != "Camera") && (componentsInChild.name != "BG" && componentsInChild.name != "Floor"))
					componentsInChild.gameObject.SetActive(false);
			}

			MapManager newMap = gameObject.AddComponent<MapManager>();
			Type type = newMap.GetType();
			FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			foreach (FieldInfo field in fields)
			{
				field.SetValue(newMap, field.GetValue(mapManager));
			}
			Object.Destroy(mapManager);

			BattleSceneRoot.Instance.InitInvitationMap(newMap);
		}

	}

}