using UnityEngine;

public class FireBehaviour : MonoBehaviour
{
    private float _initialLife = 5f;
    private float _life = 5f;

    private void Start()
    {
        _life = _initialLife;
        _initialLife *= 1 / transform.localScale.x;
        var bike = FindFirstObjectByType<BikeBehaviour>();
        var bikeRb = bike.gameObject.GetComponent<Rigidbody2D>();
        
        var velocity = bikeRb.velocity;
        if (velocity.sqrMagnitude < 0.1)
        {
            velocity = Random.insideUnitCircle * 1.5f;
        }

        var force = velocity * -1;
        transform.position = bikeRb.position + force.normalized * 0.5f; 
        
        var rb = GetComponent<Rigidbody2D>();
        rb.WakeUp();
        rb.velocity = force;
    }
    
    private void Update()
    {
        transform.localScale = Vector2.one * _life / _initialLife;
        transform.Rotate(new Vector3(0, 0, Time.deltaTime * 360f));
        _life -= Time.deltaTime;
        if (_life < 0)
        {
            Destroy(gameObject);
        }
    }
}
