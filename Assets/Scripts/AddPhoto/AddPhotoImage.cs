using System;
using UnityEngine;
using UnityEngine.UI;

public class AddPhotoImage : MonoBehaviour
{
    [SerializeField] private Button _deleteButton;
    [SerializeField] private Image _image;
    [SerializeField] private ImagePicker _imagePicker;
    
    public event Action<AddPhotoImage> DeleteButtonClicked;

    public Image Image => _image;
    public bool IsActive { get; private set; }

    public ImagePicker ImagePicker => _imagePicker;

    private void OnEnable()
    {
        _deleteButton.onClick.AddListener(OnDeleteButtonClicked);
    }

    private void OnDisable()
    {
        _deleteButton.onClick.RemoveListener(OnDeleteButtonClicked);
    }

    public void Enable()
    {
        gameObject.SetActive(true);
        IsActive = true;
    }

    public void Disable()
    {
        gameObject.SetActive(false);
        IsActive = false;
    }

    private void OnDeleteButtonClicked()
    {
        _image.sprite = null;
        _deleteButton.onClick.RemoveListener(OnDeleteButtonClicked);
        DeleteButtonClicked?.Invoke(this);
    }
}