using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BattleSimulator.UI
{
    [AddComponentMenu("UI/UIGrid")]
    public class UIGrid : MaskableGraphic, ILayoutSelfController
    {
        private static readonly Vector2 UV_TOP_LEFT = Vector2.zero;
        private static readonly Vector2 UV_BOTTOM_LEFT = new Vector2(0, 1);
        private static readonly Vector2 UV_TOP_RIGHT = new Vector2(1, 0);
        private static readonly Vector2 UV_BOTTOM_RIGHT = new Vector2(1, 1);

        [SerializeField]
        Texture texture;

        public float LineThickness = 2;
        public float GridSize = 128;

        [SerializeField]
        private float gridScale = 1.0f;
        public float GridScale {
            get {
                return gridScale;
            }
            set {
                gridScale = value;
                SetVerticesDirty();
            }
        }

        [SerializeField]
        private Vector2 gridOffset = new Vector2(0, 0);

        public Vector2 GridOffset {
            get {
                return gridOffset;
            }
            set {
                gridOffset = value;
                SetVerticesDirty();
            }
        }

        public override Texture mainTexture {
            get {
                return texture == null ? s_WhiteTexture : texture;
            }
        }

        /// <summary>
            /// Texture to be used.
            /// </summary>
        public Texture Texture {
            get {
                return texture;
            }
            set {
                if (texture == value)
                    return;
                texture = value;
                SetVerticesDirty();
                SetMaterialDirty();
            }
        }

        public void SetLayoutHorizontal()
        {
            SetVerticesDirty();
        }

        public void SetLayoutVertical()
        {
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            var sizeX = rectTransform.rect.width;
            var sizeY = rectTransform.rect.height;

            float s = GridScale * GridSize;

            var l = -rectTransform.pivot.x * sizeX;
            var r = l + sizeX;
            var t = -rectTransform.pivot.y * sizeY;
            var b = t + sizeY;

            int xs = (int)(sizeX / s) + 2;
            int ys = (int)(sizeY / s) + 2;

            //var offsetX = l + (gridOffset.x % s);
            //var offsetY = b + (gridOffset.y % s);

            var center = new Vector2((r + l) * 0.5f, (t + b) * 0.5f);
            var offsetX = center.x - (xs / 2) * s + (gridOffset.x % s);
            var offsetY = center.y + (ys / 2) * s + (gridOffset.y % s);

            vh.Clear();

            for (int x = -1; x <= xs; x++)
            {
                float xx = x * s + offsetX;
                UIVertex[] q = new UIVertex[] {
                new UIVertex() { color=color, position=new Vector2(xx, t), uv0=UV_BOTTOM_LEFT},
                new UIVertex() {color=color, position=new Vector2(xx+LineThickness, t), uv0=UV_TOP_LEFT},
                new UIVertex() {color=color, position=new Vector2(xx+LineThickness, b), uv0=UV_TOP_RIGHT},
                new UIVertex() {color=color, position=new Vector2(xx, b), uv0=UV_BOTTOM_RIGHT}
            };
                vh.AddUIVertexQuad(q);
            }

            for (int y = -1; y <= ys; y++)
            {
                float yy = y * -s + offsetY;
                UIVertex[] q = new UIVertex[] {
                new UIVertex() {color=color, position=new Vector2(l, yy), uv0=UV_TOP_LEFT},
                new UIVertex() {color=color, position=new Vector2(l, yy+LineThickness), uv0=UV_BOTTOM_LEFT},
                new UIVertex() {color=color, position=new Vector2(r,yy+LineThickness), uv0=UV_BOTTOM_RIGHT},
                new UIVertex() {color=color, position=new Vector2(r,yy), uv0=UV_TOP_RIGHT}
            };
                vh.AddUIVertexQuad(q);
            }
        }
    }
}
