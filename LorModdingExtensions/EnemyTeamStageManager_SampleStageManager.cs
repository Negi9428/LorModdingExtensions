namespace LorModdingExtensions
{

	/// <summary>
	/// ステージ情報サンプル
	/// ModdingBaseを継承し、最低限mapDataAllyとworkshopIdを設定すれば動く
	/// これを適用したい戦闘のStageInfo.xml内のWaveで囲まれている中にManagerScriptとして指定してやればOK
	/// </summary>
	public class EnemyTeamStageManager_SampleStageManager : EnemyTeamStageManager_ModdingBase
	{
		private int testIndex = 0;

		/// <summary>
		/// MapData　以下の要領でMap名、BGMファイル名、背景画像名、床画像名を1セットで登録しておく
		/// BGMはStoryBgm内、背景/床画像はStoryBgSprite内に配置する
		/// 理論上は何MAPでも登録できるが、戦闘開始時のロードがかなり重くなるので注意
		/// </summary>
		protected override MapData[] mapDataAlly =>
			new[]
			{
				new MapData("map1", "test.wav", "test_bg.png", "test_floor.png"),
				new MapData("map2", "test.wav", "test_bg2.png", "test_floor2.png"),
				new MapData("map3", "test2.wav", "test_bg3.png", "test_floor.png"),
				new MapData("map4", "test3.wav", "test_bg4.png", "test_floor2.png"),
				new MapData("map5", "test3.wav", "test_bg5.png", "test_floor.png"),
				new MapData("map6", "test4.wav", "test_bg6.png", "test_floor2.png"),
			};

		/// <summary>
		/// workshopId 公式ツール内で設定できるworkshopIdと同一のものを設定する
		/// </summary>
		protected override string workshopId => "NegiMod1";
		
		/// <summary>
		/// 動作テスト用
		/// 同じ処理をPassive側とかに書いても多分動く
		/// </summary>
		public override void OnRoundStart()
		{
			base.OnRoundStart();
			
			//この2行の処理で敵側優勢を固定する
			//そもそもの仕様として敵側が優勢でないと敵専用MAPに変更する処理が発生しないので敵固有MAPにするのであれば必須
			//逆にこれを入れなければ敵が優勢の時のみMAPが変化する　司書優勢なら図書館になる
			int emotionTotalCoinNumber = StageController.Instance.GetCurrentStageFloorModel().team.emotionTotalCoinNumber;
			StageController.Instance.GetCurrentWaveModel().team.emotionTotalBonus = emotionTotalCoinNumber + 1;
			
			//このメソッドを呼べば対応したMAPに変化する　引数はMAP名
			StageController.Instance.GetStageModel().SetCurrentMapInfo(mapDataAlly[testIndex].MapName);
			
			//毎幕別のMAPに変更するための処理　MAP6まで行ったらMAP1に戻る
			if (++testIndex >= mapDataAlly.Length)
			{
				testIndex = 0;
			}
		}
	}

}