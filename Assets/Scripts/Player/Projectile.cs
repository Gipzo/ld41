using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    private bool _used = false;
    private Rigidbody2D _rb2d;

    void Awake()
    {
        _rb2d = GetComponent<Rigidbody2D>();
        gameObject.SetActive(false);
    }

    public void Fire(Vector3 origin, Vector3 velocity, float weight)
    {
        transform.position = origin;
        gameObject.SetActive(true);
        _rb2d.velocity = velocity;
        _rb2d.mass = weight;
        GetComponent<ParticleSystem>().Clear();
        GetComponent<ParticleSystem>().Stop();
        GetComponent<ParticleSystem>().Play();
        _used = false;
        // _rb2d.AddForce(velocity, ForceMode2D.Impulse);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (!_used && other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            _used = true;
            other.gameObject.GetComponent<Enemy>().Hit(_rb2d.velocity);
            gameObject.layer = LayerMask.NameToLayer("ProjectileShell");
        }

        Destroy(gameObject, 1f);
    }

}
