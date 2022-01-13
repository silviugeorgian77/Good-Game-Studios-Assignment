using UnityEngine;
using System.Collections.Generic;

public class ItemMatrixSpawner : MonoBehaviour
{
    public enum SpawnDirectionX
    {
        LEFT_TO_RIGHT,
        RIGHT_TO_LEFT
    }

    public enum SpawnDirectionY
    {
        TOP_TO_BOTTOM,
        BOTTOM_TO_TOP
    }

    public Transform spawnAreaTransform;
    public GameObject itemPrefab;
    public Transform itemsParent;

    /// <summary>
    /// The reference of this aligner will must be a GameObject that is not
    /// parented to this ItemMatrixSpawner.
    /// </summary>
    public Aligner parentAligner;

    public int itemCount = 30;
    public float marginX = 20;
    public float marginY = 0;
    public int minRows;
    public int minColumns;
    public int maxRows = int.MaxValue;
    public int maxColumns = int.MaxValue;
    public SpawnDirectionX spawnDirectionX = SpawnDirectionX.LEFT_TO_RIGHT;
    public SpawnDirectionY spawnDirectionY = SpawnDirectionY.TOP_TO_BOTTOM;

    public int Rows { get; private set; }
    public int Columns { get; private set; }
    private Vector2 spawnAreaSize;
    private Vector2 itemPrefabSize;
    private Vector2 availableItemAreaSize;
    private float availableItemAreaAspectRatio;
    private int currentItemCount;
    private float currentMarginX;
    private float currentMarginY;
    private int currentMinRows;
    private int currentMinColumns;

    public GameObject[,] SpawnedObjectsMatrix { get; private set; }
    public List<GameObject> SpawnedObjectsList { get; private set; }
        = new List<GameObject>();

    public delegate void OnItemMatrixChangedDelegate(
        ItemMatrixSpawner itemMatrixSpawner
    );
    public OnItemMatrixChangedDelegate OnItemMatrixChanged;

    private void Update()
    {
        if (itemCount < 0)
        {
            itemCount = 0;
        }
        if (minRows < 0)
        {
            minRows = 0;
        }
        if (minColumns < 0)
        {
            minColumns = 0;
        }
        if (itemCount != currentItemCount
            || marginX != currentMarginX
            || marginY != currentMarginY
            || minRows != currentMinRows
            || minColumns != currentMinColumns)
        {
            ResetMatrix();
        }
    }

    public void ResetMatrix()
    {
        ClearSpawned();
        if (itemCount > 0)
        {
            Spawn();
        }
        OnItemMatrixChanged?.Invoke(this);
    }

    private void ClearSpawned()
    {
        if (SpawnedObjectsList != null)
        {
            foreach (GameObject spawnedObject in SpawnedObjectsList)
            {
                Destroy(spawnedObject);
            }
        }
        SpawnedObjectsMatrix = null;
        SpawnedObjectsList.Clear();
    }

    private void Spawn()
    {
        currentItemCount = itemCount;
        currentMarginX = marginX;
        currentMarginY = marginY;
        currentMinRows = minRows;
        currentMinColumns = minColumns;
        parentAligner.checkEveryFrame = false;
        parentAligner.transform.localScale = Vector3.one;
        ComputeSpawnAreaSize();
        ComputeAvailableAreaAspectSize();
        ComputeAvailableSpawnAreaAspectRatio();
        ComputeTable();
        ComputeSpawnAreaSize();
        ComputeItemPrefabSize();
        SpawnItems();
        PoitionItems();
        parentAligner.Align();
        parentAligner.checkEveryFrame = true;
    }

    private void ComputeSpawnAreaSize()
    {
        spawnAreaSize = TransformUtils.GetSizeOfTransform(spawnAreaTransform);
    }

    private void ComputeAvailableAreaAspectSize()
    {
        int marginYCount = Rows - 1;
        int marginXCount = Columns - 1;

        availableItemAreaSize = new Vector2(
            spawnAreaSize.x - marginXCount * marginX,
            spawnAreaSize.y - marginYCount * marginY
        );
    }

