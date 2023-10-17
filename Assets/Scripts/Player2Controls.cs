using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// adapted from https://github.com/Firnox/GridMovement/blob/main/Assets/Scripts/GridMovement.cs
public class Player2Controls : MonoBehaviour {
  // Allows you to hold down a key for movement.
  // Time in seconds to move between one grid position and the next.
  [SerializeField] private float moveDuration = 0.1f;
  // The size of the grid
  [SerializeField] private float gridSize = 1f;
  public float moveDelay = 0.2f;
  public int score = 0;
  public PlayerControls player1;
  private bool isMoving = false;
  public static Vector2 facingDirectionP2 = Vector2.left;
  public Vector2 startingLocation;
  public QuakeSpell quakeSpell;
  public GridManager gridManager;
  // Scheduling
  private float nextActionTime = 0.0f;
  public float period = 3f;


  // // Spells
  private static bool glideSpell = false;
  private static bool gustSpell = false;
  private static bool castawaySpell = false;
  private static bool teleportOpponentSpell = false;
  public List<bool> spells = new List<bool>(){glideSpell};



  // Update is called once per frame
  private void Update() {
    // movement/attack inputs
    float horizontalInput = Input.GetAxis("Horizontal2");
    float verticalInput = Input.GetAxis("Vertical2");
    float attackInput = Input.GetAxis("Fire2");
    float threshold = 0.25f;

    bool moveLeft = horizontalInput < -threshold;
    bool moveRight = horizontalInput > threshold;
    bool moveUp = verticalInput > threshold;
    bool moveDown = verticalInput < -threshold;

    // // collect spells periodically
    if (Time.time > nextActionTime ) {
      nextActionTime += period;

      // // don't give more than 2 spells
      int num_spells = 0;
      for (int i = 0; i < spells.Count; i++) {
        num_spells += 1;
      }
      // if (num_spells < 2) {
      //   switch (Random.Range(0,3)) {
      //     case (0):
      //       glideSpell = !glideSpell;
      //       break;
      //     case (1):
      //       gustSpell = true;
      //       break;
      //     case (2):
      //       castawaySpell = true;
      //       break;
      //     case (3):
      //       teleportOpponentSpell = true;
      //       break;
      //   }
      // }

    }

    // if player is on DeathTile, die!!
    // Doesn't die if glide is active
    if (gridManager.GetTileAtPosition(transform.position) is DeathTile && !glideSpell) {
      gridManager.ClearDeathTiles();
      player1.increment_score();
      player1.reset_position();
      transform.position = startingLocation;
    }

    // Gust Spell
    //if (GustSpell.gustActive == true)
    //{
    //    StartCoroutine(Move(facingDirectionP2));
    //}

    // Only process one move at a time.
    if (!isMoving) {
      if (moveUp || moveDown || moveLeft || moveRight)
      {
        facingDirectionP2 = Vector2.zero;
        if (moveUp) { facingDirectionP2 += Vector2.up; }
        if (moveDown) { facingDirectionP2 += Vector2.down; }
        if (moveLeft) { facingDirectionP2 += Vector2.left; }
        if (moveRight) { facingDirectionP2 += Vector2.right; }
        StartCoroutine(Move(facingDirectionP2));
      }
      else if (attackInput > threshold)
      {

          if (quakeSpell.isQuakeActive())
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
            gridManager.GenerateDeathTile((Vector2)transform.position + facingDirectionP2);
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
    new WaitForSeconds(moveDelay);
  }
  void OnGUI()
  {
      GUI.Label(new Rect(900, 10, 1000, 20), $"Radish Score: {score}");
      GUI.Label(new Rect(400, 10, 500, 20), $"Quaking: {quakeSpell.isQuakeActive()}");
  }

}
