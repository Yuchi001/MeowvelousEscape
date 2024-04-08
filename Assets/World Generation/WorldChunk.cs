using NaughtyAttributes;
using UnityEngine;

public class WorldChunk : MonoBehaviour 
{
    public event System.Action<bool> OnVisibilityChanged;

    [SerializeField]
    private Transform wall;

    private void Awake()
    {
        SetVisible(IsVisible);
        if (wall && Random.value < 0.5f)
            wall.localPosition = -wall.localPosition;
    }

    [ShowNonSerializedField, ReadOnly]
    private bool isVisible;
    public bool IsVisible
    {
        get => isVisible;
        set
        {
            if (isVisible != value)
                SetVisible(value);
        }
    }

    private void SetVisible(bool value)
    {
        isVisible = value;
        if (isVisible && gameObject.activeSelf == false)
        {
            gameObject.SetActive(true);
            OnVisibilityChanged?.Invoke(true);
        }
    }

    private void LateUpdate()
    {
        if (isVisible == false)
        {
            gameObject.SetActive(false);
            OnVisibilityChanged?.Invoke(false);
        }
    }
}
