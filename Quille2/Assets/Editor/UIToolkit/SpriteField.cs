using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class SpriteField : BaseField<Sprite>
{
    //public new class UxmlFactory : UxmlFactory<SpriteField, UxmlTraits> { }

    public new static readonly string ussClassName = "sprite-field";

    public static readonly string spriteBackgroundUssClassName = ussClassName + "__background";
    public static readonly string spritePreviewUssClassName = ussClassName + "__preview";
    public static readonly string spritePropertyUssClassName = ussClassName + "__property";
    public static readonly string spriteLabelUssClassName = ussClassName + "__label";
    public static readonly string spriteObjectFieldUssClassName = ussClassName + "__object-field";

    private readonly VisualElement _spriteBackground;
    private readonly VisualElement _spritePreview;
    private readonly VisualElement _spriteProperty;
    private readonly Label _spriteLabel;
    private readonly ObjectField _spriteObjectField;



    public SpriteField() : this(string.Empty) { }

    public SpriteField(string label) : this(label, new VisualElement()) { }

    private SpriteField(string label, VisualElement visualInput) : base(label, visualInput)
    {
        AddToClassList(ussClassName);

        // 
        this.style.flexDirection = FlexDirection.Column;
        this.style.alignItems = Align.Stretch;
        this.style.alignContent = Align.Center;


        // Sprite background
        _spriteBackground = visualInput;
        _spriteBackground.AddToClassList(spriteBackgroundUssClassName);

        _spriteBackground.style.flexShrink = 0;
        _spriteBackground.style.flexGrow = 1;
        _spriteBackground.style.minHeight = EditorGUIUtility.singleLineHeight;

        _spriteBackground.style.backgroundPositionX = new BackgroundPosition(BackgroundPositionKeyword.Center);
        _spriteBackground.style.backgroundPositionY = new BackgroundPosition(BackgroundPositionKeyword.Center);
        _spriteBackground.style.backgroundRepeat = new BackgroundRepeat(Repeat.NoRepeat, Repeat.NoRepeat);
        _spriteBackground.style.backgroundSize = new BackgroundSize(BackgroundSizeType.Contain);


        //
        _spritePreview = new VisualElement();
        _spritePreview.AddToClassList(spritePreviewUssClassName);








        //
        _spriteProperty = new VisualElement();
        _spriteProperty.AddToClassList(spritePropertyUssClassName);

        //_spriteProperty.style.flexDirection = FlexDirection.Row;
        //_spriteProperty.style.alignItems = Align.Stretch;




        //
        _spriteLabel = this.labelElement;
        _spriteLabel.AddToClassList(spriteLabelUssClassName);

        //_spriteLabel.style.flexShrink = 1;
        //_spriteLabel.style.minWidth = 0;
        //_spriteLabel.style.maxWidth = Length.Percent(40);



        string newLabel = this.labelElement.text;



        // 
        _spriteObjectField = new ObjectField(newLabel) { objectType = typeof(Sprite), allowSceneObjects = false };


        //_spriteObjectField.label = this.labelElement.text;


        _spriteObjectField.AddToClassList(spriteObjectFieldUssClassName);
        _spriteObjectField.RegisterValueChangedCallback(ev => { value = ev.newValue as Sprite; });

        //_spriteObjectField.style.flexGrow = 1;
        _spriteObjectField.style.marginLeft = _spriteObjectField.style.marginRight = _spriteObjectField.style.marginBottom = 0f;
        _spriteObjectField.style.marginTop = EditorGUIUtility.standardVerticalSpacing;




        this.Add(_spriteBackground);
        _spriteBackground.Add(_spritePreview);

        //this.Add(_spriteProperty);
        //if (this.labelElement != null)
        //{
        //    _spriteProperty.Add(this.labelElement);
        //}
        //_spriteProperty.Add(_spriteObjectField);

        this.Add(_spriteObjectField);




        RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }

    public override void SetValueWithoutNotify(Sprite newValue)
    {
        //base.SetValueWithoutNotify(newValue);
        _spriteObjectField.SetValueWithoutNotify(newValue);

        _spritePreview.style.backgroundImage = (newValue != null) ? Background.FromSprite(newValue) : StyleKeyword.None;
    }

    private void OnGeometryChanged(GeometryChangedEvent e)
    {
        UpdatePreviewDimensions();
    }

    private void UpdatePreviewDimensions()
    {
        _spriteBackground.style.width = _spriteObjectField.resolvedStyle.width;
        _spriteBackground.style.height = _spriteObjectField.resolvedStyle.width;
    }

}