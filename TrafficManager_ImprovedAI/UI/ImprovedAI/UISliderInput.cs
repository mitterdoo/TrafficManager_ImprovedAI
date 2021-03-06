using System;
using System.Collections.Generic;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ICities;
using UnityEngine;

// Adapted from Road Assistant mod.

namespace TrafficManager_ImprovedAI
{
    public class Slider : UISlider
    {
        public new float value
        {
            get { return this.m_RawValue; }

            set
            {
                value = (float) Math.Round(Mathf.Max(this.minValue, Mathf.Min(this.maxValue, value)), 2);
                if (!Mathf.Approximately(value, this.m_RawValue))
                {
                    this.m_RawValue = value;
                    this.OnValueChanged();
                }
            }
        }

        protected override void OnMouseWheel(UIMouseEventParameter p)
        {
            this.value += this.scrollWheelAmount * p.wheelDelta * 1.0f;
        }

        public void RelayMouseWheel(UIMouseEventParameter p)
        {
            OnMouseWheel(p);
        }

        public void RelayMouseDown(UIMouseEventParameter p)
        {
            OnMouseDown(p);
        }
    }

    /// <summary>
    /// UIComponent inheriting from UIPanel. Creates slider with label and text field. Requires the following to be set:
    /// 
    /// LabelText - Text to display above slider.
    /// MinValue - Min value of slider.
    /// MaxValue - Max value of slider.
    /// </summary>
    public class UISliderInput : UIPanel
    {
        private UILabel label;
        private Slider slider;
        private UITextField textField;

        public string LabelText
        {
            get { return label.text; }
            set { label.text = value; }
        }

        public Slider Slider
        {
            get { return slider; }
        }

        public float MinValue
        {
            get { return slider.minValue; }
            set { slider.minValue = value; }
        }

        public float MaxValue
        {
            get { return slider.maxValue; }
            set { slider.maxValue = value; }
        }

        public float SliderValue
        {
            get { return slider.value; }
            set { slider.value = value; }
        }

        public float StepSize
        {
            get { return slider.stepSize; }
            set { slider.stepSize = value; }
        }

        public float Height
        {
            get { return height; }
            set { height = value; }
        }

        public float Width
        {
            get { return width; }
            set { width = value; }
        }

        public UIPanel Parent { get; set; }

        public override void Awake()
        {
            base.Awake();

            label = AddUIComponent<UILabel>();
            slider = AddUIComponent<Slider>();
            textField = AddUIComponent<UITextField>();

            LabelText = "(None)";

            height = 40;
            width = 460;

            this.slider.eventValueChanged += delegate(UIComponent sender, float value)
            {
                string format = "{0:0}";
                if (StepSize < 1)
                {
                    format = "{0:0.0}";
                }
                textField.text = String.Format(format, value);
            };

            // Check if the text field changed, and update the slider value.
            this.textField.eventTextSubmitted += delegate(UIComponent sender, string s)
            {
                float num;
                if (float.TryParse(s, out num))
                {
                    if (num > MaxValue)
                    {
                        textField.text = MaxValue.ToString();
                    }
                    else if (num < MinValue)
                    {
                        textField.text = MinValue.ToString();
                    }
                    SliderValue = num;
                }
                textField.text = SliderValue.ToString();
            };
        }
        public override void Start()
        {
            base.Start();

            if (Parent == null)
            {
                UnityEngine.Debug.Log(String.Format("Parent not set in {0}", this.GetType().Name));
                return;
            }

            width = Parent.width;
            isVisible = true;
            canFocus = true;
            isInteractive = true;

            label.relativePosition = new Vector3(5, 0);
            label.textScale = 1.0f;
            label.text = LabelText;

            slider.width = width - 108;
            slider.height = 3;
            slider.backgroundSprite = "BudgetSlider";
            slider.relativePosition = new Vector3(8, 27);
            slider.minValue = MinValue;
            slider.maxValue = MaxValue;
            slider.value = SliderValue;
            
            UISlicedSprite tracSprite = slider.AddUIComponent<UISlicedSprite>();
            tracSprite.relativePosition = new Vector3(0, 1);
            tracSprite.autoSize = true;
            tracSprite.size = tracSprite.parent.size;
            tracSprite.fillDirection = UIFillDirection.Horizontal;
            tracSprite.spriteName = "";

            UISlicedSprite thumbSprite = tracSprite.AddUIComponent<UISlicedSprite>();
            thumbSprite.relativePosition = new Vector3(0, 1);
            thumbSprite.fillDirection = UIFillDirection.Vertical;
            thumbSprite.autoSize = true;
            thumbSprite.spriteName = "SliderBudget";

            slider.thumbObject = thumbSprite;
            slider.fillIndicatorObject = tracSprite;            

            textField.width = 80;
            textField.height = 30;
            textField.relativePosition = new Vector3(364, 0.5f);
            textField.padding.top = 7;
            textField.normalBgSprite = "TextFieldPanel";
            textField.hoveredBgSprite = "TextFieldPanelHovered";
            textField.focusedBgSprite = "TextFieldUnderline";
            textField.selectionSprite = "EmptySprite";
            textField.selectionBackgroundColor = new Color32(0, 172, 234, 255);
            textField.selectOnFocus = true;

            string format = "{0:0}";
            if (StepSize < 1)
            {
                format = "{0:0.0}";
            }
            textField.text = String.Format(format, SliderValue);

            textField.isInteractive = true;
            textField.enabled = true;
            textField.readOnly = false;
            textField.builtinKeyNavigation = true;
            textField.eventMouseWheel += delegate(UIComponent component, UIMouseEventParameter eventParam) {
                eventParam.Use();
                slider.RelayMouseWheel(eventParam);
            };

            this.eventMouseWheel += delegate(UIComponent component, UIMouseEventParameter eventParam) {
                eventParam.Use();
                slider.RelayMouseWheel(eventParam);
            };

            this.eventMouseDown += delegate(UIComponent component, UIMouseEventParameter eventParam) {
                eventParam.Use();
                slider.RelayMouseDown(eventParam);
            };
        }
    }
}
