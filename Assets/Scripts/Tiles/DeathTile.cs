using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTile : Tile
{
    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        // _audioSource.Play();
    }
}
