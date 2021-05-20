using System;
using DiamondEngine;

public class BoonDisplay : DiamondComponent
{
    #region BOON_IMAGES_&_TEXT
    public GameObject boonImageObject1;
    public GameObject boonTextObject1;

    public GameObject boonImageObject2;
    public GameObject boonTextObject2;

    public GameObject boonImageObject3;
    public GameObject boonTextObject3;

    public GameObject boonImageObject4;
    public GameObject boonTextObject4;

    public GameObject boonImageObject5;
    public GameObject boonTextObject5;

    public GameObject boonImageObject6;
    public GameObject boonTextObject6;

    public GameObject boonImageObject7;
    public GameObject boonTextObject7;

    public GameObject boonImageObject8;
    public GameObject boonTextObject8;

    public GameObject boonImageObject9;
    public GameObject boonTextObject9;

    public GameObject boonImageObject10;
    public GameObject boonTextObject10;

    public GameObject boonImageObject11;
    public GameObject boonTextObject11;

    public GameObject boonImageObject12;
    public GameObject boonTextObject12;

    public GameObject boonImageObject13;
    public GameObject boonTextObject13;

    public GameObject boonImageObject14;
    public GameObject boonTextObject14;

    public GameObject boonImageObject15;
    public GameObject boonTextObject15;

    public GameObject boonImageObject16;
    public GameObject boonTextObject16;

    public GameObject boonImageObject17;
    public GameObject boonTextObject17;

    public GameObject boonImageObject18;
    public GameObject boonTextObject18;

    public GameObject boonImageObject19;
    public GameObject boonTextObject19;

    public GameObject boonImageObject20;
    public GameObject boonTextObject20;

    public GameObject boonImageObject21;
    public GameObject boonTextObject21;

    public GameObject boonImageObject22;
    public GameObject boonTextObject22;

    public GameObject boonImageObject23;
    public GameObject boonTextObject23;

    public GameObject boonImageObject24;
    public GameObject boonTextObject24;

    public GameObject boonImageObject25;
    public GameObject boonTextObject25;

    public GameObject boonImageObject26;
    public GameObject boonTextObject26;

    public GameObject boonImageObject27;
    public GameObject boonTextObject27;

    public GameObject boonImageObject28;
    public GameObject boonTextObject28;
    #endregion

    private GameObject[] boonImages;
    private GameObject[] boonTexts;

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

        for(int i = 0; i < boonImages.Length; ++i)
        {
            if(boonImages != null)
            {
                boonImages[i].Enable(false);
            }
        }
    }

    public void SetBoon(int index, int textureUID, int boonCount)
    {
        if (boonImages[index] == null)
            return;

        boonImages[index].Enable(true);
        Image2D image = boonImages[index].GetComponent<Image2D>();
        image.AssignLibrary2DTexture(textureUID);

        if(boonTexts[index] != null)
        {
            boonTexts[index].Enable(true);
            Text text = boonTexts[index].GetComponent<Text>();
            text.text = boonCount.ToString();
        }
    }

}