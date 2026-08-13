using System;
using UnityEngine;

namespace GHSS.Core.Spawners
{
    /// <summary>
    /// One entry of a spawn probability table: "item of this level, with this
    /// relative weight". Weights don't need to sum to 100 - they're normalized
    /// at roll time, so a designer can add/remove outcomes freely.
    /// </summary>
    [Serializable]
    public struct SpawnWeight
    {
        [SerializeField, Min(1)] private int itemLevel;
        [SerializeField, Min(0f)] private float weight;

        public SpawnWeight(int itemLevel, float weight)
        {
            this.itemLevel = itemLevel;
            this.weight = weight;
        }

        public int ItemLevel => itemLevel;
        public float Weight => weight;
    }
}
