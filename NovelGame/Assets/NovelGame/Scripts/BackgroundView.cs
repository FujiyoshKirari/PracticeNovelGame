using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BackgroundView : MonoBehaviour
{
    [SerializeField] private Image _image;

    private void Reset()
    {
        _image = GetComponent<Image>();
    }

    public void SetImage(Sprite sprite)
    {
        _image.sprite = sprite;
    }
}
