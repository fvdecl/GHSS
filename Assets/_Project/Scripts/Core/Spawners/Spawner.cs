using System;
using UnityEngine;
using GHSS.Core.Board;

namespace GHSS.Core.Spawners
{
    /// <summary>
    /// The one spawner component, for any level - same pattern as <c>Item</c>.
    /// Its level lives in <see cref="Definition"/> (data), never in the GameObject
    /// name, and it never references an item prefab directly.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class Spawner : MonoBehaviour, IBoardObject, IVisualPiece
    {
        [SerializeField] private SpriteRenderer visual;

        public SpawnerDefinition Definition { get; private set; }
        public int Level => Definition.Level;
        public SpriteRenderer Visual => visual;

        public Vector2Int? BoardPosition { get; private set; }

        public void Initialize(SpawnerDefinition definition)
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
