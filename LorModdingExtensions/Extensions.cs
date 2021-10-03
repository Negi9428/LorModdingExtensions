using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using LOR_XML;
using Mod;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace LorModdingExtensions
{

	public static class Extension
	{

		/// <summary>
		/// 新しく定義するバフの名前・説明文を既存のシステムにねじ込む
		/// 定義するバフのInitから呼び出す事で機能する
		/// 全角スペースは表示がバグるので注意
		/// アイコン画像はpng/jpgをStoryBgSpriteに配置する
		/// </summary>
		/// <param name="thisXmlList"></param>
		/// <param name="id">バフのID（任意文字列）</param>
		/// <param name="name">名称</param>
		/// <param name="desc">説明文　{0}はstackで置換される</param>
		/// <param name="workshopId">このModのworkshopID</param>
		/// <param name="iconName">アイコンの画像名</param>
		public static void AddEffectTextAndIcon(this BattleEffectTextsXmlList thisXmlList, string id, string name, string desc, string workshopId, string iconName)
		{
			if (BattleUnitBuf._bufIconDictionary.Count == 0)
			{
				new BattleUnitBuf().GetBufIcon();
			}
			if (BattleUnitBuf._bufIconDictionary.ContainsKey(id)) return;
			Sprite sprite = LoadModSprite(workshopId, iconName);
			BattleUnitBuf._bufIconDictionary[id] = sprite;
			
			thisXmlList.AddEffectText(id, name, desc);
		}
		
		
		/// <summary>
		/// 新しく定義するバフの名前・説明文を既存のシステムにねじ込む
		/// 定義するバフのInitから呼び出す事で機能する
		/// 全角スペースは表示がバグるので注意
		/// 既存のアイコンを使用する場合用（バフ側で別途設定）
		/// </summary>
		/// <param name="thisXmlList"></param>
		/// <param name="id">バフのID（任意文字列）</param>
		/// <param name="name">名称</param>
		/// <param name="desc">説明文　{0}はstackで置換される</param>
		public static void AddEffectText(this BattleEffectTextsXmlList thisXmlList, string id, string name, string desc)
		{
			Type type = thisXmlList.GetType();
			FieldInfo dic = type.GetField("_dictionary", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance);
			if (dic == null) return;
			((Dictionary<string, BattleEffectText>)(dic.GetValue(BattleEffectTextsXmlList.Instance)))[id] = new BattleEffectText()
			{
				ID = id,
				Name = name,
				Desc = desc,
			};
		}

		/// <summary>
		/// 現在のマップ情報を上書きする
		/// OnRoundStartで呼び出す　敵側が優勢であればこのMAPに変化する
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="mapName"></param>
		public static void SetCurrentMapInfo(this StageModel instance, string mapName)
		{
			Type type = instance.GetType();
			FieldInfo currentMapInfo = type.GetField("_currentMapInfo", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance);
			if (currentMapInfo == null) return;
			currentMapInfo.SetValue(instance, mapName);
		}

		/// <summary>
		/// ModのSpriteの読み込みを行う
		/// SpriteはStoryBgSprite内に配置する
		/// </summary>
		/// <param name="workshopId"></param>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public static Sprite LoadModSprite(string workshopId, string fileName)
		{
			return SpriteUtil.LoadSprite(Path.Combine(ModUtil.GetModBgSpritePath(ModContentManager.Instance.GetModPath(workshopId)), fileName), new Vector2(0.5f, 0.5f));
		}

		/// <summary>
		/// 画面上にテキストを表示する
		/// テキストは青い領域をクリックすると消える
		/// </summary>
		/// <param name="message">出力するテキスト</param>
		public static void DebugLog(string message)
		{
			GameObject baseObject = new GameObject();
			baseObject.transform.SetParent(BattleManagerUI.Instance.ui_unitListInfoSummary.transform);
			baseObject.transform.localScale = Vector3.one;

			Image baseImage = baseObject.AddComponent<Image>();
			Button baseButton = baseObject.AddComponent<Button>();
			baseImage.color = new Color(0, 0, 1, 0.5f);
			baseButton.image = baseImage;
			baseImage.GetComponent<RectTransform>().sizeDelta = new Vector2(1500, 800);
			baseButton.onClick.AddListener(() =>
			{
				Object.Destroy(baseObject);
			});
			
			GameObject textObject = Object.Instantiate(((TextMeshProUGUI)BattleManagerUI.Instance.ui_unitInformationPlayer.GetType().GetField("txt_lvTitle", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(BattleManagerUI.Instance.ui_unitInformationPlayer))?.gameObject, baseObject.transform);
			textObject.transform.localPosition = Vector3.zero;
			textObject.transform.localScale = Vector3.one;
			foreach (Component component in textObject.GetComponents(typeof(Component)))
			{
				if (component.GetType() != typeof(TextMeshPro) && component.GetType() != typeof(TextMeshProUGUI))
				{
					Object.Destroy(component);
				}
			}
			TextMeshProUGUI tmp = textObject.GetComponent<TextMeshProUGUI>();
			textObject.GetComponent<RectTransform>().sizeDelta = baseObject.GetComponent<RectTransform>().sizeDelta - new Vector2(200, 200);
			tmp.text = message;
			tmp.alignment = TextAlignmentOptions.MidlineLeft;
			tmp.enableWordWrapping = true;
			tmp.overflowMode = TextOverflowModes.Overflow;
			tmp.fontSize = 24;
		}
		
		/// <summary>
		/// 一時的なギフトを付与する
		/// アルリウネの月桂冠や罪善の茨の冠などのような表示が可能になる
		/// </summary>
		/// <param name="_instance"></param>
		/// <param name="modGiftData">表示する画像などの情報</param>
		/// <param name="position">Giftが付与される場所のタイプ　GiftPosition参照</param>
		/// <param name="forcedDisplay">強制表示フラグ　Trueであれば本来Giftを表示しない固定スキンにも強制的にギフトを追加する</param>
		/// <param name="refreshAppearance">表示更新フラグ　Trueでよい（defaultでTrue）</param>
		/// <returns></returns>
		public static GiftAppearance SetTemporaryGift(this CharacterAppearance _instance, ModGiftData modGiftData, GiftPosition position, bool forcedDisplay = false, bool refreshAppearance = true)
		{
			if (_instance.CustomAppearance == null && !forcedDisplay) return null;
			GiftAppearance giftData = _instance.CreateModGiftData(new GiftModel(Singleton<GiftXmlList>.Instance.CreateTemporaryGift("", position)), modGiftData);
			if (!refreshAppearance)
				return giftData;
			_instance.RefreshAppearanceByGifts();
			return giftData;
		}
		
		private static GiftAppearance CreateModGiftData(this CharacterAppearance _instance, GiftModel gift, ModGiftData modGiftData)
		{
			GameObject customizedAppearanceObject;
			if (_instance.CustomAppearance == null)
			{
				customizedAppearanceObject = new GameObject();
				DummyCustomizedAppearance customized = customizedAppearanceObject.AddComponent<DummyCustomizedAppearance>();
				customized.Initialize(_instance);
				_instance.GetType().GetField("_customAppearance", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(_instance, customized);
			}
			else
			{
				customizedAppearanceObject = _instance.CustomAppearance.gameObject;
			}
			
			Dictionary<GiftPosition, GiftAppearance> giftAppearanceDic = _instance.GiftAppearances;
			bool flag = false;
			GiftAppearance giftAppearance = null;
			if (giftAppearanceDic.ContainsKey(gift.ClassInfo.Position))
			{
				giftAppearance = giftAppearanceDic[gift.ClassInfo.Position];
				if (giftAppearance.ResourceName != modGiftData.GiftId)
				{
					giftAppearanceDic.Remove(gift.ClassInfo.Position);
					Object.Destroy(giftAppearance.gameObject);
					flag = true;
				}else if(!giftAppearance.isActiveAndEnabled)
				{
					giftAppearance.gameObject.SetActive(true);
				}
			}
			else
				flag = true;
			if (flag)
			{
				GameObject rootObject = new GameObject();
				Transform parent = _instance.CharacterMotions.ContainsKey(ActionDetail.Standing) ? _instance.CharacterMotions[ActionDetail.Standing].customPivot : _instance.CharacterMotions[ActionDetail.Default].customPivot;
				customizedAppearanceObject.transform.SetParent(parent);
				customizedAppearanceObject.transform.localPosition = Vector3.zero;
				customizedAppearanceObject.transform.localScale = Vector3.one;
				rootObject.transform.SetParent(customizedAppearanceObject.transform);
				giftAppearance = rootObject.AddComponent<GiftAppearance>();
				giftAppearance.SetResourceName(modGiftData.GiftId);
				Type type = giftAppearance.GetType();
				
				if(!string.IsNullOrEmpty(modGiftData.FrontSprite))
				{
					GameObject front = new GameObject();
					front.transform.SetParent(rootObject.transform);
					SpriteRenderer frontSprite = front.AddComponent<SpriteRenderer>();
					frontSprite.sprite = LoadModSprite(modGiftData.WorkshopId, modGiftData.FrontSprite);
					type.GetField("_frontSpriteRenderer", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(giftAppearance, frontSprite);
				}
				if(!string.IsNullOrEmpty(modGiftData.FrontBackSprite))
				{
					GameObject frontBack = new GameObject();
					frontBack.transform.SetParent(rootObject.transform);
					SpriteRenderer frontBackSprite = frontBack.AddComponent<SpriteRenderer>();
					frontBackSprite.sprite = LoadModSprite(modGiftData.WorkshopId, modGiftData.FrontBackSprite);
					type.GetField("_frontBackSpriteRenderer", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(giftAppearance, frontBackSprite);
				}
				if(!string.IsNullOrEmpty(modGiftData.SideSprite))
				{
					GameObject side = new GameObject();
					side.transform.SetParent(rootObject.transform);
					SpriteRenderer sideSprite = side.AddComponent<SpriteRenderer>();
					sideSprite.sprite = LoadModSprite(modGiftData.WorkshopId, modGiftData.SideSprite);
					type.GetField("_sideSpriteRenderer", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(giftAppearance, sideSprite);
				}
				if(!string.IsNullOrEmpty(modGiftData.SideBackSprite))
				{
					GameObject sideBack = new GameObject();
					sideBack.transform.SetParent(rootObject.transform);
					SpriteRenderer sideBackSprite = sideBack.AddComponent<SpriteRenderer>();
					sideBackSprite.sprite = LoadModSprite(modGiftData.WorkshopId, modGiftData.SideBackSprite);
					type.GetField("_sideBackSpriteRenderer", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(giftAppearance, sideBackSprite);
				}
				
				rootObject.transform.localPosition = Vector3.zero;
				rootObject.transform.localScale = Vector3.one;
				
				giftAppearanceDic.Add(gift.ClassInfo.Position, giftAppearance);
				
			}
			if (giftAppearance != null)
			{
				giftAppearance.Init(gift, (string)_instance.GetType().GetField("_layerName", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(_instance));
				BodyAura[] componentsInChildren = giftAppearance.gameObject.GetComponentsInChildren<BodyAura>();
				if (componentsInChildren != null && componentsInChildren.Length != 0)
				{
					foreach (BodyAura bodyAura in componentsInChildren)
						bodyAura.SetAppearance(_instance);
				}
				giftAppearance.RefreshAppearance(_instance.CustomAppearance, (CharacterMotion)_instance.GetType().GetField("_currentMotion", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(_instance));
			}
			return giftAppearance;
		}

	}

}