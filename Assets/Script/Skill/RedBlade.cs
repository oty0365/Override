using UnityEditor;
using UnityEngine;

public class RedBlade: AAttack
{
    public Rigidbody2D rb2D;
    public float speed;
    public override void OnBirth()
    {
        rb2D.linearVelocity = transform.up*speed;
    }
    public override void OnDeathInit()
    {
        
    }
    void Start()
    {
        
    }

    
}
