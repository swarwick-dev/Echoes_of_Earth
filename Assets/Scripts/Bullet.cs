using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float damage ;
    public GameObject prefab ;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        speed = 20f;
        damage = 10f;        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
