using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public float MoveSpeed;
    public int HitScore = 1;
    public int MatchEnabledScore = 5;
    public int MatchScore = 50;
    public int DeathScore = 25;

    public int Damage = 1;
    public float DamageRate = 0.4f;
    private float _damageDelay = 0;
    public int MaxHealth = 100;
    public int Health = 100;
    public int MatchHealth = 50;
    public int HitsToChangeColor = 3;
    private int _hitsCounter = 0;
    public bool AllowedToMatch = false;
    public bool AvoidOthers = true;
    public int MatchType = 0;

    public bool FollowPlayer = true;
    public bool Stunned = false;
    public float StunDelay = 0.8f;
    private float _stunTimer = 0;
    public int OldX = -1;
    public int OldY = -1;
    public Color EnemyColor;
    private float _colorChangedTimer = 0;
    [HideInInspector]
    public GridChecker GridChecker;


    public AudioClip HurtSound;
    public AudioClip ExplodeSound;



    private Rigidbody2D _rb2d;
    private PlayerController _player;
    private PhysicsMover _mover;
    private AudioSource _audio;

    private SpriteRenderer _renderer;


    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        AllowedToMatch = false;
        Health = MaxHealth;
        _rb2d = GetComponent<Rigidbody2D>();
        _mover = GetComponent<PhysicsMover>();
        _audio = GetComponent<AudioSource>();
        _renderer = GetComponent<SpriteRenderer>();
    }
    // Use this for initialization
    void Start()
    {
        _player = GameObject.FindObjectOfType<PlayerController>();
        GridChecker = GameObject.FindObjectOfType<GridChecker>();
        GridChecker.AddEnemy(this);
        EnemyColor = Color.white;
    }


    public void Explode()
    {
        Effect.SpawnEffect("Explosion", transform.position);
        GridChecker.RemoveEnemy(this);
        DestroyEnemy();

    }
    public void DestroyEnemy()
    {
        GridChecker.RemoveEnemy(this);
        Destroy(gameObject);
    }

    public void Die(bool from_match = false, int match_count = 1)
    {

        if (from_match)
        {
            TextPopup.SpawnText((MatchScore * match_count).ToString(), transform.position);
            _player.AddScore(MatchScore * match_count);
        }
        else
        {
            TextPopup.SpawnText(DeathScore.ToString(), transform.position);
            CameraShake.ShakeCamera(2.5f);
            _player.AddScore(DeathScore);
        }
        Explode();
    }

    public void Hit(Vector2 velocity)
    {
        CameraShake.ShakeCamera(1f);
        Effect.SpawnEffect("Hit", transform.position);
        _audio.clip = HurtSound;
        _audio.volume = Random.Range(0.5f, 0.8f);
        _audio.pitch = Random.Range(0.5f, 1.4f);
        _audio.Play();
        if (!Stunned)
        {
            Stunned = true;
            _stunTimer = StunDelay;
        }
        _player.AddScore(HitScore);

        Health -= 1;
        if (Health < 0)
        {
            Die();
        }

        if (Health < MatchHealth && !AllowedToMatch)
        {
            _player.AddScore(MatchEnabledScore);
            // MatchType = Random.Range(0, 4);
            AllowedToMatch = true;
        }

        if (AllowedToMatch)
        {
            // if (_hitsCounter >= HitsToChangeColor)
            // {
            //     _hitsCounter = 0;
            //     MatchType = (MatchType + 1) % 4;
            // }
            // else
            // {
            //     _hitsCounter++;
            // }
            var c = GridChecker.Colors[MatchType];
            _renderer.color = EnemyColor = new Color(c.r, c.g, c.b, 1f);
        }

        _renderer.color = GameManager.Instance.HurtColor;
        _colorChangedTimer = 0.1f;

    }

    void FixedUpdate()
    {
        if (OldX >= 0 && OldX < 9 && OldY >= 0 && OldY < 9)
        {
            gameObject.layer = LayerMask.NameToLayer("Enemy");
        }
        if (AvoidOthers)
        {
            var near_enemies = Physics2D.OverlapCircleAll(transform.position, 0.7f, 1 << LayerMask.NameToLayer("Enemy"));
            if (near_enemies.Length > 1)
            {
                Collider2D e = null;
                foreach (var c in near_enemies)
                {
                    if (c.gameObject != gameObject)
                    {
                        e = c;
                    }
                }
                if (e == null)
                    return;
                var dir = (transform.position - e.transform.position);
                var v = 20f * _rb2d.mass;
                _rb2d.AddForce(v * dir.normalized);
            }
        }
    }


    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Character"))
        {
            if (_damageDelay <= 0)
            {
                _player.Damage(Damage);
                _damageDelay = DamageRate;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (_colorChangedTimer > 0)
        {
            _colorChangedTimer -= Time.deltaTime;
            if (_colorChangedTimer <= 0)
                _renderer.color = EnemyColor;
        }


        if (_damageDelay > 0)
        {
            _damageDelay -= Time.deltaTime;
        }


        if (_stunTimer > 0)
        {
            _stunTimer -= Time.deltaTime;
        }
        else
        {
            Stunned = false;
        }


    }
}
