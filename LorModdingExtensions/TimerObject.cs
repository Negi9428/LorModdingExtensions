using UnityEngine;

namespace LorModdingExtensions
{

	public class TimerObject : MonoBehaviour
	{

		public readonly StartBattleEffect effect = new StartBattleEffect();

		private void Awake()
		{
			effect.isDone = false;
		}

		private void OnDestroy()
		{
			effect.isDone = true;
		}

	}

}