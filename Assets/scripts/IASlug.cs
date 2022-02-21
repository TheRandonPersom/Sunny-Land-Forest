using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IASlug : MonoBehaviour
{

    public  Transform       enemie;
    public  SpriteRenderer enemieSprit;
    public  Transform[]     position;
    public  float           speed;
    public  bool            isRight;

    private int             idTarget;

    // Start is called before the first frame update
    void Start()
    {

        enemieSprit = enemie.gameObject.GetComponent<SpriteRenderer>();
        enemie.position = position[0].position;
        idTarget = 1;

    }

    // Update is called once per frame
    void Update()
    {
        
        if(enemie != null)
        {
            enemie.position = Vector3.MoveTowards(enemie.position, position[idTarget].position, speed * Time.deltaTime);

            if(enemie.position == position[idTarget].position)
            {
                idTarget += 1;
                if(idTarget == position.Length)
                {
                    idTarget = 0;
                }
            }

            if (position[idTarget].position.x < enemie.position.x && isRight == true)
            {
                Flip();
            }
            else if (position[idTarget].position.x > enemie.position.x && isRight == false)
            {
                Flip();
            }

        }

    }

    void Flip()
    {

        isRight = !isRight;
        enemieSprit.flipX = !enemieSprit.flipX;
    }
}
