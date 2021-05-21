using System;
using DiamondEngine;

public class BoonDisplay : DiamondComponent
{
    #region BOON_IMAGES_&_TEXT
    public GameObject boonImageObject1 = null;
    public GameObject boonTextObject1 = null;

    public GameObject boonImageObject2 = null;
    public GameObject boonTextObject2 = null;

    public GameObject boonImageObject3 = null;
    public GameObject boonTextObject3 = null;

    public GameObject boonImageObject4 = null;
    public GameObject boonTextObject4 = null;

    public GameObject boonImageObject5 = null;
    public GameObject boonTextObject5 = null;

    public GameObject boonImageObject6 = null;
    public GameObject boonTextObject6 = null;

    public GameObject boonImageObject7 = null;
    public GameObject boonTextObject7 = null;

    public GameObject boonImageObject8 = null;
    public GameObject boonTextObject8 = null;

    public GameObject boonImageObject9 = null;
    public GameObject boonTextObject9 = null;

    public GameObject boonImageObject10 = null;
    public GameObject boonTextObject10 = null;

    public GameObject boonImageObject11 = null;
    public GameObject boonTextObject11 = null;

    public GameObject boonImageObject12 = null;
    public GameObject boonTextObject12 = null;

    public GameObject boonImageObject13 = null;
    public GameObject boonTextObject13 = null;

    public GameObject boonImageObject14 = null;
    public GameObject boonTextObject14 = null;

    public GameObject boonImageObject15 = null;
    public GameObject boonTextObject15 = null;

    public GameObject boonImageObject16 = null;
    public GameObject boonTextObject16 = null;

    public GameObject boonImageObject17 = null;
    public GameObject boonTextObject17 = null;

    public GameObject boonImageObject18 = null;
    public GameObject boonTextObject18 = null;

    public GameObject boonImageObject19 = null;
    public GameObject boonTextObject19 = null;

    public GameObject boonImageObject20 = null;
    public GameObject boonTextObject20 = null;

    public GameObject boonImageObject21 = null;
    public GameObject boonTextObject21 = null;

    public GameObject boonImageObject22 = null;
    public GameObject boonTextObject22 = null;

    public GameObject boonImageObject23 = null;
    public GameObject boonTextObject23 = null;

    public GameObject boonImageObject24 = null;
    public GameObject boonTextObject24 = null;

    public GameObject boonImageObject25 = null;
    public GameObject boonTextObject25 = null;

    public GameObject boonImageObject26 = null;
    public GameObject boonTextObject26 = null;

    public GameObject boonImageObject27 = null;
    public GameObject boonTextObject27 = null;

    public GameObject boonImageObject28 = null;
    public GameObject boonTextObject28 = null;
    #endregion

    private GameObject[] boonImages = null;
    private GameObject[] boonTexts = null;

    public void Awake()
    {
        boonImages = new GameObject[]
        {
            boonImageObject1, boonImageObject2, boonImageObject3, boonImageObject4, boonImageObject5,
            boonImageObject6, boonImageObject7, boonImageObject8, boonImageObject9, boonImageObject10,
            boonImageObject11, boonImageObject12, boonImageObject13, boonImageObject14, boonImageObject15,
            boonImageObject16, boonImageObject17, boonImageObject18, boonImageObject19, boonImageObject20,
            boonImageObject21, boonImageObject22, boonImageObject23, boonImageObject24, boonImageObject25,
            boonImageObject26, boonImageObject27, boonImageObject28,
        };

        boonTexts = new GameObject[]
        {
            boonTextObject1, boonTextObject2, boonTextObject3, boonTextObject4, boonTextObject5,
            boonTextObject6, boonTextObject7, boonTextObject8, boonTextObject9, boonTextObject10,
            boonTextObject11, boonTextObject12, boonTextObject13, boonTextObject14, boonTextObject15,
            boonTextObject16, boonTextObject17, boonTextObject18, boonTextObject19, boonTextObject20,
            boonTextObject21, boonTextObject22, boonTextObject23, boonTextObject24, boonTextObject25,
            boonTextObject26, boonTextObject27, boonTextObject28
        };

        for(int i = 0; i < boonImages.Length; i++)
        {
            if(boonImages[i] != null)
            {
                boonImages[i].Enable(false);
            }
        }
    }

    public void SetBoon(int index, int textureUID, int boonCount)
    {                                                                         
        if (boonImages[index] == null || index > boonImages.Length)
            return;

        boonImages[index].Enable(true);
        Image2D image = boonImages[index].GetComponent<Image2D>();

        if(image != null)
            image.AssignLibrary2DTexture(textureUID);

        if(boonTexts[index] != null)
        {
            boonTexts[index].Enable(true);
            Text text = boonTexts[index].GetComponent<Text>();
            text.text = boonCount.ToString();
        }
    }

}