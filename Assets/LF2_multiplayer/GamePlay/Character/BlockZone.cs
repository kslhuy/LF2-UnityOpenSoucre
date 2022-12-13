using UnityEngine;
using LF2.Client;

public class BlockZone : MonoBehaviour {
    [SerializeField] BoxCollider boxCollider;
    private void Start() {
        boxCollider.gameObject.SetActive(false);
    }

    
}