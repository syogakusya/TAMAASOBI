using OpenCvSharp;
using OpenCvSharp.Demo;
using UnityEngine;

public class CountorFinder : WebCamera
{
    [SerializeField] public FlipMode ImageFlip;
    [SerializeField] public float Threshold = 96.4f;
    [SerializeField] public float CurveAccuracy = 10f;
    [SerializeField] public float MinArea = 5000f;
    [SerializeField] public PolygonCollider2D PolygonCollider;
    public enum ShowProcessingImage
    {
        Origin = 0,
        BinaryInv = 1,
        OriginContour = 2
    }
    [SerializeField] public  ShowProcessingImage showProcessingImage;
    [SerializeField] public bool ShowKeystoneCorrection = false;
    [SerializeField] public bool ShowConvertImage = true;
    
    [SerializeField] private GameObject LeftUp;
    [SerializeField] private GameObject LeftDown;
    [SerializeField] private GameObject RightUp;
    [SerializeField] private GameObject RightDown;


    private float aveWidth = 1280;
    private float aveHeight = 720;
    private float scale = 125;
    private Mat image;
    private Mat processImage = new Mat();
    private Mat processOriginImage = new Mat();
    private Mat convertImage = new Mat();
    private Point[][] contours;
    private HierarchyIndex[] hierarchy;
    private Vector2[] vectorList;
    private Point2f _LeftUp;
    private Point2f _RightDown;
    private Point2f _RightUp;
    private Point2f _LeftDown;


    //ƒeƒXƒg
    //string shapeName = "THISAREA";
    //[SerializeField] public int CountourLength = 20;
    //

    protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
    {


        image = OpenCvSharp.Unity.TextureToMat(input);

        Cv2.Flip(image, image, ImageFlip);

        DrawAndTransformKeystone(image);

        if (ShowConvertImage == true)
        {

            Cv2.CvtColor(convertImage, processImage, ColorConversionCodes.BGR2GRAY);
            convertImage.CopyTo(image);
        }
        else
        {
            Cv2.CvtColor(image, processImage, ColorConversionCodes.BGR2GRAY);
        }
        Cv2.Threshold(processImage, processImage, Threshold, 255, ThresholdTypes.BinaryInv);
        Cv2.FindContours(processImage, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxSimple, null);
        image.CopyTo(processOriginImage);

        PolygonCollider.pathCount = 0;
        foreach (Point[] contour in contours)
        {
            Point[] points = Cv2.ApproxPolyDP(contour, CurveAccuracy, true);
            var area = Cv2.ContourArea(contour);

            if (area > MinArea)
            {
                drawContour(processImage, new Scalar(127, 127, 127), 3, points);
                drawContour(processOriginImage, new Scalar(255, 0, 0), 3, points);

                PolygonCollider.pathCount++;
                PolygonCollider.SetPath(PolygonCollider.pathCount - 1, toVector2(points));
            }

            //if (points.Length > CountourLength)
            //{
            //    if (shapeName != null)
            //    {
            //        Moments m = Cv2.Moments(contour);
            //        int cx = (int)(m.M10 / m.M00);
            //        int cy = (int)(m.M01 / m.M00);

            //        //Cv2.DrawContours(image, new Point[][] { contour }, 0, new Scalar(0,0,0), -1);
            //        Cv2.PutText(image, shapeName, new Point(cx - 50, cy), HersheyFonts.HersheySimplex, 1.0, new Scalar(0, 0, 0));
            //    }
            //}
        }


        if (output == null)
            output = OpenCvSharp.Unity.MatToTexture(OutputTexture(showProcessingImage));
        else
            OpenCvSharp.Unity.MatToTexture(OutputTexture(showProcessingImage), output);

        return true;
    }

    private Vector2[] toVector2(Point[] points)
    {
        vectorList = new Vector2[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            vectorList[i] = new Vector2(points[i].X, points[i].Y);
        }
        return vectorList;
    }

    private void drawContour(Mat Image, Scalar Color, int Thickness, Point[] Points)
    {
        for (int i = 1; i < Points.Length; i++)
        {
            Cv2.Line(Image, Points[i - 1], Points[i], Color, Thickness);
        }
        Cv2.Line(Image, Points[Points.Length - 1], Points[0], Color, Thickness);
    }

    private Mat OutputTexture(ShowProcessingImage number)
    {
        switch (number)
        {
            case ShowProcessingImage.Origin:
                return image;
            case ShowProcessingImage.BinaryInv:
                return processImage;
            case ShowProcessingImage.OriginContour:
                return processOriginImage;
        }
        return null;
    }

