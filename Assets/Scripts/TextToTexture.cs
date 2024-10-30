using UnityEngine;
using TMPro;
using System.Collections;

public class TextToTexture : MonoBehaviour
{
    public TextMeshProUGUI tmpText;      // TextMeshPro об'єкт
    public Camera uiCamera;              // Камера, яка рендерить Canvas
    public RenderTexture renderTexture;  // RenderTexture, в який рендеримо текст
    public int textureWidth = 1920;      // Ширина текстури
    public int textureHeight = 1080;      // Висота текстури
    public ParticleSystem particleSystem; // Particle System

    private bool _coroutine;

    void Start()
    {
        // Створюємо RenderTexture з необхідними параметрами
        renderTexture = new RenderTexture(textureWidth, textureHeight, 24);

        // Прив'язуємо RenderTexture до камери
        uiCamera.targetTexture = renderTexture;
    }

    IEnumerator RenderTextToSprite()
    {
        // Даємо кадру оновитися
        yield return new WaitForEndOfFrame();

        // Створюємо новий Texture2D з такими ж розмірами, як і RenderTexture
        Texture2D textTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);

        // Робимо активним RenderTexture, щоб мати можливість скопіювати дані
        RenderTexture.active = renderTexture;

        // Копіюємо пікселі з RenderTexture в Texture2D
        textTexture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        textTexture.Apply();

        // Звільняємо RenderTexture
        RenderTexture.active = null;

        // Створюємо Sprite з Texture2D
        Sprite textSprite = Sprite.Create(textTexture, new Rect(0, 0, textTexture.width, textTexture.height), new Vector2(0.5f, 0.5f));

        // Додаємо Sprite до Shape Module ParticleSystem
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
        // Отримуємо доступ до Shape Module
        var shape = particleSystem.shape;

        // Увімкнути використання текстури у Shape Module
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sprite;  // Вибираємо тип форми - Texture
        //shape.texture = textSprite.texture;  // Призначаємо текстуру з спрайта в модуль Shape
        shape.sprite = textSprite;
        // Запускаємо ParticleSystem
        particleSystem.Play();
    }
}
