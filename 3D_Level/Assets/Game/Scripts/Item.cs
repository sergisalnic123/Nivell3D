using System.Collections;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public class Item : MonoBehaviour
{
    public enum ItemType
    {
        LIFE,
        COLLECTABLE
    }

    public GameManager _gameManager;
    public ItemType _itemType;

    [Header("Life")]
    public int _LifeValue;

    [Header("Collectables")]
    public int _CollectableValue;


    public void TakeGeneralItem()
    {

        if (_itemType == ItemType.LIFE) TakeLife();

        if (_itemType == ItemType.COLLECTABLE) TakeCollectable();

    }

    void TakeLife()
    {

    }

    void TakeCollectable()
    {
        _gameManager._playerController.AddCollectableItems();
        DestroyItem();

    }

    void DestroyItem()
    {
        GameManager.Destroy(this.gameObject);
    }
}
