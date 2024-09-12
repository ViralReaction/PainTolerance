using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace PainTolerance
{
    public class CustomWidgets
    {
        public static void DrawIntOptionWithButtons(Rect rect, ref float value, string label, float minValue, float maxValue, float stepperValue)
        {
            TextAnchor anchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;

            // Draw the label
            Rect labelRect = new Rect(rect.x, rect.y, rect.width - 100f, rect.height); // Adjust label width as needed
            Widgets.Label(labelRect, label);

            // Define the widths for the buttons and text field
            float buttonWidth = 30f;
            float textWidth = 50f;
            float spacing = 5f;

            // Calculate the position for the buttons and text box
            float totalWidth = buttonWidth * 2 + textWidth + spacing * 2;
            float availableX = rect.xMax - totalWidth;

            // Minus Button (-)
            if (Widgets.ButtonText(new Rect(availableX, rect.y, buttonWidth, rect.height), "-"))
            {
                value = Math.Max(value - stepperValue, minValue); // Decrease value, but don't go below minValue
            }

            // Text field
            string buffer = value.ToString();
            Widgets.TextFieldNumeric(new Rect(availableX + buttonWidth + spacing, rect.y, textWidth, rect.height), ref value, ref buffer, minValue, maxValue); // Limit input between minValue and maxValue

            // Plus Button (+)
            if (Widgets.ButtonText(new Rect(availableX + buttonWidth + textWidth + spacing * 2, rect.y, buttonWidth, rect.height), "+"))
            {
                value = Math.Min(value + stepperValue, maxValue); // Increase value, but don't exceed maxValue
            }

            Text.Anchor = anchor;
        }

        public static float HorizontalSlider(Rect rect, float value, float min, float max, bool drawLabel = true, string label = null, string leftAlignedLabel = null, string rightAlignedLabel = null, float roundTo = -1f)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            float newValue = value;
            TextAnchor originalAnchor = Text.Anchor;
            try
            {
                switch (Event.current.GetTypeForControl(controlID))
                {
                    case EventType.MouseDown:
                        if (rect.Contains(Event.current.mousePosition) && Event.current.button == 0)
                        {
                            GUIUtility.hotControl = controlID;
                            Event.current.Use();
                        }
                        break;

                    case EventType.MouseDrag:
                        if (GUIUtility.hotControl == controlID)
                        {
                            float previousValue = newValue;
                            newValue = Mathf.Clamp((Event.current.mousePosition.x - rect.x) / rect.width * (max - min) + min, min, max);
                            if (roundTo > 0f)
                            {
                                newValue = Mathf.Round(newValue / roundTo) * roundTo;
                            }
                            if (newValue != previousValue)
                            {
                                CheckPlayDragSliderSound();
                            }

                            Event.current.Use();
                        }
                        break;

                    case EventType.MouseUp:
                        if (GUIUtility.hotControl == controlID && Event.current.button == 0)
                        {
                            GUIUtility.hotControl = 0;
                            Event.current.Use();
                        }
                        break;

                    case EventType.Repaint:
                        Rect sliderRect = new Rect(rect.x, rect.y + (drawLabel ? 20f : 0f), rect.width, 8f); // Adjust position based on label presence
                        Widgets.DrawAtlas(sliderRect, SliderRailAtlas);

                        float handleX = Mathf.Clamp(rect.x + (newValue - min) / (max - min) * rect.width - 6f, rect.x, rect.x + rect.width - 12f);
                        GUI.DrawTexture(new Rect(handleX, sliderRect.y - 2.5f, 12f, 12f), SliderHandle);

                        if (drawLabel && !string.IsNullOrEmpty(label))
                        {
                            Text.Anchor = TextAnchor.MiddleCenter;
                            Vector2 labelSize = Text.CalcSize(label);
                            Rect labelRect = new Rect(
                                rect.x + (rect.width / 2f) - (labelSize.x / 2f),
                                rect.y,
                                labelSize.x,
                                labelSize.y
                            );
                            Widgets.Label(labelRect, label);
                        }
                        if (!string.IsNullOrEmpty(leftAlignedLabel))
                        {
                            Text.Anchor = TextAnchor.MiddleLeft;
                            Widgets.Label(new Rect(rect.x, rect.y, rect.width / 2, 20f), leftAlignedLabel);
                        }

                        if (!string.IsNullOrEmpty(rightAlignedLabel))
                        {
                            Text.Anchor = TextAnchor.MiddleRight;
                            Widgets.Label(new Rect(rect.x + rect.width / 2, rect.y, rect.width / 2, 20f), rightAlignedLabel);
                        }
                        break;
                }

                return newValue;
            }
            finally
            {
                Text.Anchor = originalAnchor;
            }
        }

        public static float VerticalSlider(Rect rect, float value, float min, float max, bool drawLabel = true, string label = null, string topAlignedLabel = null, string bottomAlignedLabel = null, float roundTo = -1f)
        {
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            float newValue = value;
            TextAnchor originalAnchor = Text.Anchor;

            try
            {
                switch (Event.current.GetTypeForControl(controlID))
                {
                    case EventType.MouseDown:
                        if (rect.Contains(Event.current.mousePosition) && Event.current.button == 0)
                        {
                            GUIUtility.hotControl = controlID;
                            Event.current.Use();
                        }
                        break;

                    case EventType.MouseDrag:
                        if (GUIUtility.hotControl == controlID)
                        {
                            float previousValue = newValue;
                            // Adjust the slider interaction to the Y-axis
                            newValue = Mathf.Clamp((rect.yMax - Event.current.mousePosition.y) / rect.height * (max - min) + min, min, max);
                            if (roundTo > 0f)
                            {
                                newValue = Mathf.Round(newValue / roundTo) * roundTo;
                            }
                            if (newValue != previousValue)
                            {
                                CheckPlayDragSliderSound();
                            }

                            Event.current.Use();
                        }
                        break;

                    case EventType.MouseUp:
                        if (GUIUtility.hotControl == controlID && Event.current.button == 0)
                        {
                            GUIUtility.hotControl = 0;
                            Event.current.Use();
                        }
                        break;

                    case EventType.Repaint:
                        // Draw the slider rail for the vertical slider
                        Rect sliderRect = new Rect(rect.x + (drawLabel ? 20f : 0f), rect.y, 8f, rect.height); // Adjust position based on label presence
                        Widgets.DrawAtlas(sliderRect, SliderRailAtlas);

                        // Calculate handle position along the Y-axis
                        float handleY = Mathf.Clamp(rect.yMax - (newValue - min) / (max - min) * rect.height - 6f, rect.y, rect.yMax - 12f);
                        GUI.DrawTexture(new Rect(sliderRect.x - 2.5f, handleY, 12f, 12f), SliderHandle);

                        // Draw label in the center if specified
                        if (drawLabel && !string.IsNullOrEmpty(label))
                        {
                            Text.Anchor = TextAnchor.MiddleCenter;
                            Vector2 labelSize = Text.CalcSize(label);
                            Rect labelRect = new Rect(
                                rect.x + (rect.width / 2f) - (labelSize.x / 2f),
                                rect.y + (rect.height / 2f) - (labelSize.y / 2f),
                                labelSize.x,
                                labelSize.y
                            );
                            Widgets.Label(labelRect, label);
                        }

                        // Top-aligned label
                        if (!string.IsNullOrEmpty(topAlignedLabel))
                        {
                            Text.Anchor = TextAnchor.UpperCenter;
                            Widgets.Label(new Rect(rect.x, rect.y, rect.width, 20f), topAlignedLabel);
                        }

                        // Bottom-aligned label
                        if (!string.IsNullOrEmpty(bottomAlignedLabel))
                        {
                            Text.Anchor = TextAnchor.LowerCenter;
                            Widgets.Label(new Rect(rect.x, rect.yMax - 20f, rect.width, 20f), bottomAlignedLabel);
                        }
                        break;
                }

                return newValue;
            }
            finally
            {
                Text.Anchor = originalAnchor; // Restore original TextAnchor
            }
        }

        public static void DrawXYAxisSliders(Rect rect, ref int xValue, ref int yValue, int minX, int maxX, int minY, int maxY, string labelX = "X", string labelY = "Y")
        {
            // Store the original Text.Anchor to restore it later
            TextAnchor anchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;

            // Slider height and spacing
            float sliderHeight = 24f;  // Height for the horizontal slider (X-axis)
            float labelWidth = 50f;    // Width for the labels
            float spacing = 10f;       // Spacing between sliders and other elements

            // Adjust the width for the horizontal slider to avoid overlapping the vertical slider
            float xSliderWidth = rect.width - labelWidth - 30f - spacing;  // Subtract space for the vertical slider on the right

            // Calculate the remaining height for the vertical slider, leaving space for the horizontal slider
            float verticalSliderHeight = rect.height - sliderHeight - spacing;

            // Create a rect for the X-axis slider (horizontal)
            Rect xLabelRect = new Rect(rect.x, rect.yMax - sliderHeight, labelWidth, sliderHeight);
            Rect xSliderRect = new Rect(xLabelRect.xMax + spacing, rect.yMax - sliderHeight, xSliderWidth, sliderHeight);

            // Draw the label and the horizontal slider for the X-axis
            Widgets.Label(xLabelRect, labelX);
            xValue = (int)HorizontalSlider(xSliderRect, xValue, minX, maxX, true, labelX + ": " + xValue.ToString());

            // Create a rect for the Y-axis slider (vertical)
            Rect ySliderRect = new Rect(rect.xMax - 30f, rect.y, 24f, verticalSliderHeight);

            // Draw the vertical slider for the Y-axis
            yValue = (int)VerticalSlider(ySliderRect, yValue, minY, maxY, true, labelY + ": " + yValue.ToString());

            // Restore the original Text.Anchor
            Text.Anchor = anchor;
        }

        private static void CheckPlayDragSliderSound()
        {
            if (Time.realtimeSinceStartup > lastDragSliderSoundTime + 0.075f)
            {
                SoundDefOf.DragSlider.PlayOneShotOnCamera(null);
                lastDragSliderSoundTime = Time.realtimeSinceStartup;
            }
        }
        private static float lastDragSliderSoundTime = -1f;
        private static bool isDraggingMinHandle = false;
        private static bool isDraggingMaxHandle = false;
        private static readonly Texture2D SliderHandle = ContentFinder<Texture2D>.Get("UI/Buttons/SliderHandle", true);
        private static readonly Texture2D SliderRailAtlas = ContentFinder<Texture2D>.Get("UI/Buttons/SliderRail", true);
        private static readonly Color RangeControlTextColor = new Color(0.6f, 0.6f, 0.6f);
    }
}
