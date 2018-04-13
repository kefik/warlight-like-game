namespace GameHandlersLib
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using Common.Extensions;

    public class PlayerColorPicker
    {
        private readonly IList<KnownColor> colorsToPick;
        private readonly HashSet<KnownColor> pickedColors;

        public PlayerColorPicker(int maximumPlayersCount)
        {
            colorsToPick = new List<KnownColor>()
            {
                KnownColor.Blue,
                KnownColor.Red,
                KnownColor.Green,
                KnownColor.Orange,
                KnownColor.Yellow,
                KnownColor.YellowGreen,
                KnownColor.Pink,
                KnownColor.Cyan,
                KnownColor.Gray,
                KnownColor.Olive,
                KnownColor.DarkOrange
            }.Take(maximumPlayersCount).ToList();

            pickedColors = new HashSet<KnownColor>();
        }

        public KnownColor? PickAny()
        {
            if (colorsToPick.Count == pickedColors.Count)
            {
                return null;
            }

            var color = colorsToPick.First(x => !pickedColors.Contains(x));
            pickedColors.Add(color);
            return color;
        }

        public KnownColor? PickNext(KnownColor currentColor)
        {
            // one color available => its the one passed in parameter => cannot pick next
            if (pickedColors.Count >= colorsToPick.Count - 1)
            {
                return null;
            }

            bool takeNext = false;
            // find the color and take next non-picked color
            foreach (KnownColor knownColor in colorsToPick.RepeatInRound())
            {
                if (!takeNext && knownColor == currentColor)
                {
                    takeNext = true;
                }
                else if (takeNext && !pickedColors.Contains(knownColor))
                {
                    pickedColors.Add(knownColor);
                    return knownColor;
                }
            }

            throw new ArgumentException();
        }

        public KnownColor? PickPrevious(KnownColor currentColor)
        {
            // one color available => its the one passed in parameter => cannot pick next
            if (pickedColors.Count >= colorsToPick.Count - 1)
            {
                return null;
            }

            bool takeNext = false;
            // find the color and take next non-picked color
            foreach (KnownColor knownColor in EnumerableExtensions.RepeatInRound<KnownColor>(colorsToPick.Reverse()))
            {
                if (!takeNext && knownColor == currentColor)
                {
                    takeNext = true;
                }
                else if (takeNext && !pickedColors.Contains(knownColor))
                {
                    pickedColors.Add(knownColor);
                    return knownColor;
                }
            }

            throw new ArgumentException();
        }

        public void ReturnColor(KnownColor color)
        {
            pickedColors.Remove(color);
        }

        public void Reset()
        {
            pickedColors.Clear();
        }
    }
}