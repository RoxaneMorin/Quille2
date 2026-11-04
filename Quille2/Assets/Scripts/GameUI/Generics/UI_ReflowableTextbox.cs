using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


namespace QuilleUI
{
    public class UI_ReflowableTextbox : MonoBehaviour
    {
        // VARIABLES
        [SerializeField] protected TMP_Text myTextMain;
        [SerializeField] protected TMP_Text myTextOverflow;
        [SerializeField] protected Image myIcon;

        [SerializeField] protected RectTransform myRect;
        [SerializeField] protected RectTransform myTextMainsRect;
        [SerializeField] protected RectTransform myTextOverflowsRect;
        [SerializeField] protected RectTransform myIconsRect;

        // Layout
        [SerializeField] protected bool hasIcon = true;
        [SerializeField] protected Vector2 dimensionRatio;
        [SerializeField] protected float interiorPadding = 10.0f;
        

        // Runtime
        [SerializeField] protected Vector2 basePreferredDims;
        [SerializeField] protected Vector2 handCalculatedDims;

        [SerializeField] protected float lineHeight;
        [SerializeField] protected int lineCount;
        [SerializeField] protected float summedTextLenght;



        // PROPERTIES
        public Sprite IconSprite
        {
            get { return myIcon.sprite; }
            set { myIcon.sprite = value; }
        }
        public string TextContent
        {
            get { return myTextMain.text; }
            set 
            {
                myTextMain.SetText(value);
                RefreshTextInfo();
            }
        }

        public bool HasIcon
        {
            get { return hasIcon; }
            set { hasIcon = value; }
        }
        public Vector2 DimensionRatio
        {
            get { return dimensionRatio; }
            set 
            { 
                if (value.x >= 1 && value.y >= 1)
                {
                    dimensionRatio = value;
                }
            }
        }
        public float InteriorPadding
        {
            get { return interiorPadding; }
            set { interiorPadding = value; }
        }
        public float FontSize
        {
            get { return myTextMain.fontSize; }
            set
            {
                if (value >= 1)
                {
                    myTextMain.fontSize = myTextOverflow.fontSize = value;

                    myTextMain.ForceMeshUpdate();
                    myTextOverflow.ForceMeshUpdate();
                }
            }
        }



        // METHODS
        protected void Init()
        {
            FetchComponents();

            Vector2 everythingTopLeft = new Vector2(0.0f, 1.0f);
            myIconsRect.anchorMin = myIconsRect.anchorMax = myIconsRect.pivot = everythingTopLeft;
        }
        protected void FetchComponents()
        {
            myRect = (RectTransform)transform;

            if (myIcon == null)
            {
                myIcon = GetComponentInChildren<Image>();
            }
            if (myIcon != null)
            {
                myIconsRect = (RectTransform)myIcon.transform;
            }
            else
            {
                Debug.LogWarning($"The ReflowableTextBox '{this.name}' lacks its icon (Image) reference and will not function properly.");
            }

            if (myTextMain != null)
            {
                myTextMainsRect = (RectTransform)myTextMain.transform;
            }
            else
            {
                Debug.LogWarning($"The ReflowableTextBox '{this.name}' lacks its main TMP_Text reference and will not function properly.");
            }

            if (myTextOverflow != null)
            {
                myTextOverflowsRect = (RectTransform)myTextOverflow.transform;
            }
            else
            {
                Debug.LogWarning($"The ReflowableTextBox '{this.name}' lacks its overflow TMP_Text reference and will not function properly.");
            }
        }




        public void RefreshTextInfo()
        {
            lineHeight = myTextMain.textInfo.lineInfo[0].lineHeight;
            basePreferredDims = myTextMain.GetPreferredValues(myTextMain.preferredWidth, lineHeight);

            myTextMainsRect.sizeDelta = basePreferredDims;
            myTextMain.ForceMeshUpdate();

            lineCount = myTextMain.textInfo.lineCount;

            summedTextLenght = 0;
            for (int i = 0; i < lineCount; i++)
            {
                summedTextLenght += myTextMain.textInfo.lineInfo[i].length;
            }
        }


        
        public void SetText(string newText)
        {
            myTextMain.SetText(newText);
            RefreshTextInfo();
        }



        // TODO: move to a helper class.
        protected (float, float) SolveQuadraticFormula(float a, float b, float c)
        {
            // x = (-b ± sqrt(b^2 - 4ac)) / 2a

            float bSquared = b * b;
            float fourAC = 4 * a * c;
            float inSqrt = Mathf.Sqrt(bSquared - fourAC);

            float numeratorAdd = -b + inSqrt;
            float numeratorSub = -b - inSqrt;
            float denumerator = 2 * a;

            float resultAdd = numeratorAdd / denumerator;
            float resultSub = numeratorSub / denumerator;

            return (resultAdd, resultSub);
        }




