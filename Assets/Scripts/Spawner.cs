using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GGJ2026.Gameplay
{
    [System.Serializable]
    public class PauseConfig
    {
        public int blocksBeforePause;
        public float pauseDuration;
    }

    public class Spawner : MonoBehaviour
    {
        [Header("Bloques")]
        [SerializeField] private GameObject blockPrefab;
        [SerializeField] private int numberOfBlocks = 20;
        [SerializeField] private float startZ = 5f;
        [SerializeField] private float stepZ = 5f;
        [SerializeField] private float fixedY = 0.5f;
        [SerializeField] private float spawnInterval = 0.75f;

        [Header("Jugador")]
        [SerializeField] private Transform player;

        [Header("Pausas configurables")]
        [SerializeField] private List<PauseConfig> pausePattern = new List<PauseConfig>();

        private readonly float[] allowedXPositions = { -3f, -1f, 1f, 3f };

        public bool FinishedSpawning { get; private set; } = false;
        public int NumberOfBlocks => numberOfBlocks;

        private void Start()
        {
            StartCoroutine(SpawnBlocksRoutine());
        }

        private IEnumerator SpawnBlocksRoutine()
        {
            int blocksSpawned = 0;
            int pauseIndex = 0;

            while (blocksSpawned < numberOfBlocks)
            {
                float zPos = startZ + (blocksSpawned * stepZ);

                if (pauseIndex > 0 && player != null && zPos < player.position.z + 5f)
                {
                    zPos = player.position.z + 10f + (blocksSpawned * stepZ);
                }

                float xPos = allowedXPositions[Random.Range(0, allowedXPositions.Length)];
                Vector3 spawnPos = new Vector3(xPos, fixedY, zPos);

                GameObject blockObj = Instantiate(blockPrefab, spawnPos, Quaternion.identity);

                CarnivalBlock carnivalBlock = blockObj.GetComponent<CarnivalBlock>();
                if (carnivalBlock != null)
                {
                    MaskColors randomColor = (MaskColors)Random.Range(0, 4);
                    carnivalBlock.InitializeBlock(randomColor, CarnivalBlock.BlockState.InTransit);
                }

                blocksSpawned++;
                yield return new WaitForSeconds(spawnInterval);

                if (pauseIndex < pausePattern.Count && blocksSpawned >= pausePattern[pauseIndex].blocksBeforePause)
                {
                    Debug.Log($"Spawner en pausa por {pausePattern[pauseIndex].pauseDuration} segundos...");
                    yield return new WaitForSeconds(pausePattern[pauseIndex].pauseDuration);
                    pauseIndex++;
                }
            }

            FinishedSpawning = true;
        }
    }
}