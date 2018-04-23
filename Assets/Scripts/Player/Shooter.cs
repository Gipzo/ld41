using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : MonoBehaviour
{

    public float ProjectileOffset;

    public float ProjectileVelocity = 10f;
    public float FireRate = 1f;
    private bool _canFire = true;
    private float _timer = 0;
    public GameObject ProjectilePrefab;
    public bool FireEnabled = false;
    public Vector3 Orientation;

    public ParticleSystem Particles;
    private AudioManager _audio;
    public float ProjectileWeight = 10f;
    public int ProjectileCount = 3;
    public float ProjectileAngle = 5f;

    void Start()
    {
        _audio = GameObject.FindObjectOfType<AudioManager>();
    }

    public void Fire()
    {
        if (Particles != null)
        {
            Particles.Emit(20);
        }
        _audio.PlayShot();
        var angle = Mathf.Atan2(Orientation.y, Orientation.x);

        if (ProjectileCount == 1)
        {
            var go = GameObject.Instantiate(ProjectilePrefab);
            go.GetComponent<Projectile>().Fire(transform.position + Orientation * ProjectileOffset, ProjectileVelocity * Orientation, ProjectileWeight);
            go.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * angle + 90f);
        }
        else
        {
            var angle_start = -ProjectileCount * Mathf.Deg2Rad * ProjectileAngle / 2f;
            for (int i = 0; i < ProjectileCount; i++)
            {
                var a = angle + angle_start + Mathf.Deg2Rad * i * ProjectileAngle;
                var o = new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0);
                var go2 = GameObject.Instantiate(ProjectilePrefab);
                go2.GetComponent<Projectile>().Fire(transform.position + o * ProjectileOffset, ProjectileVelocity * o, ProjectileWeight);
                go2.transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * a + 90f);

            }
        }
    }

    void Update()
    {
        if (_timer > 0)
        {
            _canFire = false;
            _timer -= Time.deltaTime;
        }
        else
        {
            _canFire = true;
        }

        if (FireEnabled && _canFire)
        {
            Fire();
            _timer = FireRate;
        }
    }

}
