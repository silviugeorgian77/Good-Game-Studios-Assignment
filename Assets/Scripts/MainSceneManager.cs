using UnityEngine;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class MainSceneManager : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField inputField;

    [SerializeField]
    private ItemMatrixSpawner itemMatrixSpawner;

    private List<ArmyUnit> armyUnits = new List<ArmyUnit>();

    private void InitMatrix()
    {
        itemMatrixSpawner.OnItemMatrixChanged
            = (ItemMatrixSpawner ItemMatrixSpawner) =>
            {
                InitItems();
            };
        itemMatrixSpawner.itemCount = armyUnits.Count;
        itemMatrixSpawner.ResetMatrix();
    }

    private void InitItems()
    {
        ArmyUnitItem armyUnitItem;
        ArmyUnit armyUnit;
        GameObject spawnedObject;
        int armyUnitIndex = 0;
        for (int i = 0;
            i < itemMatrixSpawner.SpawnedObjectsMatrix.GetLength(0);
            i++)
        {
            for (int j = 0;
                j < itemMatrixSpawner.SpawnedObjectsMatrix.GetLength(1);
                j++)
            {
                if (armyUnitIndex < armyUnits.Count)
                {
                    spawnedObject
                        = itemMatrixSpawner.SpawnedObjectsMatrix[i, j];
                    armyUnit = armyUnits[armyUnitIndex];
                    armyUnitItem = spawnedObject.GetComponent<ArmyUnitItem>();
                    armyUnitItem.Bind(armyUnit);
                }
                armyUnitIndex++;
            }
        }
    }

    public void OnSpawnClicked()
    {
        int sum = int.Parse(inputField.text);

        List<ArmyUnitType> armyUnitTypes
            = EnumUtils.GetValues<ArmyUnitType>().ToList();
        int count = armyUnitTypes.Count();

        int[] armyCounts = RandomUtils.GenerateRandomNumbersThatAddUpToSum(
            sum,
            count,
            1,
            sum
        );

        armyUnits.Clear();
        for (int i = 0; i < armyUnitTypes.Count; i++)
        {
            for (int j = 0; j < armyCounts[i]; j++)
            {
                armyUnits.Add(
                    new ArmyUnit()
                    {
                        armyUnitType = armyUnitTypes[i]
                    }
                );
            }
        }

        InitMatrix();
    }
}
