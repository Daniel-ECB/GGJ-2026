using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GGJ2026.Gameplay
{
    [System.Serializable]
    public struct ManualBlockConfig
    {
        public MaskColors color;
        public BlockPosition position;
    }

    public enum BlockPosition
    {
        Left = -3,
        LeftCenter = -1,
        RightCenter = 1,
        Right = 3
    }

    [System.Serializable]
    public class ManualSection
    {
        public string sectionName = "Sección";
        public int blockCount = 4;
        public float pauseAfterSec = 2f;
        public ManualBlockConfig[] blocks;

        public void Validate()
        {
            if (blocks == null)
            {
                blocks = new ManualBlockConfig[blockCount];
                for (int i = 0; i < blockCount; i++)
                {
                    blocks[i].color = MaskColors.Red;
                    blocks[i].position = BlockPosition.LeftCenter;
                }
            }
            else if (blocks.Length != blockCount)
            {
                
                System.Array.Resize(ref blocks, blockCount);
                for (int i = 0; i < blocks.Length; i++)
                {
                    
                    if (blocks[i].color == 0 && blocks[i].position == 0)
                    {
                        blocks[i].color = MaskColors.Red;
                        blocks[i].position = BlockPosition.LeftCenter;
                    }
                }
            }
        }
    }

    public class Spawner : MonoBehaviour
    {
        [Header("Bloques Automáticos")]
        [SerializeField] private GameObject blockPrefab;
        [SerializeField] private int numberOfBlocks = 20;
        [SerializeField] private float startZ = 5f;
        [SerializeField] private float stepZ = 5f;
        [SerializeField] private float fixedY = 0.5f;
        [SerializeField] private float spawnInterval = 0.75f;

        [Header("Jugador")]
        [SerializeField] private Transform player;

        [Header("Modo Manual")]
        [SerializeField] private bool manualMode = false;
        [SerializeField] private List<ManualSection> manualSections = new List<ManualSection>();

        private readonly float[] allowedXPositions = { -3f, -1f, 1f, 3f };

        public bool FinishedSpawning { get; private set; } = false;
        public int NumberOfBlocks => manualMode ? GetManualTotalBlocks() : numberOfBlocks;

        private int GetManualTotalBlocks()
        {
            int total = 0;
            foreach (var section in manualSections)
                total += section.blockCount;
            return total;
        }

        private void OnValidate()
        {
            if (manualMode)
            {
                foreach (var section in manualSections)
                {
                    section.Validate();
                }
            }
        }

        private void Start()
        {
            StartCoroutine(SpawnBlocksRoutine());
        }

        private IEnumerator SpawnBlocksRoutine()
        {
            if (manualMode)
            {
                int globalIndex = 0;
                foreach (var section in manualSections)
                {
                    section.Validate();

                    for (int i = 0; i < section.blocks.Length; i++)
                    {
                        float zPos = startZ + (globalIndex * stepZ);
                        Vector3 spawnPos = new Vector3((int)section.blocks[i].position, fixedY, zPos);

                        GameObject blockObj = Instantiate(blockPrefab, spawnPos, Quaternion.identity);

                        CarnivalBlock carnivalBlock = blockObj.GetComponent<CarnivalBlock>();
                        if (carnivalBlock != null)
                        {
                            carnivalBlock.InitializeBlock(section.blocks[i].color, CarnivalBlock.BlockState.InTransit);
                        }

                        globalIndex++;
                        yield return new WaitForSeconds(spawnInterval);
                    }

                    
                    if (section.pauseAfterSec > 0f)
                    {
                        Debug.Log($"Pausa después de {section.sectionName} por {section.pauseAfterSec} segundos...");
                        yield return new WaitForSeconds(section.pauseAfterSec);
                    }
                }

                FinishedSpawning = true;
                yield break;
            }

            
            int blocksSpawned = 0;
            while (blocksSpawned < numberOfBlocks)
            {
                float zPos = startZ + (blocksSpawned * stepZ);
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
            }

            FinishedSpawning = true;
        }
    }
}