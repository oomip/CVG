using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// adapted from https://github.com/Firnox/GridMovement/blob/main/Assets/Scripts/GridMovement.cs
public class Player2Controls : MonoBehaviour {
  // Allows you to hold down a key for movement.
  [SerializeField] private bool isRepeatedMovement = false;
  // Time in seconds to move between one grid position and the next.
  [SerializeField] private float moveDuration = 0.1f;
  // The size of the grid
  [SerializeField] private float gridSize = 1f;

  private bool isMoving = false;

  // Update is called once per frame
  private void Update() {
    // Only process on move at a time.
    if (!isMoving) {
      // Accomodate two different types of moving.
      System.Func<KeyCode, bool> inputFunction;
      if (isRepeatedMovement) {
        // GetKey repeatedly fires.
        inputFunction = Input.GetKey;
      } else {
        // GetKeyDown fires once per keypress
        inputFunction = Input.GetKeyDown;
      }

      // If the input function is active, move in the appropriate direction.
      if (inputFunction(KeyCode.Y)) {
        StartCoroutine(Move(Vector2.up));
      } else if (inputFunction(KeyCode.N)) {
        StartCoroutine(Move(Vector2.down));
      } else if (inputFunction(KeyCode.G)) {
        StartCoroutine(Move(Vector2.left));
      } else if (inputFunction(KeyCode.J)) {
        StartCoroutine(Move(Vector2.right));
      } else if (inputFunction(KeyCode.T)) {
        StartCoroutine(Move(Vector2.up + Vector2.left));
      } else if (inputFunction(KeyCode.U)) {
        StartCoroutine(Move(Vector2.up + Vector2.right));
      } else if (inputFunction(KeyCode.B)) {
        StartCoroutine(Move(Vector2.down + Vector2.left));
      } else if (inputFunction(KeyCode.M)) {
        StartCoroutine(Move(Vector2.down + Vector2.right));
      }
    }
  }

  // Smooth movement between grid positions.
  private IEnumerator Move(Vector2 direction) {
    // Record that we're moving so we don't accept more input.
    isMoving = true;

    // Make a note of where we are and where we are going.
    Vector2 startPosition = transform.position;
    Vector2 endPosition = startPosition + (direction * gridSize);

    // Smoothly move in the desired direction taking the required time.
    float elapsedTime = 0;
    while (elapsedTime < moveDuration) {
      elapsedTime += Time.deltaTime;
      float percent = elapsedTime / moveDuration;
      transform.position = Vector2.Lerp(startPosition, endPosition, percent);
      yield return null;
    }

    // Make sure we end up exactly where we want.
    transform.position = endPosition;

    // We're no longer moving so we can accept another move input.
    isMoving = false;
  }
}

