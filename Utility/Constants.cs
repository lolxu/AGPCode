namespace __OasisBlitz.Utility
{
    public static class Constants
    {
        public const int PlayerLayer = 3;
        public const int PenetrableLayer = 6;
        public const int LargePenetrableLayer = 7;
        public const int SandBackfaceStencilLayer = 12;
        public const int LargeSandStencilLayer = 13;

#if DEBUG
        //Debug Constants
        public const bool DebugRootStateChanges = true;
        public const bool DebugLevelLoader = false;
        public const bool DebugSubstateChanges = false;
        public const bool DebugBounce = false;
        public const bool DebugStuck = false;
        public const bool DebugSwarmNavMesh = false;
        public const bool DebugBounceKnightShield = false;
        public const bool DebugIKRun = false;
        public const bool DebugIKIdle = false;
        //Abilities
        public const bool RestrictedMovementOn = false;
#endif
    }
}
