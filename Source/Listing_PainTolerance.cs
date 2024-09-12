using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PainTolerance
{
    [StaticConstructorOnStartup]
    public class Listing_PainTolerance : Listing_Standard
    {
        public new float verticalSpacing = 5f;
        public void CustomGraph(string label, float height = 0f, float labelPct = 1f)
        {
            // Draw the labels and axes
            float height2 = (height != 0f) ? height : Text.CalcHeight(label, base.ColumnWidth * labelPct);
            Rect rect = base.GetRect(height2, labelPct);
            rect.width = Math.Min(rect.width + 24f, base.ColumnWidth);
            Widgets.Label(new Rect(rect.x, rect.y, 200, 30), "PainToleranceGraph".Translate());

            // Define axis bounds
            float graphHeight = rect.height;
            float graphWidth = graphHeight;
            float xStart = rect.x + 50;
            float yStart = rect.y + 50;
            float extendedBodySizeEnd = ModSettings_PainTolerance.bodySizeEnd + 1f;

            // Draw the X-axis (Body Size)
            Widgets.DrawLineHorizontal(xStart, yStart + graphHeight, graphWidth);
            Widgets.DrawLineVertical(xStart, yStart, graphHeight);

            // Loop through body sizes and plot points
            int steps = 500; // The number of increments for plotting
            for (int i = 0; i <= steps; i++)
            {
                // Calculate the body size and pain tolerance
                float bodySize = Mathf.Lerp(0f, extendedBodySizeEnd, (float)i / steps);
                float painTolerance = (bodySize <= ModSettings_PainTolerance.bodySizeStart) ? 1f : CalculatePainTolerance(bodySize);
                float secondCurveTolerance = painTolerance * ModSettings_PainTolerance.insectSensitivityMultiplier;


                // Map values to graph coordinates
                float xPos = xStart + graphWidth * (bodySize) / (extendedBodySizeEnd); // X position mapped
                float yPos = yStart + graphHeight * (1f - painTolerance);
                float yPosSecond = yStart + graphHeight * (1f - secondCurveTolerance);

                // Draw a small box (point) to represent the graph point
                float blockSize = 1f;

                Rect pointRect = new Rect(xPos - blockSize / 2f, yPos - blockSize / 2f, blockSize, blockSize); // Smaller block
                Widgets.DrawBoxSolid(pointRect, Color.green);
                if (ModSettings_PainTolerance.insectSenstivityBonus)
                {
                    Rect pointRectSecond = new Rect(xPos - blockSize / 2f, yPosSecond - blockSize / 2f, blockSize, blockSize);
                    Widgets.DrawBoxSolid(pointRectSecond, Color.blue);
                }
            }

            // Draw dotted vertical lines at bodySizeStart, bodySizeMid, and bodySizeEnd
            float startXPos = xStart + graphWidth * (ModSettings_PainTolerance.bodySizeStart - 0f) / (extendedBodySizeEnd - 0f);
            float midXPos = xStart + graphWidth * (ModSettings_PainTolerance.bodySizeMid - 0f) / (extendedBodySizeEnd - 0f);
            float endXPos = xStart + graphWidth * (ModSettings_PainTolerance.bodySizeEnd - 0f) / (extendedBodySizeEnd - 0f);

            // Draw dotted vertical line at the start point (bodySizeStart)
            DrawDottedVerticalLine(startXPos, yStart, graphHeight, Color.red);
            DrawDottedVerticalLine(midXPos, yStart, graphHeight, Color.green);

            // Draw dotted vertical line at the end point (bodySizeEnd)
            DrawDottedVerticalLine(endXPos, yStart, graphHeight, Color.red);
            base.Gap(60f);
        }
        public float CalculatePainTolerance(float bodySize)
        {
            // Define start, end, and midpoint (where pain tolerance = 0.5)
            float midpoint = ModSettings_PainTolerance.bodySizeMid;
            float midpointValue = (1f - ModSettings_PainTolerance.painToleranceEnd) / 2f;
            // End point where the curve approaches endTolerance

            // Calculate the decay constant dynamically based on the start and midpoint
            float decayConstant = -Mathf.Log(midpointValue) / (midpoint - ModSettings_PainTolerance.bodySizeStart);

            // If the body size is less than or equal to the start, return 1 (full pain tolerance)
            if (bodySize <= ModSettings_PainTolerance.bodySizeStart)
            {
                return 1f;
            }
            // Calculate the dynamic decay, ensuring the pain tolerance approaches endTolerance at bodySizeEnd
            else if (bodySize <= ModSettings_PainTolerance.bodySizeEnd)
            {
                float normalizedBodySize = (bodySize - ModSettings_PainTolerance.bodySizeStart) / (ModSettings_PainTolerance.bodySizeEnd - ModSettings_PainTolerance.bodySizeStart);
                return ModSettings_PainTolerance.painToleranceEnd + (1f - ModSettings_PainTolerance.painToleranceEnd) * (1f - normalizedBodySize) * Mathf.Exp(-decayConstant * (bodySize - ModSettings_PainTolerance.bodySizeStart));
            }
            // For body sizes beyond the end, return the end tolerance (no further decay)
            else
            {
                return ModSettings_PainTolerance.painToleranceEnd;
            }
        }
        private void DrawDottedVerticalLine(float x, float yStart, float height, Color color)
        {
            float dotHeight = 5f; // Height of each dot
            float gapHeight = 3f; // Gap between dots

            // Save the current GUI color
            Color previousColor = GUI.color;

            // Set the GUI color to the desired color
            GUI.color = color;

            // Draw the dotted line with color
            for (float y = yStart; y < yStart + height; y += dotHeight + gapHeight)
            {
                Widgets.DrawLineVertical(x, y, dotHeight);  // Draw each small vertical segment
            }

            // Restore the previous GUI color
            GUI.color = previousColor;
        }
        public void CustomIntBoxWithButtons(string label, ref float intValue, float minValue, float maxValue, float stepperValue, string tooltip = null, float height = 0f, float labelPct = 1f)
        {
            float height2 = (height != 0f) ? height : Text.CalcHeight(label, base.ColumnWidth * labelPct);
            Rect rect = base.GetRect(height2, labelPct);
            rect.width = Math.Min(rect.width + 24f, base.ColumnWidth);
            if (this.BoundingRectCached == null || rect.Overlaps(this.BoundingRectCached.Value))
            {
                if (!tooltip.NullOrEmpty())
                {
                    if (Mouse.IsOver(rect))
                    {
                        Widgets.DrawHighlight(rect);
                    }
                    TooltipHandler.TipRegion(rect, tooltip);
                }
                CustomWidgets.DrawIntOptionWithButtons(rect, ref intValue, label, minValue, maxValue, stepperValue);
            }
            base.Gap(this.verticalSpacing);
        }
        public void CustomCheckboxLabeled(string label, ref bool checkOn, string tooltip = null, float height = 0f, float labelPct = 1f)
        {
            float height2 = (height != 0f) ? height : Text.CalcHeight(label, base.ColumnWidth * labelPct);
            Rect rect = base.GetRect(height2, labelPct);
            rect.width = Math.Min(rect.width + 24f, base.ColumnWidth);
            if (this.BoundingRectCached == null || rect.Overlaps(this.BoundingRectCached.Value))
            {
                if (!tooltip.NullOrEmpty())
                {
                    if (Mouse.IsOver(rect))
                    {
                        Widgets.DrawHighlight(rect);
                    }
                    TooltipHandler.TipRegion(rect, tooltip);
                }
                Widgets.CheckboxLabeled(rect, label, ref checkOn, false, null, null, false, false);
            }
            base.Gap(this.verticalSpacing);
        }

        public void CustomDropdownLabeledEnum<T>(string label, ref T selectedValue, Dictionary<T, Action> enumActions, string tooltip = null, float height = 0f, float labelPct = 1f, float dropdownWidthFactor = 0.75f) where T : Enum
        {
            float height2 = (height != 0f) ? height : Text.CalcHeight(label, base.ColumnWidth * labelPct);
            Rect rect = base.GetRect(height2, labelPct);
            float labelWidth = rect.width * 0.4f;
            float dropdownWidth = rect.width * dropdownWidthFactor;
            float rightPadding = 10f;
            Rect dropdownRect = new Rect(rect.xMax - dropdownWidth - rightPadding, rect.y, dropdownWidth, rect.height);
            Rect labelRect = new Rect(rect.x, rect.y, rect.width - dropdownWidth - rightPadding, rect.height);
            if (this.BoundingRectCached == null || rect.Overlaps(this.BoundingRectCached.Value))
            {
                if (!tooltip.NullOrEmpty())
                {
                    if (Mouse.IsOver(dropdownRect))
                    {
                        Widgets.DrawHighlight(dropdownRect);
                    }
                    TooltipHandler.TipRegion(dropdownRect, tooltip);
                }
                TextAnchor originalAnchor = Text.Anchor;
                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(labelRect, label);
                Text.Anchor = originalAnchor;
                string translatedSelectedValue = ("zoomForTracking_" + selectedValue.ToString()).Translate();
                if (Widgets.ButtonText(dropdownRect, translatedSelectedValue))
                {
                    List<FloatMenuOption> options = new List<FloatMenuOption>();

                    foreach (T enumValue in Enum.GetValues(typeof(T)))
                    {
                        options.Add(new FloatMenuOption(enumValue.ToString(), () =>
                        {
                            if (enumActions != null && enumActions.ContainsKey(enumValue))
                            {
                                enumActions[enumValue]?.Invoke();
                            }
                            else
                            {
                                Log.Warning("No action defined for: " + enumValue.ToString());
                            }
                        }));
                    }
                    Find.WindowStack.Add(new FloatMenu(options));
                }
            }
            base.Gap(this.verticalSpacing);
        }
        public float CustomSliderLabel(string label, float val, float min, float max, float labelPct = 0.5f, string tooltip = null, string label2 = null, string rightLabel = null, string leftLabel = null, float roundTo = -1f)
        {
            Rect rect = base.GetRect(30f, 1f);
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect.LeftPart(labelPct), label);
            if (tooltip != null)
            {
                TooltipHandler.TipRegion(rect.LeftPart(labelPct), tooltip);
            }
            Text.Anchor = TextAnchor.UpperLeft;
            float result = CustomWidgets.HorizontalSlider(rect.RightPart(1f - labelPct), val, min, max, true, label2, leftLabel, rightLabel, roundTo);
            base.Gap(this.verticalSpacing);
            return result;
        }

        private Rect ClampRectToScreen(Rect rect)
        {
            float screenWidth = UI.screenWidth;
            float screenHeight = UI.screenHeight;

            if (rect.xMax > screenWidth)
            {
                rect.x -= (rect.xMax - screenWidth);
            }
            if (rect.x < 0)
            {
                rect.x = 0;
            }

            if (rect.yMax > screenHeight)
            {
                rect.y -= (rect.yMax - screenHeight);
            }
            if (rect.y < 0)
            {
                rect.y = 0;
            }

            return rect;
        }


    }
}