using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldController : MonoBehaviour
{
    private Animator animator;
    private bool growing = true;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        animator.SetTrigger("hit");
    }

        // Update is called once per frame
        void Update()
    {
        transform.Rotate(new Vector3(0,0,Time.deltaTime*120));
        if (growing)
        {
            if (transform.localScale.x < 1.22f)
            {
                transform.localScale = transform.localScale * 1.008f;
            }
            else
            {
                growing = false;
            }
        }
        else
        {
            if (transform.localScale.x > 1f)
            {
                transform.localScale = transform.localScale * 0.992f;
            }
            else
            {
                growing = true;
            }
        }
        
        
        
    }
}
