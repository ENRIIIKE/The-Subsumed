using UnityEngine;

public class MeshCut : Interactable
{
    [SerializeField] private GameObject _cutGameObject;
    [SerializeField] private GameObject _openGameObject;

    [SerializeField] private BoxCollider _collider;

    public void Cut()
    {
        if (_showLog) Debug.Log("Changing mesh!");

        _openGameObject.SetActive(true);
        _cutGameObject.SetActive(false);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, _collider.size);
    }
}
