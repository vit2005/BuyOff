using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraShake : MonoBehaviour
{
    [Header("Bump Settings")]
    public List<AnimationCurve> bumpCurves;
    public float minRandomTime = 1f;
    public float maxRandomTime = 3f;

    [Header("Turn Settings")]
    public AnimationCurve turnLeft;
    public AnimationCurve turnRight;
    public float minRandomTurnTime = 2f;
    public float maxRandomTurnTime = 5f;

    private float nextBumpTime;
    private float nextTurnTime;
    private float bumpDuration;
    private float turnDuration;
    private float startTimeBump;
    private float startTimeTurn;
    private Vector3 originalPosition;
    private bool stopShaking = false; // Змінна для зупинки шейку

    private float currentYOffset = 0f;
    private float currentXOffset = 0f;

    void Start()
    {
        originalPosition = transform.localPosition;
        ScheduleNextBump();
        ScheduleNextTurn();
    }

    void Update()
    {
        if (stopShaking) return; // Якщо шейк зупинено, не виконується оновлення

        float currentTime = Time.time;

        // Handle bump animation
        if (currentTime >= nextBumpTime)
        {
            int curveIndex = Random.Range(0, bumpCurves.Count);
            bumpDuration = bumpCurves[curveIndex].keys[bumpCurves[curveIndex].keys.Length - 1].time;
            StartCoroutine(ShakeCameraY(bumpCurves[curveIndex]));
            ScheduleNextBump();
        }

        // Handle turn animation
        if (currentTime >= nextTurnTime)
        {
            bool isTurningLeft = Random.value > 0.5f;
            AnimationCurve chosenCurve = isTurningLeft ? turnLeft : turnRight;
            turnDuration = chosenCurve.keys[chosenCurve.keys.Length - 1].time;
            StartCoroutine(ShakeCameraX(chosenCurve));
            ScheduleNextTurn();
        }

        // Оновлення позиції камери з накладеними ефектами
        transform.localPosition = originalPosition + new Vector3(currentXOffset, currentYOffset, 0);
    }

    private void ScheduleNextBump()
    {
        nextBumpTime = Time.time + Random.Range(minRandomTime, maxRandomTime);
    }

    private void ScheduleNextTurn()
    {
        nextTurnTime = Time.time + Random.Range(minRandomTurnTime, maxRandomTurnTime);
    }

    private IEnumerator ShakeCameraY(AnimationCurve curve)
    {
        startTimeBump = Time.time;
        while (Time.time - startTimeBump < bumpDuration && !stopShaking)
        {
            float elapsed = Time.time - startTimeBump;
            currentYOffset = curve.Evaluate(elapsed);
            yield return null;
        }
        currentYOffset = 0f; // Скидання після завершення ефекту
    }

    private IEnumerator ShakeCameraX(AnimationCurve curve)
    {
        startTimeTurn = Time.time;
        while (Time.time - startTimeTurn < turnDuration && !stopShaking)
        {
            float elapsed = Time.time - startTimeTurn;
            currentXOffset = curve.Evaluate(elapsed);
            yield return null;
        }
        currentXOffset = 0f; // Скидання після завершення ефекту
    }

    public void StopCameraShake()
    {
        stopShaking = true;
        StopAllCoroutines();
        currentYOffset = 0f;
        currentXOffset = 0f;
        transform.localPosition = originalPosition;
    }

    public void ResumeCameraShake()
    {
        stopShaking = false;
        ScheduleNextBump();
        ScheduleNextTurn();
    }
}
