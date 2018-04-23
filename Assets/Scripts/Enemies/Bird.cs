using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{

    private Enemy _enemy;
    private Rigidbody2D _rb2d;
    private PlayerController _player;
    private PhysicsMover _mover;

    public float MaxSpeed = 3f;
    public Vector3 NextJump;
    public Vector3 CurrentJump;
    public float JumpDelay = 3f;
    public float ExplodeDelay = 10f;
    private float _explodeTimer = 0f;
    private float _jumpTimer = 0;
    public float FlyTime = 1.1f;
    private float _flyTimer = 0;
    public float FlyDeviationSpeed = 5f;
    public float FlyDeviationValue = 5f;
    // Use this for initialization

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        _jumpTimer = Random.Range(0, JumpDelay);
        _enemy = GetComponent<Enemy>();
        _rb2d = GetComponent<Rigidbody2D>();
        _mover = GetComponent<PhysicsMover>();
        NextJump = transform.position;
    }
    void Start()
    {
        _player = GameObject.FindObjectOfType<PlayerController>();
        _enemy.AllowedToMatch = true;
        var c = _enemy.GridChecker.Colors[_enemy.MatchType];
        GetComponent<SpriteRenderer>().color = new Color(c.r, c.g, c.b, 1f);
        _explodeTimer = ExplodeDelay;
    }

    // Update is called once per frame
    void Update()
    {
        var move_to_player = !_enemy.Stunned;
        var random_cell = new Vector3(Random.RandomRange(-9f, 9f), Random.RandomRange(-9f, 9f), 0);
        var direction_to_player = (random_cell - transform.position).normalized;

        _mover.MoveVector = Vector3.zero;

        NextJump = direction_to_player;

        if (_jumpTimer > 0)
        {
            _jumpTimer -= Time.deltaTime;
            _mover.MaxSpeed = 0;
            CurrentJump = NextJump;
        }
        else
        {

            if (_flyTimer < FlyTime)
            {
                _flyTimer += Time.deltaTime;
                var j_angle = Mathf.Atan2(CurrentJump.normalized.y, CurrentJump.normalized.x) + FlyDeviationValue * Mathf.Sin(FlyDeviationSpeed * Time.timeSinceLevelLoad / 3f) * Mathf.Cos(FlyDeviationSpeed * Time.timeSinceLevelLoad / 7f) * Mathf.Sin(FlyDeviationSpeed * Time.timeSinceLevelLoad / 11f);
                var dir = new Vector3(Mathf.Cos(j_angle), Mathf.Sin(j_angle), 0);
                _mover.MaxSpeed = MaxSpeed;
                _mover.MoveVector = dir;
            }
            else
            {
                _jumpTimer = JumpDelay;
                _flyTimer = 0;
            }

        }
        if (_explodeTimer > 0)
        {
            _explodeTimer -= Time.deltaTime;
        }
        else
        {
            _enemy.Explode();
        }

    }
}
