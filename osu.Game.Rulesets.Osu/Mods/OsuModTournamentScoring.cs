// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Localisation;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Osu.Objects;

namespace osu.Game.Rulesets.Osu.Mods
{
    public class OsuModTournamentScoring : Mod, IApplicableToHitObject
    {
        public override string Name => "Tournament Scoring";
        public override string Acronym => "TS";
        public override LocalisableString Description => "Adjusts scoring to tournament values: 300s=full points, 100s=95% of 300s, 50s=20% of 300s.";
        public override double ScoreMultiplier => 1.0;
        public override ModType Type => ModType.Conversion;

        public override bool Ranked => true;

        public void ApplyToHitObject(HitObject hitObject)
        {
            // This mod doesn't modify hit objects, just scoring
        }
    }
}
