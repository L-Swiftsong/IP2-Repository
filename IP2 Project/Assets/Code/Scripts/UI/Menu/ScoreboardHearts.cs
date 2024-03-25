using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ScoreboardHearts : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] spriteRenderer;
    [SerializeField] private Sprite redHeart;
    [SerializeField] private Sprite greenHeart;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(spriteSwap());
    }

    // Update is called once per frame
    
    IEnumerator spriteSwap()
    {
        for (int i = 0; i == 3; i++)
        {
            if (spriteRenderer[0] == redHeart)
            {
                spriteRenderer[0].sprite = greenHeart;
            }
            else if(spriteRenderer[1] == redHeart)
            {
                spriteRenderer[1].sprite = greenHeart;
            }
            else if (spriteRenderer[2] == redHeart)
            {
                spriteRenderer[2].sprite = greenHeart;
            }
            if (i == 3)
            {
                foreach (SpriteRenderer heartSprite in spriteRenderer)
                {
                    heartSprite.sprite = redHeart;
                }
            }
        }

        yield return new WaitForSeconds(2);

       
    }
}
