using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    private AudioSource _source;


    public AudioClip ShotSound;
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        _source = GetComponent<AudioSource>();
    }

    // Use this for initialization
    void Start()
    {

    }

    public void PlayShot()
    {
        _source.PlayOneShot(ShotSound);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
