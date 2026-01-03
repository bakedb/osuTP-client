// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Osu.Scoring
{
    public partial class OsuTournamentScoreProcessor : OsuScoreProcessor
    {
        private const int base_300_points = 300;
        private const int tournament_100_points = 285; // 95% of 300
        private const int tournament_50_points = 60;   // 20% of 300

        public override int GetBaseScoreForResult(HitResult result)
        {
            return result switch
            {
                HitResult.Great => base_300_points,
                HitResult.Ok => tournament_100_points,
                HitResult.Meh => tournament_50_points,
                _ => base.GetBaseScoreForResult(result)
            };
        }

        protected override double GetComboScoreChange(JudgementResult result)
        {
            // Use tournament scoring values for combo calculation
            int tournamentBaseScore = GetBaseScoreForResult(result.Judgement.MaxResult);
            return tournamentBaseScore * Math.Pow(result.ComboAfterJudgement, COMBO_EXPONENT);
        }
    }
}
