using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    public int Health = 100;
    public int MaxHealth = 100;
    public bool Firing;
    public long Score = 0;
    public Text ScoreText;
    public Vector3 Orientation;
    public Vector3 MoveVector;
    private Camera _camera;
    private Rigidbody2D _rb2d;
    private Spawner _spawner;
    private PhysicsMover _mover;
    private Shooter _shooter;
    private SpriteRenderer _renderer;
    public LineRenderer LineRenderer;
    private float _colorChangedTimer = 0;
    private Color DefaultColor = Color.white;

    public float BonusTimer = 0;
    public float MaxBonusTimer = 10f;
    public BonusType CurrentBonus = BonusType.None;

    void Start()
    {
        _camera = Camera.main;
        _shooter = GetComponent<Shooter>();
        _mover = GetComponent<PhysicsMover>();
        _renderer = GetComponent<SpriteRenderer>();
        _spawner = GameObject.FindObjectOfType<Spawner>();
        AddScore(0);
    }

    public void Init()
    {
        _renderer.color = DefaultColor = Color.white;
        Health = MaxHealth;
        transform.position = new Vector3(0, -1f, 0);
        Score = 0;
        _shooter.ProjectileWeight = 10f;
        _shooter.ProjectileCount = 1;
    }

    public void Damage(int damage)
    {
        Health -= damage;
        Effect.SpawnEffect("Hurt", transform.position);
        _renderer.color = GameManager.Instance.HurtColor;
        _colorChangedTimer = 0.1f;
        if (Health <= 0)
        {
            DefaultColor = _renderer.color = Color.black;
            Effect.SpawnEffect("Explosion_Big", transform.position);
            GameManager.Instance.GameEnded(true);
        }
    }

    public void ApplyBonus(BonusType type)
    {
        switch (type)
        {
            case BonusType.Health:
                Health = Mathf.Min(Health + 10, MaxHealth);
                GameManager.Instance.ShowMessage("+10 HEALTH");
                break;
            case BonusType.Weight:
                _shooter.ProjectileWeight += 10f;
                GameManager.Instance.ShowMessage("+PROJECTILE WEIGHT");
                break;
            case BonusType.Double:
                if (_shooter.ProjectileCount == 1)
                {
                    BonusTimer = 10f;
                    _shooter.ProjectileCount = 2;
                    GameManager.Instance.ShowMessage("DOUBLE BARREL!");
                }
                break;
            case BonusType.Triple:

                if (_shooter.ProjectileCount <= 2)
                {
                    BonusTimer = 7f;
                    _shooter.ProjectileCount = 3;
                    GameManager.Instance.ShowMessage("TRIPLE BARREL!");
                }
                break;
            case BonusType.Mega:
                if (_shooter.ProjectileCount <= 3)
                {
                    BonusTimer = 5f;
                    _shooter.ProjectileCount = 5;
                    GameManager.Instance.ShowMessage("MEGA BARREL");
                }
                break;
        }
    }

    public void AddScore(int value)
    {
        Score += value;
        ScoreText.text = "SCORE:\n" + Score.ToString() +
         "\n\nWAVE:\n" + _spawner.CurrentWave.ToString() +
         "\n\nENEMIES:\n" + _spawner.EnemiesLeft.ToString();
    }

    void GetInput()
    {
        if (GameManager.Instance.Paused)
        {
            Firing = false;
            return;
        }
        _mover.MoveVector = MoveVector = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        Firing = Input.GetButton("Fire");
        var m = transform.position - _camera.ScreenToWorldPoint(Input.mousePosition);
        Orientation = new Vector2(m.x, m.y).normalized;

    }

    void ShowLaserPointer()
    {

        LineRenderer.SetPosition(0, transform.position);
        int mask = 1 << LayerMask.NameToLayer("Character");
        mask = ~mask;
        mask = mask << LayerMask.NameToLayer("CharacterProjectiles");
        var rh = Physics2D.Raycast(transform.position, -Orientation, 20f, mask);
        LineRenderer.SetPosition(1, new Vector3(rh.point.x, rh.point.y, 0));
    }

    void Update()
    {

        if (_colorChangedTimer > 0)
        {
            _colorChangedTimer -= Time.deltaTime;
            if (_colorChangedTimer <= 0)
                _renderer.color = DefaultColor;
        }

        if (BonusTimer > 0)
        {
            BonusTimer -= Time.deltaTime;
            if (BonusTimer <= 0)
            {
                _shooter.ProjectileCount = 1;
            }
        }
        GetInput();
        ShowLaserPointer();
        _mover.Orientation = Orientation;

        _shooter.Orientation = -Orientation;
        _shooter.FireEnabled = Firing;

    }
}
