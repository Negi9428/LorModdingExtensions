using System;
using System.Collections.Generic;
using UnityEngine;

namespace LorModdingExtensions
{
  /// <summary>
  /// Gift未対応のスキン（顔固定スキン？）にGiftを乗せるためのダミー用クラス
  /// </summary>
  public class DummyCustomizedAppearance : CustomizedAppearance
	{

		public void Initialize(CharacterAppearance characterAppearance)
		{
			face = new SpriteRenderer[0];
			brow = new SpriteRenderer[0];
			mouth = new SpriteRenderer[0];
			head = new SpriteRenderer[0];
			frontHair = new SpriteRenderer[0];
			backHair = new SpriteRenderer[0];
			allSpriteList = new List<SpriteRenderer>();
      _scaleFactor = characterAppearance.customHeadScaleFactor;
    }

		public override void ChangeLayer(string layerName)
		{
		}

		public override void RefreshAppearanceByMotion(CharacterMotion motion)
    {

      int num1 = 1000;
      int num2 = 1000;
      int num3 = 1000;
      int num4 = 1000;
      SpriteSet spriteSet1 = null;
      SpriteSet spriteSet2 = null;
      foreach (SpriteSet t in motion.motionSpriteSet)
      {
        int sortingOrder = t.sprRenderer.sortingOrder;
        switch (t.sprType)
        {
          case CharacterAppearanceType.FrontHair:
            if (sortingOrder < num2)
            {
              num2 = sortingOrder;
            }
            break;
          case CharacterAppearanceType.RearHair:
            if (sortingOrder < num3)
            {
              num3 = sortingOrder;
            }
            break;
          case CharacterAppearanceType.Face:
            if (sortingOrder < num1)
            {
              num1 = sortingOrder;
            }
            break;
          case CharacterAppearanceType.Head:
            if (sortingOrder < num4)
              num4 = sortingOrder;
            spriteSet1 = t;
            break;
          case CharacterAppearanceType.Hood:
            if (spriteSet2 == null)
            {
              spriteSet2 = t;
              break;
            }
            if (sortingOrder > spriteSet2.sprRenderer.sortingOrder)
            {
              spriteSet2 = t;
            }
            break;
        }
      }

      if (spriteSet1 != null)
      {
        transform.parent = motion.customPivot == null ? spriteSet1.sprRenderer.transform : motion.customPivot;
      }
      else
        Debug.Log("There is not [Head] in motion." + motion.actionDetail);
      transform.localPosition = Vector3.zero;
      transform.localRotation = Quaternion.identity;
      transform.localScale = Vector3.one * _scaleFactor;
      if (motion.motionSpriteSet.Exists(x => x.sprType == CharacterAppearanceType.Hood))
      {
        for (int index = 0; index < this.backHair.Length; ++index)
          backHair[index].gameObject.SetActive(false);
      }
      OnRefreshAppearance refreshAppearance = this._onRefreshAppearance;
      refreshAppearance?.Invoke(this);
    }

    public override void SetSpriteByGift(CharacterAppearanceType appearanceType, bool enable)
    {
    }
    
  }

}