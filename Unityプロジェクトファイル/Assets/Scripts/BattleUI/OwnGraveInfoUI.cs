using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

namespace Battle
{
    public class OwnGraveInfoUI : UIBase
    {
        public Text ownGraveMessageText;

        public void Show(GraveInfo graveInfo)
        {
            base.Show();

            switch (graveInfo.curseType)
            {
                case GraveInfo.CurseType.None:
                    ownGraveMessageText.text = "あなたは墓になにもありませんが、\n" + graveInfo.checkCounter + "人が調べました。\n辞世の句: " + graveInfo.deathMessage;
                    break;
                case GraveInfo.CurseType.Damage:
                    ownGraveMessageText.text = "あなたが墓に仕掛けた罠に\n" + graveInfo.checkCounter + "人が引っかかりました。\n" + graveInfo.checkCounter + "コインゲット！ \n辞世の句: " + graveInfo.deathMessage;
                    break;
                case GraveInfo.CurseType.Heal:
                    ownGraveMessageText.text = "あなたが墓にかけた回復魔法で\n" + graveInfo.checkCounter + "人が救済されました。\n" + graveInfo.checkCounter + "コインゲット！ \n辞世の句: " + graveInfo.deathMessage;
                    break;
                default:
                    break;
            }
        }
    }

}