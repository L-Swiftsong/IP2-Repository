using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class DecptiveScreen : Ability
{
    public GameObject Clone;
    public int AmountOfClones;

    public override void Activate(GameObject parent, Transform transform)
    {
        float x = parent.transform.position.x;
        float y = parent.transform.position.y;

        float r = parent.transform.rotation.z;

        for (int i = 0; i < AmountOfClones; i++)
        {

            GameObject Clone1 = Instantiate(Clone, new Vector3(x, y, transform.position.z), Quaternion.identity);


            GameObject.Destroy(Clone1, activeTime);

        }



        

    }
}
