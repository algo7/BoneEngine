namespace BoneEngine
{
    /// <summary>
    /// The engine as plain rules (no Unity), so they're unit tested: which gear pushes how hard and in which direction,
    /// when a bone is taken and how it drains, and when the steerer may take the boat over.
    /// </summary>
    internal static class EngineRules
    {
        /// <summary>The fuel: the bone fragments item's shared name (same on every copy, every language).</summary>
        public const string FuelItem = "$item_bonefragments";

        /// <summary>High gear's push per physics tick, as a multiple of the boat's own good-wind full-sail push.</summary>
        public const float EnginePower = 1f;

        /// <summary>High-gear seconds one bone lasts; lower gears drain it proportionally slower.</summary>
        public const float BurnSeconds = 15f;

        /// <summary>The boat's owner must have held it this long before the steerer takes it (closes the open-hold race).</summary>
        public const float OwnerStableSeconds = 2f;

        /// <summary>How often the steerer checks whether to take the boat.</summary>
        public const float ClaimInterval = 1f;

        /// <summary>The sail gear is the engine gear: rowing and reverse low, half sail medium, full sail high.</summary>
        public static float GearFraction(Ship.Speed gear)
        {
            switch (gear)
            {
                case Ship.Speed.Back:
                case Ship.Speed.Slow: return 1f / 3f;
                case Ship.Speed.Half: return 2f / 3f;
                case Ship.Speed.Full: return 1f;
                default: return 0f;
            }
        }

        /// <summary>+1 forward, -1 in reverse, 0 stopped.</summary>
        public static int Direction(Ship.Speed gear)
        {
            if (gear == Ship.Speed.Back) return -1;
            return gear == Ship.Speed.Stop ? 0 : 1;
        }

        /// <summary>The impulse to add this tick (the game's sail impulse is also sailFactor × mass per tick).</summary>
        public static float Impulse(float sailForceFactor, float fraction, float mass)
        {
            return EnginePower * sailForceFactor * fraction * mass;
        }

        /// <summary>The owner's game runs the engine only for its own steering, or when nobody steers (sails left up).</summary>
        public static bool RunsHere(bool localSteering, bool anyoneSteering)
        {
            return localSteering || !anyoneSteering;
        }

        /// <summary>No bone loaded (the remaining time of the loaded one is used up).</summary>
        public static bool NeedsBone(float remaining)
        {
            return remaining <= 0f;
        }

        /// <summary>A bone may be taken from the hold: never under an open chest window, never from an empty hold.</summary>
        public static bool CanLoad(bool holdOpen, int bones)
        {
            return !holdOpen && bones > 0;
        }

        /// <summary>What's left of the loaded bone after this tick's engine time (dt × gear fraction), floored at 0.</summary>
        public static float Drain(float remaining, float dt, float fraction)
        {
            if (fraction <= 0f || dt <= 0f) return remaining;
            var left = remaining - dt * fraction;
            return left > 0f ? left : 0f;
        }

        /// <summary>The steerer takes the boat only when not owner, nobody has the hold open, and the owner has been stable.</summary>
        public static bool ShouldClaim(bool steering, bool owner, bool holdOpen, float ownerStableSeconds)
        {
            return steering && !owner && !holdOpen && ownerStableSeconds >= OwnerStableSeconds;
        }

        /// <summary>The one line the mod shows: the item's name in the player's language, then the count.</summary>
        public static string CountLine(string itemName, int count)
        {
            return itemName + ": " + count;
        }
    }
}
