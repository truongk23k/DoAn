using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private Transform lastLevelPart;
    [SerializeField] private List<Transform> levelParts;
    private List<Transform> currentLevelParts;
    [SerializeField] private SnapPoint nextSnapPoints;

    [Space]
    [SerializeField] private float generationCooldown;
    private float cooldownTimer;
    private bool generationOver;

    private void Start()
    {
        currentLevelParts = new List<Transform>(levelParts);
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

    private void FinishGeneration()
    {
        generationOver = true;

        Transform levelPart = Instantiate(lastLevelPart);
        LevelPart levelPartScript = levelPart.GetComponent<LevelPart>();
        levelPartScript.SnapAndAlignPartTo(nextSnapPoints);
    }

    [ContextMenu("Create Next Level Part")]
    private void GenerateNextLevelPart()
    {
        Transform newPart = Instantiate(ChooseRandomPart());

        LevelPart levelPartScript = newPart.GetComponent<LevelPart>();
        levelPartScript.SnapAndAlignPartTo(nextSnapPoints);
        nextSnapPoints = levelPartScript.GetExitSnapPoint();
    }

    private Transform ChooseRandomPart()
    {
        int randomIndex = Random.Range(0, currentLevelParts.Count);
        Transform chosenPart = currentLevelParts[randomIndex];
        currentLevelParts.RemoveAt(randomIndex);
        return chosenPart;
    }
}
