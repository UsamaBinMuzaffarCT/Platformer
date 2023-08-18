using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AnimateAndDestroy : MonoBehaviour
{
    [SerializeField] private List<Sprite> sprites;
    private async void AnimateSprites()
    {
        for(int i = 0; i < sprites.Count; i++)
        {
            await Task.Delay(100);
            GetComponent<SpriteRenderer>().sprite = sprites[i];
        }
        Destroy(gameObject);
    }
    private void Awake()
    {
        AnimateSprites();
    }
}
