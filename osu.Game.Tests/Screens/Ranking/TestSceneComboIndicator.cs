// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using NUnit.Framework;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Osu;
using osu.Game.Rulesets.Scoring;
using osu.Game.Scoring;
using osu.Game.Screens.Ranking.Expanded;
using osu.Game.Tests.Beatmaps;

namespace osu.Game.Tests.Screens.Ranking
{
    [TestFixture]
    public class TestSceneComboIndicator : OsuTestScene
    {
        [Test]
        public void TestXFCIndicator()
        {
            var score = createScore(new Dictionary<HitResult, int>
            {
                { HitResult.Great, 100 },
                { HitResult.Ok, 0 },
                { HitResult.Meh, 0 },
                { HitResult.Miss, 0 }
            }, maxCombo: 100);

            var indicator = new ComboIndicator(score);
            Add(indicator);
        }

        [Test]
        public void TestFCIndicator()
        {
            var score = createScore(new Dictionary<HitResult, int>
            {
                { HitResult.Great, 95 },
                { HitResult.Ok, 5 },
                { HitResult.Meh, 0 },
                { HitResult.Miss, 0 }
            }, maxCombo: 100);

            var indicator = new ComboIndicator(score);
            Add(indicator);
        }

        [Test]
        public void TestSDCBIndicator()
        {
            var score = createScore(new Dictionary<HitResult, int>
            {
                { HitResult.Great, 90 },
                { HitResult.Ok, 5 },
                { HitResult.Meh, 3 },
                { HitResult.Miss, 2 }
            }, maxCombo: 90);

            var indicator = new ComboIndicator(score);
            Add(indicator);
        }

        [Test]
        public void TestNoIndicator()
        {
            var score = createScore(new Dictionary<HitResult, int>
            {
                { HitResult.Great, 80 },
                { HitResult.Ok, 5 },
                { HitResult.Meh, 5 },
                { HitResult.Miss, 10 }
            }, maxCombo: 80);

            var indicator = new ComboIndicator(score);
            Add(indicator);
        }

        private ScoreInfo createScore(Dictionary<HitResult, int> statistics, int maxCombo)
        {
            var beatmap = CreateBeatmap(new OsuRuleset().RulesetInfo);

            return new ScoreInfo
            {
                Statistics = statistics,
                MaxCombo = maxCombo,
                BeatmapInfo = beatmap.BeatmapInfo,
                Ruleset = new OsuRuleset().RulesetInfo,
                Mods = new Mod[] { },
                HitEvents = new List<HitEvent>()
            };
        }
    }
}