    private int VectorToMatposX(float pos, Vector2 Size)
    {
        return Mathf.RoundToInt((pos * scale) + Size.x / 2);
    }

    private int VectorToMatposY(float pos, Vector2 Size)
    {
        return Mathf.RoundToInt(-(pos * scale) + Size.y / 2 + 1.5f);
    }


    Scalar KeyStoneColor = new Scalar(0, 255, 0);
    Vector2 rectSize;
    Vector3 lUpos;
    Vector3 lDpos;
    Vector3 rUpos;
    Vector3 rDpos;

    private void DrawAndTransformKeystone(Mat input)
    {
        rectSize = GetComponent<RectTransform>().sizeDelta;
        lUpos = LeftUp.transform.position;
        lDpos = LeftDown.transform.position;
        rUpos = RightUp.transform.position;
        rDpos = RightDown.transform.position;
        

        LeftUp.SetActive(false);
        RightUp.SetActive(false);
        LeftDown.SetActive(false);
        RightDown.SetActive(false);

        if (ShowKeystoneCorrection == true)
        {
            LeftUp.SetActive(true);
            RightUp.SetActive(true);
            LeftDown.SetActive(true);
            RightDown.SetActive(true);

            Cv2.Circle(input, VectorToMatposX(lUpos.x, rectSize), VectorToMatposY(lUpos.y, rectSize), 20, KeyStoneColor, -1);
            Cv2.Circle(input, VectorToMatposX(lDpos.x, rectSize), VectorToMatposY(lDpos.y, rectSize), 20, KeyStoneColor, -1);
            Cv2.Circle(input, VectorToMatposX(rUpos.x, rectSize), VectorToMatposY(rUpos.y, rectSize), 20, KeyStoneColor, -1);
            Cv2.Circle(input, VectorToMatposX(rDpos.x, rectSize), VectorToMatposY(rDpos.y, rectSize), 20, KeyStoneColor, -1);

            Cv2.Line(input, VectorToMatposX(lUpos.x, rectSize), VectorToMatposY(lUpos.y, rectSize), VectorToMatposX(lDpos.x, rectSize), VectorToMatposY(lDpos.y, rectSize), KeyStoneColor, 3);
            Cv2.Line(input, VectorToMatposX(lDpos.x, rectSize), VectorToMatposY(lDpos.y, rectSize), VectorToMatposX(rDpos.x, rectSize), VectorToMatposY(rDpos.y, rectSize), KeyStoneColor, 3);
            Cv2.Line(input, VectorToMatposX(rDpos.x, rectSize), VectorToMatposY(rDpos.y, rectSize), VectorToMatposX(rUpos.x, rectSize), VectorToMatposY(rUpos.y, rectSize), KeyStoneColor, 3);
            Cv2.Line(input, VectorToMatposX(rUpos.x, rectSize), VectorToMatposY(rUpos.y, rectSize), VectorToMatposX(lUpos.x, rectSize), VectorToMatposY(lUpos.y, rectSize), KeyStoneColor, 3);
        }

        if (ShowConvertImage == true)
        {
            _LeftUp = new Point2f((lUpos.x * scale) + rectSize.x / 2, -(lUpos.y * scale) + rectSize.y / 2);
            _LeftDown = new Point2f((lDpos.x * scale) + rectSize.x / 2, -(lDpos.y * scale) + rectSize.y / 2);
            _RightUp = new Point2f((rUpos.x * scale) + rectSize.x / 2, -(rUpos.y * scale) + rectSize.y / 2);
            _RightDown = new Point2f((rDpos.x * scale) + rectSize.x / 2, -(rDpos.y * scale) + rectSize.y / 2);

            var edjePoints = new Point2f[]
                {
                    _LeftUp,
                    _LeftDown,
                    _RightUp,
                    _RightDown,
                };

            var outPoints = new Point2f[]
            {
                    new Point2f(0,0),
                    new Point2f(0,aveHeight),
                    new Point2f(aveWidth, 0),
                    new Point2f(aveWidth,aveHeight),
            };

            Mat transMat = Cv2.GetPerspectiveTransform(edjePoints, outPoints);
            OpenCvSharp.Size imageSize = new OpenCvSharp.Size(aveWidth, aveHeight);

            Cv2.WarpPerspective(input, convertImage, transMat, imageSize);
        }
    }
}