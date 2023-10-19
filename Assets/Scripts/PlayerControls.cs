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
  public PlayerControls otherPlayer;
  private bool isMoving = false;
  private bool isCastingSpell = false;
  public Vector2 facingDirection = Vector2.left;
  public Vector2 startingLocation;
  public QuakeSpell quakeSpell;
  public GridManager gridManager;
  // Scheduling
  private float nextActionTime = 0.0f;
  public float period = 3f;
  public string horizAxis;
  public string vertAxis;
  public string attackAxis;
  public string spell1Axis;
  public string spell2Axis;
  public Color movingColor;
  public Color notMovingColor;
  public int playerNumber;
  private AudioSource destroySound;



  // Spells
  public Dictionary<string, bool> spells = new Dictionary<string, bool>(){
    {"Glide", false},
    {"Gust", false},
    {"Dash", false},
    {"Castaway", false}
  };
  private float glidingTime = 0f;
  private float dashingTime = 0f;
  public List<string> availableSpells = new List<string>();

  private void Start() {
    destroySound = GetComponent<AudioSource>();
  }


  // Update is called once per frame
  private void Update() {
    // movement/attack inputs
    float horizontalInput = Input.GetAxis(horizAxis);
    float verticalInput = Input.GetAxis(vertAxis);
    float attackInput = Input.GetAxis(attackAxis);
    float spell1Input = Input.GetAxis(spell1Axis);
    float spell2Input = Input.GetAxis(spell2Axis);
    float threshold = 0.25f;

    bool moveLeft = horizontalInput < -threshold;
    bool moveRight = horizontalInput > threshold;
    bool moveUp = verticalInput > threshold;
    bool moveDown = verticalInput < -threshold;
    bool spell1 = spell1Input > threshold;
    bool spell2 = spell2Input > threshold;

    // collect spells periodically
    if (Time.time > nextActionTime ) {
      nextActionTime += period;

      // don't give more than 2 spells
      if (availableSpells.Count < 2) {
        switch (Random.Range(0,6)) {
          case (0):
            if (!spells["Glide"]) {
              spells["Glide"] = true;
              availableSpells.Add("Glide");
            }
            break;
          case (1):
            if (!spells["Gust"]) {
              spells["Gust"] = true;
              availableSpells.Add("Gust");
            }
            break;
          case (2):
            if (!spells["Dash"]) {
              spells["Dash"] = true;
              availableSpells.Add("Dash");
            }
            break;
          case (3):
            if (!spells["Castaway"]) {
              spells["Castaway"] =  true;
              availableSpells.Add("Castaway");
            }
            break;
          default:
            break;
        }
      }
      if (glidingTime > 0) {
        glidingTime -= 1;
      } else if (dashingTime > 0) {
        dashingTime -= 1;
      } else {
        isCastingSpell = false;
        moveDelay = 0.2f;
      }
    }

    // If player is on DeathTile (and not Gliding), die!!
    if (gridManager.GetTileAtPosition(transform.position) is DeathTile && glidingTime <= 0) {
      gridManager.ClearDeathTiles();
      otherPlayer.incrementScore();
      resetPosition();
      otherPlayer.resetPosition();
      transform.position = startingLocation;
    }

    // Moving. Only process one move at a time.
    if (!isMoving) {
      if (moveUp || moveDown || moveLeft || moveRight)
      {
        facingDirection = Vector2.zero;
        if (moveUp) { facingDirection += Vector2.up; }
        if (moveDown) { facingDirection += Vector2.down; }
        if (moveLeft) { facingDirection += Vector2.left; }
        if (moveRight) { facingDirection += Vector2.right; }
        StartCoroutine(Move(facingDirection));
      }
      // Attacking.
      if (attackInput > threshold) {
        // Sound
        destroySound.Play();

        // when Quaking, add 2 random ordinally adjacent deathTiles
        if (quakeSpell.isQuakeActive()) {
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
        // when not Quaking, add deathTile at tile they're facing
        } else {
          gridManager.GenerateDeathTile((Vector2)transform.position + facingDirection);
          StartCoroutine(Move(Vector2.zero));
        }
      }
    }

    // Spell activation.
    string activeSpell = "none";
    if (spell1 && availableSpells.Count > 0) {
      activeSpell = availableSpells[0];
      availableSpells.RemoveAt(0);
    } else if (spell2 && availableSpells.Count > 1) {
      activeSpell = availableSpells[1];
      availableSpells.RemoveAt(1);
    }
    if (activeSpell != null && !isCastingSpell) {
      StartCoroutine(Cast(activeSpell));
    }
  }

  // Smooth movement between grid positions.
  private IEnumerator Move(Vector2 direction) {
    // Record that we're moving so we don't accept more input.
    isMoving = true;
    GetComponent<SpriteRenderer>().color = movingColor;

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
    GetComponent<SpriteRenderer>().color = notMovingColor;
  }


  private IEnumerator Cast(string activeSpell) {
    spells[activeSpell] = false;

    if (isCastingSpell) {
      yield return new WaitForSeconds(0.2f);
    }
    // Glide Spell
    if (activeSpell == "Glide") {
      glidingTime = 2;
      isCastingSpell = true;
    }
    // Gust Spell
    if (activeSpell == "Gust") {
      otherPlayer.pushInDirection(facingDirection);
    }
    // Dash Spell
    if (activeSpell == "Dash") {
      moveDelay = 0.1f;
      dashingTime = 2;
      isCastingSpell = true;
    }
    // Castaway Spell
    if (activeSpell == "Castaway") {
      otherPlayer.moveToRandomTile();
    }
    yield return new WaitForSeconds(0.2f);
  }

  public void incrementScore() {
    this.score += 1;
  }
  public Vector2 getFacingDirection() {
    return facingDirection;
  }
  public Vector3 getPos() {
    return transform.position;
  }
  public void pushInDirection(Vector2 dir) {
    StartCoroutine(Move(dir));
  }
  public void moveToRandomTile() {
    Vector2 pos = new Vector2(UnityEngine.Random.Range(1,10), UnityEngine.Random.Range(1,6)) * gridSize;
    while (gridManager.GetTileAtPosition(pos) is DeathTile)
      pos = new Vector2(UnityEngine.Random.Range(1,10), UnityEngine.Random.Range(1,6)) * gridSize;
    Vector2 randomPos = pos;
    transform.position = randomPos;
  }
  public void resetPosition() {
    transform.position = startingLocation;
    new WaitForSeconds(moveDelay);
  }
  void OnGUI()
  {
    string spell1 = "";
    string spell2 = "";
    if (availableSpells.Count > 0) {
      spell1 = availableSpells[0];
      if (availableSpells.Count > 1) {
        spell2 = availableSpells[1];
      }
    }
    if (playerNumber == 1) {
      float x1 = 5f;
      float x2 = 100f;
      float y1 = 10f;
      float y2 = Screen.height;
      // Score
      GUI.Label(new Rect(x1, y1, x2, y2), $"Potato Score: {score}");

      // Spells GUI
      GUI.Label(new Rect(x1, y2/2-150, x2, y2/2), "Attack \n(Space)");
      GUI.Label(new Rect(x1, y2/2-100, x2, y2/2), "Spells \n(Q,E)");
      GUI.Label(new Rect(x1, y2/2, x2, y2/2+100), $"Q: {spell1}\nE: {spell2}");

      // Gliding/Dashing time
      GUI.Label(new Rect(x1, y2/2+40, x2, y2/2+150), $"Gliding {glidingTime}");
      GUI.Label(new Rect(x1, y2/2+80, x2, y2/2+160), $"Dashing {dashingTime}");

      // Quaking
      if (quakeSpell.isQuakeActive()) {
        GUI.Label(new Rect(Screen.width/2, y1, Screen.width*3/2, 40), "QUAKING!");
      }
    } else if (playerNumber == 2) {
      float x1 = Screen.width - 60;
      float x2 = Screen.width - 5;
      float y1 = 10f;
      float y2 = Screen.height;
      // Score
      GUI.Label(new Rect(x1-100, y1, x2, y2), $"Radish Score: {score}");

      // Spells GUI
      GUI.Label(new Rect(x1, y2/2-150, x2, y2/2), "Attack \n(B)");
      GUI.Label(new Rect(x1, y2/2-50, x2, y2/2), "Spells \n(A,Y)");
      GUI.Label(new Rect(x1, y2/2, x2, y2/2+100), $"A: {spell1}\nY: {spell2}");

      // Gliding/Dashing time
      GUI.Label(new Rect(x1, y2/2+40, x2, y2/2+150), $"Gliding {glidingTime}");
      GUI.Label(new Rect(x1, y2/2+80, x2, y2/2+160), $"Dashing {dashingTime}");
    }
  }

}
