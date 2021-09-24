namespace LorModdingExtensions
{
	/// <summary>
	/// バフ追加サンプル
	/// </summary>
	public class BattleUnitBuf_SampleBuf : BattleUnitBuf
	{

		/// <summary>
		/// バフID　関数からバフを探す場合などの引数に指定する　重複不可
		/// </summary>
		protected override string keywordId => "sampleBuf_negi";
		
		/// <summary>
		/// バフの名称
		/// </summary>
		private static readonly string name = "テストバフです";
		
		/// <summary>
		/// バフの説明文　{0}はバフのスタック数に置換される
		/// </summary>
		private static readonly string desc = "テスト説明文 スタック数は{0}です";
		
		/// <summary>
		/// アイコン画像名　StoryBgSprite内に配置する
		/// </summary>
		private static readonly string icon = "Test_Icon.png";
		
		/// <summary>
		/// workshopId 公式ツール内で設定できるworkshopIdと同一のものを設定する
		/// </summary>
		private static readonly string workshopID = "NegiMod1";
		
		// 既存のアイコンを使用したい場合はここを設定する　対応文字列はKeywordBufの定義を参照のこと
		// protected override string keywordIconId => "Alriune_Petal";

		public override void Init(BattleUnitModel owner)
		{
			base.Init(owner);
			
			//これを呼び出すことでゲーム本体のバフ情報に追加できる
			//既存のアイコンを使用する場合はAddEffectTextの方を呼び出せばよい
			BattleEffectTextsXmlList.Instance.AddEffectTextAndIcon(keywordId, name, desc, workshopID, icon);
		}

		public override void OnRoundEnd()
		{
			Destroy();
		}

	}

}