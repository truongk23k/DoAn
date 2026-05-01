using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private Transform lastLevelPart;
    [SerializeField] private List<Transform> levelParts;
    private List<Transform> currentLevelParts;
    private List<Transform> generatedLevelParts = new List<Transform>();
    [SerializeField] private SnapPoint nextSnapPoint;
    private SnapPoint defaultSnapPoint;

    [Space]
    [SerializeField] private float generationCooldown;
    private float cooldownTimer;
    private bool generationOver;

    private void Start()
    {
        defaultSnapPoint = nextSnapPoint;
        InitializedGeneration();
    }

    private void Update()
    {
        if (generationOver)
            return;

        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0)
        {
            if (currentLevelParts.Count > 0)
            {
                GenerateNextLevelPart();
                cooldownTimer = generationCooldown;
            }
            else if (!generationOver)
            {
                FinishGeneration();
            }
        }
    }

    [ContextMenu("Restart Generation")]
    private void InitializedGeneration()
    {
        nextSnapPoint = defaultSnapPoint;
        generationOver = false;
        currentLevelParts = new List<Transform>(levelParts);

        DestroyOldLevelParts();
    }

    private void DestroyOldLevelParts()
    {
        foreach (Transform t in generatedLevelParts)
        {
            Destroy(t.gameObject);
        }
        generatedLevelParts = new List<Transform>();
    }

    private void FinishGeneration()
    {
        generationOver = true;

        GenerateNextLevelPart();
    }

    [ContextMenu("Create Next Level Part")]
    private void GenerateNextLevelPart()
    {
        Transform newPart = null;

        if (generationOver)
            newPart = Instantiate(lastLevelPart);
        else
            newPart = Instantiate(ChooseRandomPart());

        generatedLevelParts.Add(newPart);

        LevelPart levelPartScript = newPart.GetComponent<LevelPart>();
        levelPartScript.SnapAndAlignPartTo(nextSnapPoint);

        if (levelPartScript.IntersectionDetected())
        {
            InitializedGeneration();
            return;
        }

        nextSnapPoint = levelPartScript.GetExitSnapPoint();
    }

    private Transform ChooseRandomPart()
    {
        int randomIndex = Random.Range(0, currentLevelParts.Count);
        Transform chosenPart = currentLevelParts[randomIndex];
        currentLevelParts.RemoveAt(randomIndex);
        return chosenPart;
    }
}
