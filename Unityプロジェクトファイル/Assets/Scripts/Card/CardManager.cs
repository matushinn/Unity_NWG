using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

//カード管理//
public class CardManager : SingletonMonoBehaviour<CardManager>
{
    public static readonly Rect CARDIMAGESIZE = new Rect(0, 0, 159, 256);

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private List<CardData> cardDataList = new List<CardData>()
    {
        {new CardData("Fast15", "素早さアップ", CardData.Type.Fast, 1.5f, "NN", "Card_Fast15") },
        {new CardData("Fast20", "素早さアップ", CardData.Type.Fast, 2.0f, "NR", "Card_Fast20") },
        {new CardData("Shield15", "防御アップ", CardData.Type.Shield, 1.5f, "SR", "Card_Shield15") },
        {new CardData("Shield20", "防御アップ", CardData.Type.Shield, 2.0f, "UR", "Card_Shield20") }
    };

    public List<Texture2D> textures = new List<Texture2D>();

    public Texture2D GetTextureByCardId(string cardId)
    {
        string fileName = cardDataList.First(cardData => cardData.id == cardId).textureDataName;
        return textures.Find(file => file.name == fileName);
    }

    public Texture2D GetTextureByCarData(CardData cardData)
    {
        return textures.Find(file => file.name == cardData.textureDataName);
    }

    public CardData GetCardDataByCardId(string cardId)
    {
        return cardDataList.First(cardData => cardData.id == cardId);
    }

    public CardData GetCardDataFromRarelity(string gachaListNumber, string rarelityStr)
    {
        //CardData.Rarelity rarelity = (CardData.Rarelity)System.Enum.Parse(typeof(CardData.Rarelity), rarelityStr);

        //gachaListNumberはダミー　複数ガチャがあるとき用//
        return cardDataList.First(cardData => cardData.rarelity == rarelityStr);
    }
}