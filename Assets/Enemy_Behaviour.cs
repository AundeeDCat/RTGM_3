using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Behaviour : MonoBehaviour
{

    private float speed = 1f;

    public Material Item_Skin;

    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.gameObject.tag == "Item")
        {
            Ragdoll();
        }

        else
        {
            Attack();
        }
    }

    private void Ragdoll()
    {
        this.gameObject.GetComponent<MeshRenderer>().material = Item_Skin;
    }

    private void Attack()
    {
        Vector3 target = player.transform.position;
        float step = speed * Time.deltaTime;

        this.transform.LookAt(target);
        this.transform.position = Vector3.MoveTowards(transform.position, target, step);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Item")
        {
            this.gameObject.tag = "Item";
        }
    }
}
