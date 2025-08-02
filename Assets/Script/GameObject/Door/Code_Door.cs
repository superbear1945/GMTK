using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Code_Door : MonoBehaviour, Interface_Door
{

    [SerializeField] private Collider2D _doorCollider;

    // Start is called before the first frame update
    void Start()
    {
        _doorCollider.isTrigger = true; // Set the door collider to be a trigger
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            BeginOverlap(collision.gameObject);
        }
    }

    public void BeginOverlap(GameObject obj)
    {
        if(GameManager.Instance._getKey)
        {
            gameObject.SetActive(false); // Disable the door if the player has the key
        }
        else
        {
            //没钥匙不开门
        }
    }

}
