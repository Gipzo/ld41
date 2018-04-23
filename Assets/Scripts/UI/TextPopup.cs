using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextPopup : MonoBehaviour
{
    public Text Text;
    public float FloatSpeed = 2f;
    public float Timer = 1f;

    public static void SpawnText(string title, Vector3 position)
    {
        var prefab = Resources.Load<GameObject>("TextPopup");
        var go = GameObject.Instantiate(prefab, position, Quaternion.identity);
        go.GetComponent<TextPopup>().Show(title);
    }

    public void Show(string text)
    {
        Text.text = text;
        Destroy(gameObject, Timer);
    }

    void Update()
    {
        transform.position += new Vector3(0, Time.deltaTime * FloatSpeed, 0);
    }
}
