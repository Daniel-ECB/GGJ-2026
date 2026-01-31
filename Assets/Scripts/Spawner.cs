using UnityEngine;
using System.Collections;

namespace GGJ2026.Gameplay
{
    public class BlockSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject blockPrefab;
        [SerializeField] private int numberOfBlocks = 20; 
        [SerializeField] private float startZ = 5f;      
        [SerializeField] private float stepZ = 5f;      
        [SerializeField] private float fixedY = 0.5f;     
        [SerializeField] private float spawnInterval = 1.0f; 

        
        private readonly float[] allowedXPositions = { -3f, -1f, 1f, 3f };

        private void Start()
        {
            StartCoroutine(SpawnBlocksRoutine());
        }

        private IEnumerator SpawnBlocksRoutine()
        {
            for (int i = 0; i < numberOfBlocks; i++)
            {
                
                float zPos = startZ + (i * stepZ);

                
                float xPos = allowedXPositions[Random.Range(0, allowedXPositions.Length)];

                Vector3 spawnPos = new Vector3(xPos, fixedY, zPos);

               
                GameObject blockObj = Instantiate(blockPrefab, spawnPos, Quaternion.identity);

              
                CarnivalBlock carnivalBlock = blockObj.GetComponent<CarnivalBlock>();
                if (carnivalBlock != null)
                {
                    MaskColors randomColor = (MaskColors)Random.Range(0, 4);
                    carnivalBlock.InitializeBlock(randomColor, CarnivalBlock.BlockState.InTransit);
                }

               
                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }


}

