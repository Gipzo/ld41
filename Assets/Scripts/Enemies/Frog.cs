using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog : MonoBehaviour
{

    private Enemy _enemy;
    private Rigidbody2D _rb2d;
    private PlayerController _player;
    private PhysicsMover _mover;

    public float MaxSpeed = 3f;
    public Vector3 NextJump;
    public Vector3 CurrentJump;
    public float JumpDelay = 3f;
    private float _jumpTimer = 0;
    public float FlyTime = 1.1f;
    private float _flyTimer = 0;
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

    }

    // Update is called once per frame
    void Update()
    {
        var move_to_player = !_enemy.Stunned;
        var direction_to_player = (_player.transform.position - transform.position).normalized;

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
                _mover.MaxSpeed = MaxSpeed;
                _mover.MoveVector = CurrentJump.normalized;
            }
            else
            {
                _jumpTimer = JumpDelay;
                _flyTimer = 0;
            }

        }

    }
}
