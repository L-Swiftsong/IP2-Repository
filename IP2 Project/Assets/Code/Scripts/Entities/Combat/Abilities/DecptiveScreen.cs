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

        for (int i = 0; i < AmountOfClones; i++)
        {
            float randVar = Random.Range(0f, 0.2f);
            GameObject clone = Instantiate(Clone, new Vector3(x + randVar, y + randVar, transform.position.z), Quaternion.identity);

            GameObject.Destroy(clone, activeTime);
        }
    }
}
