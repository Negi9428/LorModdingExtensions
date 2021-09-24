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

	}

}