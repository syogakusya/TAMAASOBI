using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropdownShowImage : MonoBehaviour
{
    [SerializeField] GameObject maincam;
    [SerializeField] GameObject optioncam;
    [SerializeField] CountorFinder CountorValueSetter;
    [SerializeField] Dropdown ShowImage;
    [SerializeField] Dropdown ImageFlip;
    [SerializeField] Toggle Keystone;
    [SerializeField] Toggle Convert;
    [SerializeField] GameObject opCanvas;
    [SerializeField] Slider Threshold;
    [SerializeField] Text TextThreshold;
    [SerializeField] Slider Accuracy;
    [SerializeField] Text TextAccuracy;
    [SerializeField] Slider MinArea;
    [SerializeField] Text TextMinArea;

    private void Start()
    {
        switch ((int)CountorValueSetter.ImageFlip)
        {
            case 0: ImageFlip.value = 0; break;
            case 1: ImageFlip.value = 1; break;
            case -1: ImageFlip.value = 2; break;
        }
        ShowImage.value = (int)CountorValueSetter.showProcessingImage;
        Keystone.isOn = CountorValueSetter.ShowKeystoneCorrection;
        Convert.isOn = CountorValueSetter.ShowConvertImage;
        Threshold.value = CountorValueSetter.Threshold;
        Accuracy.value = CountorValueSetter.CurveAccuracy;
        MinArea.value = CountorValueSetter.MinArea;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.O)){
            opCanvas.SetActive(!opCanvas.activeSelf);
        }
    }

    public void OnShowImageChange()
    {
        if(ShowImage.value == 0)
        {
            CountorValueSetter.showProcessingImage = CountorFinder.ShowProcessingImage.Origin;
        }
        else if(ShowImage.value == 1)
        {
            CountorValueSetter.showProcessingImage = CountorFinder.ShowProcessingImage.BinaryInv;
        }
        else if(ShowImage.value == 2)
        {
            CountorValueSetter.showProcessingImage = CountorFinder.ShowProcessingImage.OriginContour;
        }
    }

    public void OnImageFlipChange()
    {
        if(ImageFlip.value == 0)
        {
            CountorValueSetter.ImageFlip = OpenCvSharp.FlipMode.X;
        }
        else if(ImageFlip.value == 1)
        {
            CountorValueSetter.ImageFlip= OpenCvSharp.FlipMode.Y;
        }
        else if(ImageFlip.value == 2)
        {
            CountorValueSetter.ImageFlip = OpenCvSharp.FlipMode.XY;
        }
    }

    public void OnKeystoneChange()
    {
        CountorValueSetter.ShowKeystoneCorrection = Keystone.isOn;
        maincam.SetActive(!Keystone.isOn);
        optioncam.SetActive(Keystone.isOn);


    }

    public void OnConvertImage()
    {
        CountorValueSetter.ShowConvertImage = Convert.isOn;
    }

    public void OnThresholdChange()
    {
        CountorValueSetter.Threshold = Threshold.value;
        TextThreshold.text = "Threshold = "+ Threshold.value;
    }

    public void OnAccuracyChange()
    {
        CountorValueSetter.CurveAccuracy = Accuracy.value;
        TextAccuracy.text = "Accuracy = " + Accuracy.value;
    }

    public void OnMinAreaChange()
    {
        CountorValueSetter.MinArea = MinArea.value;
        TextMinArea.text = "MinArea = " + MinArea.value;
    }

}
