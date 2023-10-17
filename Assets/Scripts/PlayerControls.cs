using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// adapted from https://github.com/Firnox/GridMovement/blob/main/Assets/Scripts/GridMovement.cs
public class PlayerControls : MonoBehaviour {
  // Allows you to hold down a key for movement.
  // Time in seconds to move between one grid position and the next.
  [SerializeField] private float moveDuration = 0.1f;
  // The size of the grid
  [SerializeField] private float gridSize = 1f;
  public float moveDelay = 0.2f;
  public int score = 0;
  public Player2Controls player2;
  private bool isMoving = false;
  public static Vector2 facingDirectionP1 = Vector2.left;
  public Vector2 startingLocation;
  public GridManager gridManager;

  // Update is called once per frame
  private void Update() {
    float horizontalInput = Input.GetAxis("Horizontal");
    float verticalInput = Input.GetAxis("Vertical");
    float attackInput = Input.GetAxis("Fire");
    float threshold = 0.25f;

    bool moveLeft = horizontalInput < -threshold;
    bool moveRight = horizontalInput > threshold;
    bool moveUp = verticalInput > threshold;
    bool moveDown = verticalInput < -threshold;

    // if player is on DeathTile, die!!
    if (gridManager.GetTileAtPosition(transform.position) is DeathTile) {
      gridManager.ClearDeathTiles();
      player2.increment_score();
      player2.reset_position();
      transform.position = startingLocation;
    }


    // Gust Spell
    //if (GustSpell.gustActive == true)
    //{
    //    StartCoroutine(Move(facingDirectionP1));
    //}

    // Only process one move at a time.
    if (!isMoving) {
      if (moveUp || moveDown || moveLeft || moveRight)
      {
        facingDirectionP1 = Vector2.zero;
        if (moveUp) { facingDirectionP1 += Vector2.up; }
        if (moveDown) { facingDirectionP1 += Vector2.down; }
        if (moveLeft) { facingDirectionP1 += Vector2.left; }
        if (moveRight) { facingDirectionP1 += Vector2.right; }
        StartCoroutine(Move(facingDirectionP1));
      }
      else if (attackInput > threshold)
      {

          if (QuakeSpell.quakeActive == true)
          {
            List<Vector2> deathTiles = new List<Vector2>();
            while (deathTiles.Count < 2) {
              Vector2 nextSpot = (UnityEngine.Random.Range(0,2)-1) * Vector2.right * gridSize + (UnityEngine.Random.Range(0,2)-1) * Vector2.up * gridSize;
              if (nextSpot != Vector2.zero) {
                deathTiles.Add(nextSpot);
              }
            }
            gridManager.GenerateDeathTile((Vector2)transform.position + deathTiles[0]);
            gridManager.GenerateDeathTile((Vector2)transform.position + deathTiles[1]);
            StartCoroutine(Move(Vector2.zero));
          } else {
            gridManager.GenerateDeathTile((Vector2)transform.position + facingDirectionP1);
            StartCoroutine(Move(Vector2.zero));
          }
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

      // Delay next move
      yield return new WaitForSeconds(moveDelay);
    }


    // We're no longer moving so we can accept another move input.
    isMoving = false;
  }
  public void increment_score() {
    this.score += 1;
  }
  public void reset_position() {
    transform.position = startingLocation;
  }

  void OnGUI()
  {
      GUI.Label(new Rect(10, 10, 100, 20), $"Potato Score: {score}");
  }
}
