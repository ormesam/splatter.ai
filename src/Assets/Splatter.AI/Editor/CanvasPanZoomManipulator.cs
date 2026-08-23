using UnityEngine;
using UnityEngine.UIElements;

namespace Splatter.AI.Editor {
    /// <summary>
    /// Wheel zoom centred on the cursor plus left- or middle-button drag panning. The canvas is
    /// read-only and nothing is selectable, so drags may start anywhere, including on a node.
    /// </summary>
    internal sealed class CanvasPanZoomManipulator : Manipulator {
        // Per wheel-delta unit; one Windows notch (±3) is ~×1.16, matching GraphView's feel.
        // The exponential form also absorbs fractional trackpad deltas.
        private const float ZoomStep = 1.05f;

        private readonly BehaviourTreeCanvas canvas;
        private Vector2 panAtDown;
        private Vector3 pointerDownPosition;
        private int capturedPointerId = -1;

        public CanvasPanZoomManipulator(BehaviourTreeCanvas canvas) {
            this.canvas = canvas;
        }

        protected override void RegisterCallbacksOnTarget() {
            target.RegisterCallback<WheelEvent>(OnWheel);
            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
            target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            target.RegisterCallback<PointerUpEvent>(OnPointerUp);
            target.RegisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
        }

        protected override void UnregisterCallbacksFromTarget() {
            target.UnregisterCallback<WheelEvent>(OnWheel);
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
            target.UnregisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
        }

        private void OnWheel(WheelEvent evt) {
            Vector2 cursor = canvas.WorldToLocal(evt.mousePosition);
            float newZoom = Mathf.Clamp(canvas.Zoom * Mathf.Pow(ZoomStep, -evt.delta.y),
                BehaviourTreeCanvas.MinZoom, BehaviourTreeCanvas.MaxZoom);

            // Keep the content point under the cursor fixed: pan' = m - (zoom'/zoom)(m - pan).
            Vector2 newPan = cursor - (newZoom / canvas.Zoom) * (cursor - canvas.Pan);
            canvas.SetViewTransform(newPan, newZoom);
            evt.StopPropagation();
        }

        private void OnPointerDown(PointerDownEvent evt) {
            if (evt.button != 0 && evt.button != 2) {
                return;
            }

            panAtDown = canvas.Pan;
            pointerDownPosition = evt.position;
            capturedPointerId = evt.pointerId;
            target.CapturePointer(evt.pointerId);
            canvas.Focus();
            evt.StopPropagation();
        }

        private void OnPointerMove(PointerMoveEvent evt) {
            if (capturedPointerId != evt.pointerId || !target.HasPointerCapture(evt.pointerId)) {
                return;
            }

            canvas.SetViewTransform(panAtDown + (Vector2)(evt.position - pointerDownPosition), canvas.Zoom);
            evt.StopPropagation();
        }

        private void OnPointerUp(PointerUpEvent evt) {
            if (capturedPointerId != evt.pointerId) {
                return;
            }

            target.ReleasePointer(evt.pointerId);
            capturedPointerId = -1;
            evt.StopPropagation();
        }

        private void OnPointerCaptureOut(PointerCaptureOutEvent evt) {
            // Covers releases that never reach OnPointerUp, e.g. play-mode exit mid-drag.
            capturedPointerId = -1;
        }
    }
}
