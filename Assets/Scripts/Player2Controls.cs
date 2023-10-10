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
  private Vector2 facingDirection = Vector2.left;
  public GridManager gridManager;

  // Update is called once per frame
  private void Update() {

    // if player is on DeathTile, die!!
    if (gridManager.GetTileAtPosition(transform.position) is DeathTile) {
      Destroy(gameObject);
    }

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
        facingDirection = Vector2.up;
        StartCoroutine(Move(facingDirection));
      } else if (inputFunction(KeyCode.N)) {
        facingDirection = Vector2.down;
        StartCoroutine(Move(facingDirection));
      } else if (inputFunction(KeyCode.G)) {
        facingDirection = Vector2.left;
        StartCoroutine(Move(facingDirection));
      } else if (inputFunction(KeyCode.J)) {
        facingDirection = Vector2.right;
        StartCoroutine(Move(facingDirection));
      } else if (inputFunction(KeyCode.T)) {
        facingDirection = Vector2.up + Vector2.left;
        StartCoroutine(Move(facingDirection));
      } else if (inputFunction(KeyCode.U)) {
        facingDirection = Vector2.up + Vector2.right;
        StartCoroutine(Move(facingDirection));
      } else if (inputFunction(KeyCode.B)) {
        facingDirection = Vector2.down + Vector2.left;
        StartCoroutine(Move(facingDirection));
      } else if (inputFunction(KeyCode.M)) {
        facingDirection = Vector2.down + Vector2.right;
        StartCoroutine(Move(facingDirection));
      } else if (inputFunction(KeyCode.H)) {
        gridManager.GenerateDeathTile((Vector2) transform.position + facingDirection);
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

    // if player moves onto DeathTile, die!!
    if (gridManager.GetTileAtPosition(endPosition) is DeathTile) {
      Destroy(gameObject);
    }

    // if player moves diagonally against a wall, move ordinally
    if (gridManager.GetTileAtPosition(endPosition) == null) {
      // left wall
      if (gridManager.GetTileAtPosition(endPosition + (Vector2.right * gridSize))) {
        endPosition = endPosition + (Vector2.right * gridSize);
      }
      // right wall
      else if (gridManager.GetTileAtPosition(endPosition + Vector2.left*gridSize)) {
        endPosition = endPosition + (Vector2.left * gridSize);
      }
      // bottom wall
      else if (gridManager.GetTileAtPosition(endPosition + Vector2.up*gridSize)) {
        endPosition = endPosition + (Vector2.up * gridSize);
      }
      // top wall
      else if (gridManager.GetTileAtPosition(endPosition + Vector2.down*gridSize)) {
        endPosition = endPosition + (Vector2.down * gridSize);
      }
    }

    // keep player within bounds
    if (gridManager.GetTileAtPosition(endPosition) != null) {
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
    }


    // We're no longer moving so we can accept another move input.
    isMoving = false;
  }
}
