using UnityEngine;

public class TowerRangeDisplay : MonoBehaviour {
    [Tooltip("Reference to the RangeCircle SpriteRenderer (child object).")]
    public SpriteRenderer rangeCircle;

    [Tooltip("Current range (radius) of the tower.")]
    public float range = 5f;

    // Fixed alpha value for the range circle when visible (5% opacity).
    private float fixedRangeAlpha = 0.05f;

    // Tower state flags.
    [SerializeField] // Makes it visible in the inspector (optional)
    private bool isPlaced = false;     // Has the tower been placed?
    private bool isSelected = false;   // Is the tower selected (to show its range) after placement?
    private bool isBeingPlaced = false;   // Is the tower currently being dragged?

    // Reference to the tower's own SpriteRenderer.
    private SpriteRenderer towerSprite;

    // Offset between the mouse position and tower center during drag.
    private Vector3 offset;

    void Start() {
        towerSprite = GetComponent<SpriteRenderer>();
        if (towerSprite == null) {
            Debug.LogError("Tower SpriteRenderer is not found on " + gameObject.name);
        }
        if (rangeCircle == null) {
            Debug.LogError("RangeCircle SpriteRenderer is not assigned!");
            return;
        }
        rangeCircle.transform.localScale = new Vector3(range * 1, range * 1, 1);
        SetRangeCircleAlpha(0f);
        SetTowerAlpha(0.25f);
    }

    void Update() {
        rangeCircle.transform.localScale = new Vector3(range * 1, range * 1, 1);
        
        // Follow mouse when being placed
        if (isBeingPlaced) {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            transform.position = mousePosition;
            SetRangeCircleAlpha(fixedRangeAlpha);
        } else if (isPlaced) {
            SetRangeCircleAlpha(isSelected ? fixedRangeAlpha : 0f);
        }
    }

    void OnMouseDown() {
        if (!isPlaced) {
            if (!isBeingPlaced) {
                isBeingPlaced = true;
                SetRangeCircleAlpha(fixedRangeAlpha);
            } else {
                isBeingPlaced = false;
                isPlaced = true;
                SetTowerAlpha(1f);
                SetRangeCircleAlpha(0f);
            }
        } else {
            isSelected = !isSelected;
        }
    }

    // Expose the isPlaced state via a public property.
    public bool IsPlaced {
        get { return isPlaced; }
    }

    void SetRangeCircleAlpha(float alpha) {
        Color newColor = rangeCircle.color;
        newColor.a = alpha;
        rangeCircle.color = newColor;
    }

    void SetTowerAlpha(float alpha) {
        if (towerSprite != null) {
            Color newColor = towerSprite.color;
            newColor.a = alpha;
            towerSprite.color = newColor;
        }
    }
}
