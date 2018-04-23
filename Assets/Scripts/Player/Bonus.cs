using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BonusType
{
    None = -1,
    Health = 0,
    Weight = 1,
    Double = 2,
    Triple = 3,
    Mega = 4
}

public class Bonus : MonoBehaviour
{

    public Sprite[] Sprites;
    public BonusType BonusType;


    public void Init(BonusType type)
    {
        BonusType = type;
        GetComponent<SpriteRenderer>().sprite = Sprites[(int)type];
    }

    public static void SpawnBonus(BonusType type, Vector3 position)
    {
        var prefab = Resources.Load<GameObject>("Bonus");
        var go = GameObject.Instantiate(prefab, position, Quaternion.identity);
        go.GetComponent<Bonus>().Init(type);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Character"))
        {
            other.gameObject.GetComponent<PlayerController>().ApplyBonus(BonusType);
            Destroy(gameObject);
        }

    }
}
