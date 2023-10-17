// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class BackgroundColor : MonoBehaviour
// {
//     public QuakeSpell quakeSpell;
//     public Color quakingColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
//     public Color notQuakingColor = new Color(0.5f, 0.5f, 0.5f, 0.0f);
//     private SpriteRenderer background = GetComponent<SpriteRenderer>();

//     // Start is called before the first frame update
//     void Start()
//     {
//         background.color.a = quakingColor;
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         if (quakeSpell.isQuakeActive()) {
//             background.color.a = quakingColor;
//         } else {
//             background.color.a = notQuakingColor;
//         }
//     }
// }
