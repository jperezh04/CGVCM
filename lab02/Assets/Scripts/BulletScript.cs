using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{

    public AudioClip Sound;
    public float Speed;
    private Rigidbody2D Rigidbody2D;
    private Vector2 Direction;
    void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        Camera.main.GetComponent<AudioSource>().PlayOneShot(Sound);
    }

   
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Rigidbody2D.linearVelocity = Direction * Speed;
    }
    public void SetDirection(Vector2 direction)
    {
        Direction = direction;
    }

    public void DestroyBullet()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        jhon_movement john = collision.GetComponent<jhon_movement>();
        GruntScript grunt = collision.GetComponent<GruntScript>();
        if(john != null)
        {
            john.Hit();
        }
        if( grunt != null)
        {
            grunt.Hit();
        }
        DestroyBullet();
    }
}
