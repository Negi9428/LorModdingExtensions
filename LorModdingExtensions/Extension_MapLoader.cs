using System;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LorModdingExtensions
{

	public static class Extension_MapLoader
	{

		public static void ChangeToEgoMap(EgoMapData mapData, string workshopId)
		{
			FieldInfo mapChanged = StageController.Instance.GetType().GetField("_mapChanged", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance);
			if ((bool)mapChanged.GetValue(StageController.Instance)) return;
			mapChanged.SetValue(StageController.Instance, true);
			
			Type type = BattleSceneRoot.Instance.GetType();
			FieldInfo _mapChangeFilter = type.GetField("_mapChangeFilter", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance);
			MapChangeFilter filter = (MapChangeFilter)_mapChangeFilter.GetValue(BattleSceneRoot.Instance);
			filter.StartMapChangingEffect(Direction.RIGHT);
			
			GameObject gameObject = Util.LoadPrefab("InvitationMaps/InvitationMap_Circus", SingletonBehavior<BattleSceneRoot>.Instance.transform); 
			
			

			GameObject timer = new GameObject();
			StageController.Instance.RegisterStartBattleEffect(timer.AddComponent<TimerObject>().effect);
			timer.AddComponent<AutoDestruct>().time = 1.0f;

			foreach (Component componentsInChild in gameObject.GetComponentsInChildren<Component>())
			{
				if (componentsInChild is SpriteRenderer && componentsInChild.name == "BG")
				{
					Sprite bgSprite = Extension.LoadModSprite(workshopId, mapData.BackGround);
					(componentsInChild as SpriteRenderer).sprite = bgSprite;
					if (bgSprite == null) continue;
					float ratio = 1920 / bgSprite.rect.width;
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

			MapManager mapManager = gameObject.GetComponent<MapManager>();
			MapManager newMap = gameObject.AddComponent<MapManager>();
			Type type3 = newMap.GetType();
			FieldInfo[] fields = type3.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			foreach (FieldInfo field in fields)
			{
				field.SetValue(newMap, field.GetValue(mapManager));
			}
			Object.Destroy(mapManager);
			newMap.isBossPhase = false;
			newMap.isEgo = true;

			if (BattleSceneRoot.Instance.currentMapObject.isCreature)
				Object.Destroy(BattleSceneRoot.Instance.currentMapObject.gameObject);
			else
				BattleSceneRoot.Instance.currentMapObject.EnableMap(false);
			if (newMap != null)
			{
				BattleSceneRoot.Instance.currentMapObject = newMap;
				BattleSceneRoot.Instance.currentMapObject.ActiveMap(true);
				BattleSceneRoot.Instance.currentMapObject.InitializeMap();
			}
		}

	}

}