using System;
using DiamondEngine;

public class Hub_PanelImage : DiamondComponent
{
    private int[] groguIcons;
    private int[] mandoIcons;
    private int[] weaponIcons;    

    public void Awake()
    {
        groguIcons = new int[8] {
        201150191, 536585524, 2142726839, 1686235517, 914398508, 1333293800, 267008225, 761184293 };
        mandoIcons = new int[24] {
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0, 0, 0 };
        weaponIcons = new int[19] {
        0, 0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0,
        0, 0, 0, 0, 0, 0 };
    }

    public void UpdateIcon(int treeName, int treeNumber)
    {
        switch (treeName)
        {
            case 1:
                gameObject.GetComponent<Button>().ChangeSprites(groguIcons[treeNumber], groguIcons[treeNumber], groguIcons[treeNumber]);
                break;
            case 2:
                gameObject.GetComponent<Button>().ChangeSprites(0, 0, mandoIcons[treeNumber]);
                break;
            case 3:
                gameObject.GetComponent<Button>().ChangeSprites(0, 0, weaponIcons[treeNumber]);
                break;
            default:
                break;
        }

    }
}