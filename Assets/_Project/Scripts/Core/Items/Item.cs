using System;
using UnityEngine;
using GHSS.Core.Board;

namespace GHSS.Core.Items
{
    /// <summary>
    /// The one and only item component, for any level. Its level lives in
    /// <see cref="Definition"/> (data), never in the GameObject name.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class Item : MonoBehaviour, IBoardObject, IVisualPiece
    {
        [SerializeField] private SpriteRenderer visual;

        public ItemDefinition Definition { get; private set; }
        public int Level => Definition.Level;
        public SpriteRenderer Visual => visual;

        public Vector2Int? BoardPosition { get; private set; }

        public void Initialize(ItemDefinition definition)
        {
            Definition = definition != null ? definition : throw new ArgumentNullException(nameof(definition));

            if (visual != null)
            {
                visual.sprite = definition.Icon;
                visual.color = definition.Color;
            }
        }

        public void SetBoardPosition(Vector2Int? position)
        {
            BoardPosition = position;
        }
    }
}
