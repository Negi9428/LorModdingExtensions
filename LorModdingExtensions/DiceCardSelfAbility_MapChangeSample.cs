using System;
using LorModdingExtensions;
using UnityEngine;

/// <summary>
/// マップを上書きするバトルページ能力のサンプル
/// OnStartBattle内にてExtension_MapLoader.ChangeToEgoMapを呼び出すだけでOK
/// 渡すのは背景画像・床画像からなるEgoMapDataとModの公式ツール内で設定できるworkshopId
/// </summary>
public class DiceCardSelfAbility_MapChangeSample : DiceCardSelfAbilityBase
{
	/// <summary>
	/// マップ情報　別にフィールドで持たなくても関数内に直接入れても良い
	/// 画像は例によってStoryBgSprite内に配置する
	/// </summary>
	private readonly EgoMapData mapData = new EgoMapData("test_bg2.png", "test_floor.png");
	
	/// <summary>
	/// workshopId 公式ツール内で設定できるworkshopIdと同一のものを設定する
	/// </summary>
	private string workshopId = "NegiMod1";

	public override void OnStartBattle()
	{
		base.OnStartBattle();
		Extension_MapLoader.ChangeToEgoMap(mapData, workshopId);
		
		GameObject nullObject = null;
		try
		{
			//必ずNull
			nullObject.name = "";
		}
		catch (Exception e)
		{
			//エラーメッセージとスタックトレースを出力
			Extension.DebugLog(e.Message + "\n\n" + Environment.StackTrace);
		}
	}

}