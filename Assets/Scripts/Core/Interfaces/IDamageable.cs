// Assets/Scripts/Core/Interfaces/IDamageable.cs
using UnityEngine;

namespace GothicShooter.Core
{
    /// <summary>
    /// Universal interface for all entities that can take damage.
    /// Used by: Player, Enemies, Destructible Props, Traps, Bosses, etc.
    /// Ensures consistent damage pipeline across the entire game.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// Apply damage to this entity.
        /// </summary>
        /// <param name="amount">Amount of damage to apply</param>
        /// <param name="source">GameObject that caused the damage (can be null for environmental damage)</param>
        void TakeDamage(float amount, GameObject source = null);

        /// <summary>
        /// Check if entity is dead.
        /// </summary>
        bool IsDead { get; }

        /// <summary>
        /// Current health value (read-only for external systems).
        /// </summary>
        float CurrentHealth { get; }

        /// <summary>
        /// Maximum health value.
        /// </summary>
        float MaxHealth { get; }

        /// <summary>
        /// Transform reference for visual feedback, spawning particles, ragdolls, etc.
        /// </summary>
        Transform Transform { get; }
    }
}
