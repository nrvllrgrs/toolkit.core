namespace ToolkitEngine.VisualScripting
{
    public static class EventHooks
    {
        // TimedCurve
        public const string OnTimedCurveBeginCompleted = nameof(OnTimedCurveBeginCompleted);
        public const string OnTimedCurveEndCompleted = nameof(OnTimedCurveEndCompleted);
        public const string OnTimedCurvePaused = nameof(OnTimedCurvePaused);
        public const string OnTimedCurvePlayed = nameof(OnTimedCurvePlayed);
        public const string OnTimedCurveTimeChanged = nameof(OnTimedCurveTimeChanged);

        // Set
        public const string OnSetEmpty = nameof(OnSetEmpty);
        public const string OnSetItemAdded = nameof(OnSetItemAdded);
        public const string OnSetItemRemoved = nameof(OnSetItemRemoved);

        // PoolItem
        public const string OnPoolItemGet = nameof(OnPoolItemGet);
        public const string OnPoolItemSpawned = nameof(OnPoolItemSpawned);
        public const string OnPoolItemReleased = nameof(OnPoolItemReleased);
    }
}