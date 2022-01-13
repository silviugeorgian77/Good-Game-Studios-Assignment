using UnityEngine;

public class ArmyUnitItem : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer armyUnitRenderer;

    [SerializeField]
    private Sprite spearmanSprite;

    [SerializeField]
    private Sprite swordsmanSprite;

    [SerializeField]
    private Sprite archerSprite;

    public void Bind(ArmyUnit armyUnit)
    {
        Sprite sprite = null;
        if (armyUnit.armyUnitType == ArmyUnitType.SPEARMAN)
        {
            sprite = spearmanSprite;
        }
        else if (armyUnit.armyUnitType == ArmyUnitType.SWORDSMAN)
        {
            sprite = swordsmanSprite;
        }
        else if (armyUnit.armyUnitType == ArmyUnitType.ARCHER)
        {
            sprite = archerSprite;
        }
        armyUnitRenderer.sprite = sprite;
    }
}
