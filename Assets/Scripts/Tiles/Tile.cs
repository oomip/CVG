// from https://www.youtube.com/watch?v=kkAjpQAM-jE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class Tile : MonoBehaviour {
    public string TileName;
    [SerializeField] protected SpriteRenderer _renderer;

    [SerializeField] private bool _isDeadly;
    public virtual void Init(int x, int y) {}
}
