using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ScoreboardHearts : MonoBehaviour
{
    [SerializeField] private Image[] hearts;
    [SerializeField] private Image[] crosses;
    [SerializeField] private Sprite redHeart;
    [SerializeField] private Sprite greenHeart;
    [SerializeField] private Sprite blueCross;
    [SerializeField] private Sprite yellowCross;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(heartSwap());
        StartCoroutine(crossSwap());
    }

    // Update is called once per frame

    IEnumerator heartSwap()
    {
        while (true)
        {
            foreach (Image heartImage in hearts)
            {
                if (heartImage.sprite == redHeart)
                {
                    heartImage.sprite = greenHeart;
                }
                else
                {
                    heartImage.sprite = redHeart;
                }

                yield return new WaitForSeconds(2);
            }

            
        }     

    }

    IEnumerator crossSwap()
    {
        while (true)
        {
            foreach (Image crossImage in crosses)
            {
                if (crossImage.sprite == blueCross)
                {
                    crossImage.sprite = yellowCross;
                }
                else
                {
                    crossImage.sprite = blueCross;
                }

                yield return new WaitForSeconds(1);
            }
        }
    }
}
