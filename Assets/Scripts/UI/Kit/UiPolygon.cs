using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Kit {
    /// <summary>
    /// Wypukły wielokąt UI (kształty z makiety: panel HUD ze ściętymi rogami, ikony żyć, znacznik menu).
    /// Punkty w układzie znormalizowanym prostokąta (0,0 = lewy dół, 1,1 = prawy góra), opcjonalny gradient pionowy.
    /// </summary>
    [RequireComponent(typeof(CanvasRenderer))]
    public class UiPolygon : MaskableGraphic {
        [SerializeField] private List<Vector2> points = new List<Vector2> {
            new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(1f, 0f)
        };
        [SerializeField] private bool useGradient;
        [SerializeField] private Color bottomColor = Color.black;
        [Tooltip("Gradient od lewej (color) do prawej (bottomColor) zamiast od góry do dołu")]
        [SerializeField] private bool horizontalGradient;

        public void SetPoints(IList<Vector2> normalizedPoints) {
            points.Clear();
            points.AddRange(normalizedPoints);
            SetVerticesDirty();
        }

        public void SetGradient(Color top, Color bottom, bool horizontal = false) {
            useGradient = true;
            horizontalGradient = horizontal;
            color = top;
            bottomColor = bottom;
            SetVerticesDirty();
        }

        protected override void OnPopulateMesh(VertexHelper vh) {
            vh.Clear();
            if (points == null || points.Count < 3) return;

            var r = GetPixelAdjustedRect();
            var center = Vector2.zero;
            foreach (var p in points) center += p;
            center /= points.Count;

            AddVertex(vh, r, center);
            foreach (var p in points) AddVertex(vh, r, p);
            for (var i = 0; i < points.Count; i++) vh.AddTriangle(0, i + 1, (i + 1) % points.Count + 1);
        }

        private void AddVertex(VertexHelper vh, Rect r, Vector2 n) {
            var c = !useGradient ? color : horizontalGradient ? Color.Lerp(color, bottomColor, n.x) : Color.Lerp(bottomColor, color, n.y);
            vh.AddVert(new Vector3(r.xMin + n.x * r.width, r.yMin + n.y * r.height), c, Vector2.zero);
        }
    }
}
