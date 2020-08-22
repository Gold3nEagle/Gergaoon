using System.Collections.Generic;
using System.Linq;
using SweetSugar.Scripts.Blocks;
using SweetSugar.Scripts.Core;

namespace SweetSugar.Scripts.Effects
{
    /// <summary>
    /// Cloud animation effect for levels with not only down direction
    /// </summary>
    public static class DirectionCloudEffect
    {
        public static void SetGroupSquares(Square[] squaresArray)
        {
            var groups = new List<List<Square>>();


            foreach (var square in squaresArray)
            {
                // groups = square.SetGroupSquares(groups);
                groups = square.GetGroupsSquare(groups);
            }
            groups.RemoveAll(i => i.Count() < LevelManager.THIS.field.enterPoints);
            foreach (var group in groups)
            {
                foreach (var square in group)
                {
                    square.squaresGroup = group;
                }
            }
            var list = squaresArray.Where(i => i.squaresGroup.Count < LevelManager.THIS.field.enterPoints);
            groups.Clear();
            foreach (var square in list)
            {
                // groups = square.SetGroupSquaresRest(list, groups);
                groups = square.GetGroupsSquare(groups, null, false);
            }
            foreach (var group in groups)
            {
                foreach (var square in group)
                {
                    square.squaresGroup = group;
                }
            }
        }

    }
}
