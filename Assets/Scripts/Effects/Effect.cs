using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{

    public int ParticleCount;
    public ParticleSystem Particles;
    public AudioClip Sound;
    public float Timer = 1f;
    private AudioSource _audio;

    public static void SpawnEffect(string name, Vector3 position)
    {
        var prefab = Resources.Load<GameObject>(name);
        var go = GameObject.Instantiate(prefab);
        go.GetComponent<Effect>().Fire(position);
    }

    public void Fire(Vector3 position)
    {
        _audio = GetComponent<AudioSource>();
        transform.position = position;
        if (Particles != null)
            Particles.Emit(ParticleCount);
        _audio.clip = Sound;
        _audio.volume = Random.Range(0.5f, 0.8f);
        _audio.pitch = Random.Range(0.5f, 1.4f);
        _audio.Play();
        Destroy(gameObject, Timer);

    }
}
