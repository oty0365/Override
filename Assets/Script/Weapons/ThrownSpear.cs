using NUnit.Framework.Constraints;
using UnityEngine;

public class ThrownSpear : APoolingObject
{
    public Rigidbody2D rb;
    public float shootSpeed;

    void Start()
    {
        
    }

    void Update()
    {
        
    }
    public override void OnBirth()
    {
        rb.linearVelocity = (transform.right+ transform.up).normalized*shootSpeed;
    }
    public override void OnDeathInit()
    {

    }
}
