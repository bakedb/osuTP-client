// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable

using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Scoring;
using osu.Game.Users.Drawables;
using osuTK;

namespace osu.Game.Screens.Ranking.Expanded
{
    /// <summary>
    /// The content that appears in the middle section of the <see cref="ScorePanel"/>.
    /// </summary>
    public partial class ExpandedPanelTopContent : CompositeDrawable
    {
        private readonly APIUser user;
        private readonly ScoreInfo score;

        private Sample appearanceSample;

        private readonly bool playAppearanceSound;

        /// <summary>
        /// Creates a new <see cref="ExpandedPanelTopContent"/>.
        /// </summary>
        /// <param name="user">The <see cref="APIUser"/> to display.</param>
        /// <param name="score">The <see cref="ScoreInfo"/> for combo indicator.</param>
        /// <param name="playAppearanceSound">Whether appearance sample should play</param>
        public ExpandedPanelTopContent(APIUser user, ScoreInfo score, bool playAppearanceSound = false)
        {
            this.user = user;
            this.score = score;
            this.playAppearanceSound = playAppearanceSound;
            Anchor = Anchor.TopCentre;
            Origin = Anchor.Centre;

            AutoSizeAxes = Axes.Both;
        }

        [BackgroundDependencyLoader]
        private void load(AudioManager audio)
        {
            appearanceSample = audio.Samples.Get(@"Results/score-panel-top-appear");

            InternalChild = new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Children = new Drawable[]
                {
                    new UpdateableAvatar(user)
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Size = new Vector2(80),
                        CornerRadius = 20,
                        CornerExponent = 2.5f,
                        Masking = true,
                    },
                    new Container
                    {
                        AutoSizeAxes = Axes.Both,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Children = new Drawable[]
                        {
                            new ClickableUsername(user)
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                            },
                            score != null ? new ComboIndicator(score)
                            {
                                Anchor = Anchor.CentreRight,
                                Origin = Anchor.CentreRight,
                                Margin = new MarginPadding { Left = 10 }
                            } : Empty()
                        }
                    }
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            if (playAppearanceSound)
                appearanceSample?.Play();
        }
    }
}
