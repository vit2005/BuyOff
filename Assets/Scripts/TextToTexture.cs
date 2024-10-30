using UnityEngine;
using TMPro;
using System.Collections;

public class TextToTexture : MonoBehaviour
{
    public TextMeshProUGUI tmpText;      // TextMeshPro ��'���
    public Camera uiCamera;              // ������, ��� ��������� Canvas
    public RenderTexture renderTexture;  // RenderTexture, � ���� ��������� �����
    public int textureWidth = 1920;      // ������ ��������
    public int textureHeight = 1080;      // ������ ��������
    public ParticleSystem particleSystem; // Particle System

    private bool _coroutine;

    void Start()
    {
        // ��������� RenderTexture � ����������� �����������
        renderTexture = new RenderTexture(textureWidth, textureHeight, 24);

        // ����'����� RenderTexture �� ������
        uiCamera.targetTexture = renderTexture;
    }

    IEnumerator RenderTextToSprite()
    {
        // ���� ����� ���������
        yield return new WaitForEndOfFrame();

        // ��������� ����� Texture2D � ������ � ��������, �� � RenderTexture
        Texture2D textTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);

        // ������ �������� RenderTexture, ��� ���� ��������� ��������� ���
        RenderTexture.active = renderTexture;

        // ������� ����� � RenderTexture � Texture2D
        textTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        textTexture.Apply();

        // ��������� RenderTexture
        RenderTexture.active = null;

        // ��������� Sprite � Texture2D
        Sprite textSprite = Sprite.Create(textTexture, new Rect(0, 0, textTexture.width, textTexture.height), new Vector2(0.5f, 0.5f));

        // ������ Sprite �� Shape Module ParticleSystem
        ApplySpriteToShape(textSprite);

        yield return new WaitForEndOfFrame();
        _coroutine = false;
    }

    private void Update()
    {
        if (!_coroutine)
        {
            _coroutine = true;
            StartCoroutine(RenderTextToSprite());
        }
    }

    void ApplySpriteToShape(Sprite textSprite)
    {
        // �������� ������ �� Shape Module
        var shape = particleSystem.shape;

        // �������� ������������ �������� � Shape Module
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sprite;  // �������� ��� ����� - Texture
        //shape.texture = textSprite.texture;  // ���������� �������� � ������� � ������ Shape
        shape.sprite = textSprite;
        // ��������� ParticleSystem
        particleSystem.Play();
    }
}
