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
        // TODO: also have a surrounding padding?

        // Runtime
        [SerializeField] protected bool isDirty;

        [SerializeField] protected Vector2 shortestTextDimensions;
        [SerializeField] protected Vector2 calculatedTextDimensions;

        [SerializeField] protected float paddedIconWidth;
        [SerializeField] protected float paddedIconHeight;

        [SerializeField] protected float lineHeight;
        [SerializeField] protected int lineCount;

        [SerializeField] protected float summedTextLenght;
        [SerializeField] protected float iconAsAdditionalTextLenght;
        // TODO: deserialize these after testing.
  

        // PROPERTIES
        public Sprite IconSprite
        {
            get 
            { 
                if (myIcon != null)
                {
                    return myIcon.sprite;
                }
               else
                {
                    return null;
                }
            }
            set 
            { 
                if (myIcon != null)
                {
                    myIcon.sprite = value;
                }
            }
        }
        public string TextContent
        {
            get 
            { 
                if (myTextMain != null)
                {
                    return myTextMain.text;
                }
                else
                {
                    return string.Empty;
                }
            }
            set 
            {
                if (myTextMain != null)
                {
                    myTextMain.SetText(value);
                    RefreshTextData();
                }
            }
        }

        public Vector2 DimensionRatio
        {
            get { return dimensionRatio; }
            set
            {
                if (value.x >= 1 && value.y >= 1)
                {
                    dimensionRatio = value;
                    isDirty = true;
                }
            }
        }
        public float InteriorPadding
        {
            get { return interiorPadding; }
            set
            {
                interiorPadding = value;
                RefreshIconAndPaddingData();
            }
        }

        public bool HasIcon
        {
            get { return hasIcon && myIcon != null; }
            set 
            {
                hasIcon = value;
                isDirty = true;
            }
        }
        public Vector2 IconSize
        {
            get 
            { 
                if (myIconsRect != null)
                {
                    return myIconsRect.sizeDelta;
                }
                else
                {
                    return Vector2.zero;
                }
            }
            set
            {
                if (myIconsRect != null)
                {
                    myIconsRect.sizeDelta = value;
                    RefreshIconAndPaddingData();
                }
            }
        }
        public float FontSize
        {
            get 
            { 
                if (myTextMain != null)
                {
                    return myTextMain.fontSize;
                }
                else
                { 
                    return 0.0f; 
                }
            }
            set
            {
                if (myTextMain != null && value >= 1)
                {
                    myTextMain.fontSize = myTextOverflow.fontSize = value;
                    RefreshTextData();
                }
            }
        }

        public bool IsDirty { get { return isDirty; } }
        public void SetDirty() { isDirty = true; }

        public Vector2 CurrentDimensions
        {
            get { return myRect.sizeDelta; }
        }



        // METHODS
        protected void Init()
        {
            FetchComponents();

            myIconsRect.anchorMin = myIconsRect.anchorMax = myIconsRect.pivot = new Vector2(0.0f, 1.0f);

            myTextMain.ForceMeshUpdate();
            myTextOverflow.ForceMeshUpdate();
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


        // -> LAYOUT MANAGEMENT
        protected void RefreshIconAndPaddingData()
        {
            if (myIcon != null)
            {
                paddedIconWidth = myIconsRect.sizeDelta.x + interiorPadding;
                paddedIconHeight = myIconsRect.sizeDelta.y + interiorPadding;
                iconAsAdditionalTextLenght = paddedIconWidth * (paddedIconHeight / lineHeight); // should we round something here?
            }
            else
            {
                paddedIconWidth = 0.0f;
                paddedIconHeight = 0.0f;
                iconAsAdditionalTextLenght = 0.0f;
            }

            isDirty = true;
        }
        protected void RefreshTextData()
        {
            if (myTextMain != null)
            {
                myTextMain.ForceMeshUpdate();
                myTextOverflow.ForceMeshUpdate();
                // Not sure these are necessary;

                Vector2 originalSizeDelta = myTextMainsRect.sizeDelta;
                try
                {
                    shortestTextDimensions = myTextMainsRect.sizeDelta = myTextMain.GetPreferredValues(myTextMain.text, myTextMain.preferredWidth, Mathf.Infinity);
                    myTextMain.ForceMeshUpdate();

                    TMP_TextInfo mainTextInfo = myTextMain.textInfo;

                    lineCount = mainTextInfo.lineCount;
                    lineHeight = mainTextInfo.lineCount > 0 ? mainTextInfo.lineInfo[0].lineHeight : 0;

                    summedTextLenght = 0;
                    for (int i = 0; i < lineCount; i++)
                    {
                        summedTextLenght += mainTextInfo.lineInfo[i].length;
                        //summedTextLenght += mainTextInfo.lineInfo[i].maxAdvance;
                    }
                }
                finally
                {
                    myTextMainsRect.sizeDelta = originalSizeDelta;
                    myTextMain.ForceMeshUpdate();
                }
            }
            else
            {
                shortestTextDimensions = Vector2.zero;
                lineCount = 0;
                lineHeight = 0.0f;
                summedTextLenght = 0.0f;
            }

            if (hasIcon)
            {
                RefreshIconAndPaddingData();
            }

            isDirty = true;
        }

        protected Vector2 CalculateTextDimensionsFromSummedLenght(float summedTextLenght, Vector2 dimensionRatio, float lineHeight, float lineCount)
        {
            float ratio = dimensionRatio.x / dimensionRatio.y;

            // ax^2 + bx + c -> rx^2 + rHmx - L
            // a -> rH -> targetRatio * lineHeight
            // b -> rHm -> targetRatio * lineHeight * lineCount
            // c -> -L -> -summedLineWidth;

            float rH = ratio * lineHeight;
            float rHm = ratio * lineHeight * lineCount;

            Vector2 quadraticSolutions;
            if (MathHelpers.TrySolveQuadratic(rH, rHm, -summedTextLenght, out quadraticSolutions))
            {
                if (quadraticSolutions.x > 0)
                {
                    return new Vector2(summedTextLenght / quadraticSolutions.x, 
                                       lineHeight * (lineCount + quadraticSolutions.x));
                }
                else if (quadraticSolutions.y > 0)
                {
                    return new Vector2(summedTextLenght / quadraticSolutions.y, 
                                       lineHeight * (lineCount + quadraticSolutions.y));
                }
            }
            // Else,
            return myTextMain.GetPreferredValues(myTextMain.text, summedTextLenght/lineCount, Mathf.Infinity);
        }

        public Vector2 UpdateLayout(bool refreshAllData = false)
        {
            if (refreshAllData)
            {
                RefreshTextData();
                RefreshIconAndPaddingData();
            }

            float totalSummedLength = summedTextLenght;
            if (hasIcon && myIcon != null)
            {
                totalSummedLength += iconAsAdditionalTextLenght;
            }
            calculatedTextDimensions = CalculateTextDimensionsFromSummedLenght(totalSummedLength, dimensionRatio, lineHeight, lineCount);

            if (hasIcon && myIcon != null)
            {
                myIcon.gameObject.SetActive(true);
                myIconsRect.anchoredPosition = Vector2.zero;

                myTextMainsRect.anchoredPosition = new Vector2(paddedIconWidth, 0.0f);
                myTextMainsRect.sizeDelta = new Vector2(calculatedTextDimensions.x - paddedIconWidth, paddedIconHeight);
                myTextMain.ForceMeshUpdate();

                if (myTextMain.isTextOverflowing)
                {
                    myTextOverflow.gameObject.SetActive(true);

                    myTextMain.overflowMode = TextOverflowModes.Linked;
                    myTextMain.linkedTextComponent = myTextOverflow;
                    myTextMain.ForceMeshUpdate();

                    myTextOverflowsRect.anchoredPosition = new Vector2(0.0f, -paddedIconHeight);
                    myTextOverflowsRect.sizeDelta = new Vector2(calculatedTextDimensions.x, calculatedTextDimensions.y - paddedIconHeight);
                    myTextOverflow.ForceMeshUpdate();

                    float totalWidth = Mathf.Max(paddedIconWidth + myTextMain.renderedWidth, myTextOverflow.renderedWidth);
                    float totalHeight = paddedIconHeight + myTextOverflow.renderedHeight;

                    myRect.sizeDelta = new Vector2(totalWidth, totalHeight);
                }
                else
                {
                    myTextOverflow.gameObject.SetActive(false);

                    myTextMain.overflowMode = TextOverflowModes.Overflow;
                    myTextMain.ForceMeshUpdate();

                    myTextOverflowsRect.sizeDelta = Vector2.zero;
                    myTextOverflow.ForceMeshUpdate();
                    
                    myRect.sizeDelta = new Vector2(paddedIconWidth + myTextMain.renderedWidth, Mathf.Max(myIconsRect.sizeDelta.y, myTextMain.renderedHeight));

                    myTextMainsRect.anchoredPosition = new Vector2(paddedIconWidth, -(myRect.sizeDelta.y - myTextMain.renderedHeight) / 2.0f);
                    if (myIconsRect.sizeDelta.y < myTextMain.renderedHeight) // Not sure this is necessary / the offset is visible as such small values
                    {
                        myIconsRect.anchoredPosition = new Vector2(0.0f, -(myRect.sizeDelta.y - myIconsRect.sizeDelta.y) / 2.0f);
                    }
                }
            }
            else
            {
                myIcon.gameObject.SetActive(false);
                myTextOverflow.gameObject.SetActive(false);

                myTextMain.overflowMode = TextOverflowModes.Overflow;

                myTextMainsRect.anchoredPosition = Vector2.zero;
                myTextMainsRect.sizeDelta = calculatedTextDimensions;
                myTextMain.ForceMeshUpdate();

                myRect.sizeDelta = myTextMain.GetRenderedValues();
            }

            isDirty = false;
            return myRect.sizeDelta;
        }


        // BUILT IN
        private void Awake()
        {
            Init();
        }
    }
}

