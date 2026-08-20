using System;

namespace Splatter.AI.Tests.Stubs {
    public class TrackingObservingDecorator : ObservingDecorator {
        private readonly Func<bool> condition;

        public int ConditionEvaluations { get; private set; }
        public int StartObservingCalls { get; private set; }
        public int StopObservingCalls { get; private set; }

        public TrackingObservingDecorator(Func<bool> condition, AbortMode mode)
            : base("Tracking observer", mode) {

            this.condition = condition;
        }

        public void RaiseChanged() {
            OnObservedValueChanged();
        }

        protected override bool IsConditionMet() {
            ConditionEvaluations++;

            return condition();
        }

        protected override void StartObserving() {
            StartObservingCalls++;
        }

        protected override void StopObserving() {
            StopObservingCalls++;
        }
    }
}
