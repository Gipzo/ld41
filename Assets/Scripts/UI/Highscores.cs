using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Highscores : MonoBehaviour
{
    private Text _text;
    private string get_url = "https://plassion.com/getrec.php";
    private string set_url = "https://plassion.com/setrec.php";
    private string private_code = "aT7HraH5iOFKvErr";
    private string public_code = "ufxDZ2WCoULiySYc";


    private static Highscores _instance;
    public static Highscores Instance
    {
        get
        {
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<Highscores>();
            return _instance;
        }
    }

    void Awake()
    {
        _text = GetComponent<Text>();
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        StartCoroutine(GetHighscores());
    }

    public void SendScore(string nick, long score)
    {
        StartCoroutine(SetHighscore(nick, score));
    }

    IEnumerator SetHighscore(string nick, long score)
    {
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        long currentEpochTime = (long)(System.DateTime.UtcNow - epochStart).TotalSeconds;

        var form = new WWWForm();
        form.AddField("cod", private_code);
        form.AddField("uid", currentEpochTime.ToString());
        form.AddField("val", score.ToString());
        form.AddField("txt", nick);

        var d = System.Text.Encoding.Default.GetString(form.data);

        UnityWebRequest www = UnityWebRequest.Get(set_url + "?" + d);
        yield return www.SendWebRequest();


        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
        }
        StartCoroutine(GetHighscores());

    }

    IEnumerator GetHighscores()
    {

        using (WWW www = new WWW(get_url + "?cod=" + public_code+"&qty=16"))
        {
            yield return www;

            var data = www.text.Split('\n');

            var scores = "";
            if (data.Length > 2)
            {
                for (int i = 2; i < data.Length; i++)
                {
                    var score_data = data[i].Split(',');
                    if (score_data.Length > 1)
                    {
                        scores += score_data[1] + " " + score_data[0] + "\n";
                    }
                }
            }

            _text.text = "HIGHSCORES:\n\n" + scores;
            if (www.error != null)
                Debug.Log(www.error);
        }
    }


}
