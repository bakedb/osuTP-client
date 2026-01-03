// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Rulesets.Scoring;
using osu.Game.Scoring;
using System;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Screens.Ranking.Expanded
{
    /// <summary>
    /// Displays combo status indicator (XFC, FC, SB, SDCB) for a score.
    /// </summary>
    public partial class ComboIndicator : CompositeDrawable
    {
        private readonly ScoreInfo score;

        private OsuSpriteText spriteText = null!;
        private Container backgroundContainer = null!;
        private string displayText = string.Empty;

        public ComboIndicator(ScoreInfo score)
        {
            this.score = score;
            Anchor = Anchor.CentreRight;
            Origin = Anchor.CentreRight;
            AutoSizeAxes = Axes.Both;
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            var comboStatus = getComboStatus(score);
            var (color, text) = getComboDisplay(comboStatus);
            displayText = text; // Assign to field

            InternalChild = new Container
            {
                AutoSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    backgroundContainer = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = color,
                        Alpha = 0.9f,
                        CornerRadius = 12,
                        Masking = true,
                        Child = new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = ColourInfo.GradientVertical(color, color.Darken(0.3f))
                        }
                    },
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Horizontal,
                        Spacing = new Vector2(4, 0),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Children = new Drawable[]
                        {
                            !string.IsNullOrEmpty(displayText) ? Empty() : new SpriteIcon
                            {
                                Icon = FontAwesome.Solid.Star,
                                Size = new Vector2(16),
                                Colour = Color4.White,
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft
                            },
                            spriteText = new OsuSpriteText
                            {
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Text = displayText,
                                Font = OsuFont.Torus.With(size: 20, weight: FontWeight.Bold),
                                Colour = Color4.White,
                                Padding = new MarginPadding { Horizontal = 10, Vertical = 6 },
                                Shadow = true,
                                ShadowColour = color,
                                ShadowOffset = new Vector2(0, 2),
                            }
                        }
                    }
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            // Add pulsing animation for non-empty indicators
            if (!string.IsNullOrEmpty(displayText))
            {
                spriteText.FadeTo(0.7f).Then().FadeTo(1f, 500).Loop();
            }
        }

        public static ComboStatus getComboStatus(ScoreInfo score)
        {
            var statistics = score.Statistics;
            var maxCombo = score.MaxCombo;
            var maxAchievableCombo = score.GetMaximumAchievableCombo();

            // Count different hit types
            int count300 = statistics.TryGetValue(HitResult.Great, out var c300) ? c300 : 0;
            int count100 = statistics.TryGetValue(HitResult.Ok, out var c100) ? c100 : 0;
            int count50 = statistics.TryGetValue(HitResult.Meh, out var c50) ? c50 : 0;
            int countMiss = statistics.TryGetValue(HitResult.Miss, out var cMiss) ? cMiss : 0;

            // Check for XFC (all 300s on combo-affecting objects, no slider breaks)
            int totalComboObjects = count300 + count100 + count50 + countMiss;
            bool hasSliderBreaks = false;
            int nonSliderComboBreaks = 0;

            if (score.HitEvents.Count > 0)
            {
                foreach (var hitEvent in score.HitEvents)
                {
                    if (hitEvent.Result == HitResult.IgnoreMiss)
                    {
                        hasSliderBreaks = true;
                        // Slider end misses don't break combo, so don't count as combo breaks
                    }
                    else if (hitEvent.Result.BreaksCombo())
                    {
                        nonSliderComboBreaks++;
                    }
                }
            }
            else
            {
                // Fallback: check statistics for slider breaks
                hasSliderBreaks = statistics.TryGetValue(HitResult.IgnoreMiss, out var sliderMisses) && sliderMisses > 0;
                // Count combo breaks from statistics (misses, 50s, 100s) but exclude slider end misses
                var ignoreMisses = statistics.TryGetValue(HitResult.IgnoreMiss, out var ignores) ? ignores : 0;
                var sliderEnds = statistics.TryGetValue(HitResult.SliderTailHit, out var ends) ? ends : 0;
                // IgnoreMiss includes missed slider ends, so subtract those from total misses
                var nonSliderMisses = countMiss - ignoreMisses;
                nonSliderComboBreaks = nonSliderMisses + count50 + count100;
            }

            // Check for XFC (all 300s on combo-affecting objects, no slider breaks)
            if (count300 == totalComboObjects && count100 == 0 && count50 == 0 && countMiss == 0 && !hasSliderBreaks)
                return ComboStatus.XFC;

            // Check for SB (full combo but fewer slider end hits than expected)
            var sliderEndMisses = statistics.TryGetValue(HitResult.IgnoreMiss, out var endMisses) ? endMisses : 0;
            var largeTickMisses = statistics.TryGetValue(HitResult.LargeTickMiss, out var tickMisses) ? tickMisses : 0;
            var sliderTailHits = statistics.TryGetValue(HitResult.SliderTailHit, out var tailHits) ? tailHits : 0;

            // Debug output
            System.Diagnostics.Debug.WriteLine($"SB Debug: sliderEndMisses={sliderEndMisses}, largeTickMisses={largeTickMisses}, sliderTailHits={sliderTailHits}, maxCombo={maxCombo}, maxAchievableCombo={maxAchievableCombo}, nonSliderComboBreaks={nonSliderComboBreaks}");

            // SB: Full combo (maxCombo == maxAchievableCombo) but fewer slider end hits than expected
            // This indicates slider end misses that don't break combo
            if (maxCombo == maxAchievableCombo && sliderTailHits < maxAchievableCombo && nonSliderComboBreaks == 0)
                return ComboStatus.SB;

            // Check for FC (full combo - max combo equals maximum achievable)
            if (maxCombo == maxAchievableCombo)
                return ComboStatus.FC;

            // Check for SDCB (Single Digit Combo Breaks) - check this last
            int totalComboBreaks = nonSliderComboBreaks;
            if (totalComboBreaks > 0 && totalComboBreaks <= 9)
                return ComboStatus.SDCB;

            return ComboStatus.None;
        }

        public static (Color4 color, string text) getComboDisplay(ComboStatus status)
        {
            return status switch
            {
                ComboStatus.XFC => (Color4Extensions.FromHex("#FFD700"), "XFC"), // Gold
                ComboStatus.FC => (Color4Extensions.FromHex("#4CAF50"), "FC"),   // Green
                ComboStatus.SB => (Color4Extensions.FromHex("#2196F3"), "SB"),   // Blue
                ComboStatus.SDCB => (Color4Extensions.FromHex("#FF9800"), "SDCB"), // Orange
                _ => (Color4.Transparent, string.Empty)
            };
        }

        public enum ComboStatus
        {
            None,
            XFC,
            FC,
            SB,
            SDCB
        }
    }
}
