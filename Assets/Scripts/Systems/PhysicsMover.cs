using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsMover : MonoBehaviour
{

    public bool RotateToOrientation = false;
    public float MaxSpeed = 10f;
    public float MoveForce = 10f;

    public Vector3 MoveVector;
    public Vector3 Orientation;
    private Rigidbody2D _rb2d;

    void Awake()
    {
        _rb2d = GetComponent<Rigidbody2D>();
    }


    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.Paused)
            return;
        var angle = Mathf.Atan2(Orientation.y, Orientation.x);
        if (MoveVector.magnitude > 0)
        {
            _rb2d.AddForce(MoveForce * MoveVector * _rb2d.mass);

            if (_rb2d.velocity.magnitude > MaxSpeed)
                _rb2d.velocity = MaxSpeed * _rb2d.velocity.normalized;

        }
        if (RotateToOrientation)
        {
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * angle + 90f);
        }
    }

}
