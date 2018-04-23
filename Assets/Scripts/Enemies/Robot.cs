using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{


    private Enemy _enemy;
    private Rigidbody2D _rb2d;
    private PlayerController _player;
    private PhysicsMover _mover;

    public Vector3 NextCell;
    public Vector3 TargetCell;
    public bool CanMoveNext;

    // Use this for initialization
    void Awake()
    {
        _enemy = GetComponent<Enemy>();

        _rb2d = GetComponent<Rigidbody2D>();
        _mover = GetComponent<PhysicsMover>();
    }

    void Start()
    {
        _player = GameObject.FindObjectOfType<PlayerController>();
        _enemy.AvoidOthers = false;
        TargetCell = transform.position;
    }

    void Update()
    {
        var move_to_player = !_enemy.Stunned;
        var direction_to_player = (_player.transform.position - transform.position).normalized;
        var angle = Mathf.Atan2(direction_to_player.y, direction_to_player.x) + Mathf.PI;

        _mover.MoveVector = Vector3.zero;
        if (angle > 2f * Mathf.PI - Mathf.PI / 4f || angle < Mathf.PI / 4f)
        {
            NextCell = transform.position + new Vector3(-1f, 0, 0);
        }
        if (angle > Mathf.PI / 4f && angle < Mathf.PI / 2f + Mathf.PI / 4f)
        {
            NextCell = transform.position + new Vector3(0, -1f, 0);
        }
        if (angle > Mathf.PI / 2f + Mathf.PI / 4f && angle < Mathf.PI + Mathf.PI / 4f)
        {
            NextCell = transform.position + new Vector3(1f, 0, 0);
        }
        if (angle > Mathf.PI + Mathf.PI / 4f && angle < 2f * Mathf.PI - Mathf.PI / 4f)
        {
            NextCell = transform.position + new Vector3(0, 1f, 0);
        }

        if ((transform.position - TargetCell).magnitude > 0.01f)
        {
            _mover.MoveVector = (TargetCell - transform.position).normalized;
        }
        else
        {
            var has_same = false;
            foreach (var other in GameObject.FindObjectsOfType<Robot>())
            {
                if ((other.TargetCell - NextCell).magnitude < 0.1f)
                {
                    has_same = true;
                    break;
                }
            }
            if (!has_same)
                TargetCell = NextCell;
        }

    }
}
