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
        1577500403, 957705934,1372558958, 199890712, 897227592, 2025342343, 435516336, 761216663, //Utility
        546882389, 1063854057, 2019801014, 1189706487, 111066637, 1629205377, 1116284871, 1583302608, //Aggression
        775271058, 292345320, 1671001101, 1136043790, 681984580, 1732492657, 157271948, 1558659302 }; //Defense

        weaponIcons = new int[19] {
        957705934, 2098479941, 664499657, 1189706487, 111066637, 2037896323, 1063854057, //Primary
        430313587, 1189706487, 1465364877, 2098479941, 111066637, 2037896323, //Secondary
        1189706487, 1809967235, 1683429269, 961618377, 111066637, 1030329892 }; //Special
    }

    public void UpdateIcon(int treeName, int treeNumber)
    {
        switch (treeName)
        {
            case 1:
                gameObject.GetComponent<Button>().ChangeSprites(groguIcons[treeNumber], groguIcons[treeNumber], groguIcons[treeNumber]);
                break;
            case 2:
                gameObject.GetComponent<Button>().ChangeSprites(mandoIcons[treeNumber], mandoIcons[treeNumber], mandoIcons[treeNumber]);
                break;
            case 3:
                gameObject.GetComponent<Button>().ChangeSprites(weaponIcons[treeNumber], weaponIcons[treeNumber], weaponIcons[treeNumber]);
                break;
            default:
                break;
        }

    }
}