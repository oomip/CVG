using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundColor : MonoBehaviour
{
    public QuakeSpell quakeSpell;
    public Color quakingColor;
    public Color notQuakingColor;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().color = quakingColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (quakeSpell.isQuakeActive()) {
            GetComponent<SpriteRenderer>().color = quakingColor;
        } else {
            GetComponent<SpriteRenderer>().color = notQuakingColor;
        }
    }
}