        //
        protected Vector2 CalculateDimensionsForSummedTextLenght(float summedTextLenght)
        {
            if (lineHeight == 0)
            {
                lineHeight = myTextMain.textInfo.lineInfo[0].lineHeight;
            }

            float currentRatio = basePreferredDims.x / basePreferredDims.y;
            float targetRatio = dimensionRatio.x / dimensionRatio.y;

            // ax^2 + bx + c -> rx^2 + rHmx - L
            // a -> rH -> targetRatio * lineHeight
            // b -> rHm -> targetRatio * lineHeight * lineCount
            // c -> -L -> -summedLineWidth;

            float rH = targetRatio * lineHeight;
            float rHm = targetRatio * lineHeight * lineCount;
            (float, float) mysteryValue = SolveQuadraticFormula(rH, rHm, -summedTextLenght);

            // TODO: verify which result to use.

            float newWidth = summedTextLenght / mysteryValue.Item1;
            float newHeight = lineHeight * (lineCount + mysteryValue.Item1);

            return new Vector2(newWidth, newHeight);
        }


        // TODO: figure out why the result of editor Ui manipulations is sometimes fucky




        public void UpdateTextboxDimensions(bool refreshTextInfo = true)
        {
            if (refreshTextInfo)
            {
                RefreshTextInfo();
            }

            if (hasIcon)
            {
                myIcon.gameObject.SetActive(true);

                float paddedIconWidth = myIconsRect.sizeDelta.x + interiorPadding;
                float paddedIconHeight = myIconsRect.sizeDelta.y + interiorPadding;

                float iconHeightInLines = paddedIconHeight / lineHeight;
                float iconAsAdditionalWidth = paddedIconWidth * iconHeightInLines;
                float summedTextAndIconLenght = summedTextLenght + iconAsAdditionalWidth;

                handCalculatedDims = CalculateDimensionsForSummedTextLenght(summedTextAndIconLenght);

                myTextMainsRect.sizeDelta = new Vector2(handCalculatedDims.x - paddedIconWidth, paddedIconHeight);
                myTextMain.ForceMeshUpdate();

                if (myTextMain.isTextOverflowing)
                {
                    myTextOverflow.gameObject.SetActive(true);
                    myTextMain.overflowMode = TextOverflowModes.Linked;
                    myTextMain.linkedTextComponent = myTextOverflow;

                    myTextMainsRect.anchoredPosition = new Vector2(paddedIconWidth, 0.0f);
                    myTextMain.ForceMeshUpdate();

                    myTextOverflowsRect.anchoredPosition = new Vector2(0.0f, -paddedIconHeight);
                    myTextOverflowsRect.sizeDelta = new Vector2(handCalculatedDims.x, handCalculatedDims.y - paddedIconHeight);
                    myTextOverflow.ForceMeshUpdate();

                    myRect.sizeDelta = new Vector2(handCalculatedDims.x, paddedIconHeight + myTextOverflow.GetRenderedValues().y);
                }
                else
                {
                    myTextOverflow.gameObject.SetActive(false);
                    myTextMain.overflowMode = TextOverflowModes.Overflow;

                    float textPaddingY = -(myIconsRect.sizeDelta.y - myTextMain.renderedHeight) / 2.0f;
                    myTextMainsRect.anchoredPosition = new Vector2(paddedIconWidth, textPaddingY);

                    myRect.sizeDelta = new Vector2(myTextMain.renderedWidth + paddedIconWidth, Mathf.Max(myIconsRect.sizeDelta.y, myTextMain.GetRenderedValues().y));
                }
            }
            else
            {
                myIcon.gameObject.SetActive(false);
                myTextOverflow.gameObject.SetActive(false);

                myTextMain.overflowMode = TextOverflowModes.Overflow;

                handCalculatedDims = CalculateDimensionsForSummedTextLenght(summedTextLenght);

                myTextMainsRect.anchoredPosition = Vector2.zero;
                myTextMainsRect.sizeDelta = handCalculatedDims;
                myTextMain.ForceMeshUpdate();

                myRect.sizeDelta = myTextMain.GetRenderedValues();
            }
        }



        [ExecuteInEditMode]
        private void Awake()
        {
            Init();
        }
    }
}

