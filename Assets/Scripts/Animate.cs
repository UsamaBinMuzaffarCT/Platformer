using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Animate : MonoBehaviour
{
    #region variables

    [SerializeField] private List<Sprite> sprites;
    private bool running = true;
    public int delay = 100;

    #endregion

    private async void ChangeSprite()
    {
        int i = 0;
        while (running)
        {
            GetComponent<SpriteRenderer>().sprite = sprites[i];
            i++;
            if(i == sprites.Count)
            {
                i = 0;
            }
            await Task.Delay(delay);
        }
    }

    private void Start()
    {
        ChangeSprite();
    }
    private void OnDestroy()
    {
        running = false;
    }
}