    private void ComputeAvailableSpawnAreaAspectRatio()
    {
        availableItemAreaAspectRatio
            = availableItemAreaSize.x
            / availableItemAreaSize.y;
    }

    private void ComputeTable()
    {
        if (minColumns > 0 && minRows == 0)
        {
            ComputeTableByColumns();
        }
        else if (minRows > 0 && minColumns == 0)
        {
            ComputeTableByRows();
        }
        else
        {
            ComputeTableByColumns();
            Rows = (int)MathUtils.ClampValue(Rows, minRows, maxRows);
        }
    }

    private void ComputeTableByColumns()
    {
        Columns = (int)Mathf.Ceil(
            Mathf.Sqrt(availableItemAreaAspectRatio * itemCount)
        );
        Columns = (int)MathUtils.ClampValue(Columns, minColumns, maxColumns);
        Rows = (int)Mathf.Ceil((float)itemCount / Columns);
    }

    private void ComputeTableByRows()
    {
        Rows = (int)Mathf.Ceil(
            Mathf.Sqrt(1 / availableItemAreaAspectRatio * itemCount)
        );
        Rows = (int)MathUtils.ClampValue(Rows, minRows, maxRows);
        Columns = (int)Mathf.Ceil((float)itemCount / Rows);
    }

    private void ComputeItemPrefabSize()
    {
        itemPrefabSize = TransformUtils.GetSizeOfTransform(
            itemPrefab.transform,
            includeMasked: false,
            includeInactive: true
        );
    }

    private void SpawnItems()
    {
        GameObject spawnedObject;
        SpawnedObjectsMatrix = new GameObject[Rows, Columns];
        SpawnedObjectsList.Clear();
        int itemCount = 0;
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                if (itemCount < this.itemCount)
                {
                    spawnedObject = Instantiate(itemPrefab, itemsParent);
                    SpawnedObjectsMatrix[i, j] = spawnedObject;
                    SpawnedObjectsList.Add(spawnedObject);
                    itemCount++;
                }
            }
        }
    }

    private void PoitionItems()
    {
        GameObject spawnedObject;
        float currentX;
        float currentY;
        if (spawnDirectionY == SpawnDirectionY.TOP_TO_BOTTOM)
        {
            currentY
                = -itemPrefabSize.y / 2f
                + Rows / 2f * itemPrefabSize.y
                + Rows / 2f * marginY
                - .5f * marginY;
        }
        else
        {
            currentY
                = itemPrefabSize.y / 2f
                - Rows / 2f * itemPrefabSize.y
                - Rows / 2f * marginY
                + .5f * marginY;
        }
        for (int i = 0; i < Rows; i++)
        {
            if (spawnDirectionX == SpawnDirectionX.LEFT_TO_RIGHT)
            {
                currentX
                    = itemPrefabSize.x / 2f
                    - Columns / 2f * itemPrefabSize.x
                    - Columns / 2f * marginX
                    + .5f * marginX;
            }
            else
            {
                currentX
                    = -itemPrefabSize.x / 2f
                    + Columns / 2f * itemPrefabSize.x
                    + Columns / 2f * marginX
                    - .5f * marginX;
            }
            
            for (int j = 0; j < Columns; j++)
            {
                spawnedObject = SpawnedObjectsMatrix[i,j];
                if (spawnedObject != null)
                {
                    spawnedObject.transform.localPosition = new Vector3(
                        currentX,
                        currentY,
                        spawnedObject.transform.localPosition.z
                    );
                }

                if (spawnDirectionX == SpawnDirectionX.LEFT_TO_RIGHT)
                {
                    currentX += itemPrefabSize.x + marginX;
                }
                else
                {
                    currentX -= itemPrefabSize.x + marginX;
                }
            }
            if (spawnDirectionY == SpawnDirectionY.TOP_TO_BOTTOM)
            {
                currentY -= itemPrefabSize.y + marginY;
            }
            else
            {
                currentY += itemPrefabSize.y + marginY;
            }
                
        }
    }
}
