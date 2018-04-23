using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowEnemy : MonoBehaviour
{

    public float MoveForce = 300f;
    public float MoveDeviation = 100f;
    public float MoveCycle = 2f;
    public float OrientationChange = 1f;
    public float OrientationChangeSpeed = 1f;
    private float _seed;
    private Enemy _enemy;
    private Rigidbody2D _rb2d;
    private PlayerController _player;
    private PhysicsMover _mover;
    // Use this for initialization

    void Awake()
    {
        _seed = Random.Range(0, 100f);
        _enemy = GetComponent<Enemy>();
        _rb2d = GetComponent<Rigidbody2D>();
        _mover = GetComponent<PhysicsMover>();
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
        var angle = Mathf.Atan2(direction_to_player.y, direction_to_player.x) + Mathf.Sin(_seed + Time.timeSinceLevelLoad * OrientationChangeSpeed) * OrientationChange;
        direction_to_player = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);


        _mover.MaxSpeed = MoveForce + MoveDeviation * Mathf.Pow(Mathf.Sin(_seed + Time.timeSinceLevelLoad / MoveCycle), 4f);
        _mover.MaxSpeed = _player.MoveVector.magnitude > 0 ? _mover.MaxSpeed : MoveForce * 0.1f;
        _mover.MoveVector = move_to_player ? direction_to_player : Vector3.zero;

    }
}
