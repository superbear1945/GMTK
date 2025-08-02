using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour, Interface_Keypriviledge
{
    [SerializeField] private Collider2D _keyCollider;
    private bool isCollected = false;
    
    // Start is called before the first frame update
    void Start()
    {
        _keyCollider.isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HasKey()
    {
        GameManager.Instance.SetHasKey(this);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && !isCollected)
        {
            isCollected = true;
            _keyCollider.enabled = false;
            HasKey();
            Destroy(gameObject, 0.5f); // Delay destruction to allow for collection feedback
        }
    }

}
