using System;
using System.Collections.Generic;
using Celeste.Mod.CelesteNet.Client;
using Celeste.Mod.HeartWars.Controllers;
using Celeste.Mod.HeartWars.Entities.Triggers;
using Celeste.Mod.HeartWars.Inventory;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;

namespace Celeste.Mod.HeartWars.Entities.Renderers {
    public class HeartWarsRenderer : Entity {
        public HeartWarsRenderer()
            : base(Vector2.Zero) {
            Tag |= TagsExt.SubHUD;
            Tag |= Tags.FrozenUpdate;
            this.Depth = -10000000;
        }

        public HeartWarsRenderer initialize(Entity Tracking) {
            this.Tracking = Tracking;
            return this;
        }

        public Entity Tracking;

        private static MTexture[] numbers;

        public static SoundSource soundSource;

        public float Alpha = 1f;
        protected float time = 0f;
        protected bool popupShown = false;
        protected float popupTime = 100f;
        protected bool timeRateSet = false;

        public HashSet<string> TimeRateSkip = new();
        public bool ForceSetTimeRate;

        public float Angle = 0f;
        
        public int Selected = -1;
        protected int PrevSelected;
        protected float selectedTime = 0f;

        public MTexture BG = GFX.Gui["celestenet/iconwheel/bg"];
        public MTexture Line = GFX.Gui["celestenet/iconwheel/line"];
        public MTexture Indicator = GFX.Gui["celestenet/iconwheel/indicator"];

        public Color TextSelectColorA = Calc.HexToColor("84FF54");
        public Color TextSelectColorB = Calc.HexToColor("FCFF59");

        private static float getScale() {
            return Controller.level.Zoom * ((320f - Controller.level.ScreenPadding * 2f) / 320f);
        }

        public static void Initialize() {
            MTexture source = GFX.Game["pico8/font"];
            soundSource = new SoundSource();
            numbers = new MTexture[10];
            int index = 0;
            for (int i = 104; index < 4; i += 4) {
                numbers[index++] = source.GetSubtexture(i, 0, 3, 5);
            }
            for (int i = 0; index < 10; i += 4) {
                numbers[index++] = source.GetSubtexture(i, 6, 3, 5);
            }
        }

		public override void Render() {
            base.Render();
            float H = Engine.Graphics.PreferredBackBufferHeight;
            float W = Engine.Graphics.PreferredBackBufferWidth;
            float scale = Calc.Min(1f * H / 9, 1f * W / 16);
            float scaledH = scale * 9;
            float scaledW = scale * 16;
            Player player = Scene.Tracker.GetEntity<Player>();
            if (player != null && Controller.featuresEnabled) {
                string health = ((int)PlayerInfo.PlayerHealth).ToString() +
                    (PlayerInfo.PlayerExtraHealth > 1f ? "+" + ((int)PlayerInfo.PlayerExtraHealth).ToString() : "");
                string money = PlayerInfo.PlayerMoney.ToString();
                string combined = health + " hp " + money + " gold";
                int totalWidth = combined.Length * 4 - 1;
                Color outlineColor = PlayerInfo.PlayerExtraHealth > 1f ? Color.Gold : Color.Black;
                Camera cam = Controller.level.Camera;
                float yoffset = cam.Top - cam.Bottom;
                Vector2 size = CelesteNetClientFont.Measure(combined) * getScale();
                Vector2 basePosition = new Vector2(Engine.Width / 2, Engine.Height);
                ActiveFont.DrawOutline(
                    combined,
                    basePosition,
                    new Vector2(0.5f, 1f),
                    Vector2.One * getScale(),
                    Color.White, 2f,
                    outlineColor
                );
            }
            renderInventory();
        }

        private void renderInventory() {
            MouseState mouseState = MInput.Mouse.CurrentState;
            bool shown = (mouseState.RightButton == ButtonState.Pressed);
            if (!shown && Selected == -1) return;
            bool shopMode = ShopTrigger.active;
            List<Item> list = new List<Item>();
            InventoryController.inventory.ForEach(e => {
                if (shopMode && e.cost > 0) {
                    list.Add(e);
                }
                if (!shopMode && e.count > 0) {
                    list.Add(e);
                }
            });
            
            if (shown) {
                Vector2 mousePosition = new Vector2(mouseState.X, mouseState.Y);
                float H = Engine.Graphics.PreferredBackBufferHeight;
                float W = Engine.Graphics.PreferredBackBufferWidth;
                float scale = Calc.Min(H / 9f, W / 16f);
                float scaledH = scale * 9;
                float scaledW = scale * 16;
                float deltaH = H / 2 - scaledH / 2;
                float deltaW = W / 2 - scaledW / 2;
                mousePosition.X -= deltaW;
                mousePosition.Y -= deltaH;
                Camera cam = Controller.level.Camera;
                float viewScale = (float) Engine.ViewWidth / Engine.Width;
                Vector2 mouseWorldPosition = Calc.Floor(Controller.level.ScreenToWorld(mousePosition / viewScale));
                Vector2 gunline = (mouseWorldPosition - Controller.player.Position).SafeNormalize();
                float angle = gunline.Angle();
                float start = (-0.5f / list.Count) * 2f * (float) Math.PI;
                if (2f * (float) Math.PI + start < angle) {
                    angle -= 2f * (float) Math.PI;
                }
                for (int i = 0; i < list.Count; i++) {
                    float min = ((i - 0.5f) / list.Count) * 2f * (float) Math.PI;
                    float max = ((i + 0.5f) / list.Count) * 2f * (float) Math.PI;
                    if ((min <= angle && angle <= max) ||
                        (min <= (angle + 2f * (float) Math.PI) && (angle + 2f * (float) Math.PI) <= max) ||
                        (min <= (angle - 2f * (float) Math.PI) && (angle - 2f * (float) Math.PI) <= max)) {
                        Selected = i;
                        break;
                    }
                }
            }
            time += Engine.RawDeltaTime;

            if (shopMode && !shown && Selected != -1 && list[Selected]?.cost > 0) {
                list[Selected].buy();
                Selected = -1;
            }

            if (!shopMode && !shown && Selected != -1 && list[Selected]?.count > 0) {
                list[Selected].select();
                Selected = -1;
            }

            selectedTime += Engine.RawDeltaTime;
            if (PrevSelected != Selected) {
                selectedTime = 0f;
                PrevSelected = Selected;
            }

            float popupAlpha;
            float popupScale;

            popupTime += Engine.RawDeltaTime;
            if (shown && !popupShown) {
                popupTime = 0f;
            } else if ((shown && popupTime > 1f) ||
                (!shown && popupTime < 1f)) {
                popupTime = 1f;
            }
            popupShown = shown;

            if (popupTime < 0.1f) {
                float t = popupTime / 0.1f;
                // Pop in.
                popupAlpha = Ease.CubeOut(t);
                popupScale = Ease.ElasticOut(t);

            } else if (popupTime < 1f) {
                // Stay.
                popupAlpha = 1f;
                popupScale = 1f;

            } else {
                float t = (popupTime - 1f) / 0.2f;
                // Fade out.
                popupAlpha = 1f - Ease.CubeIn(t);
                popupScale = 1f - 0.2f * Ease.CubeIn(t);
            }

            float alpha = Alpha * popupAlpha;
            if (Tracking == null) {
                return;
            }

            Level level = SceneAs<Level>();
            if (level == null) {
                return;
            }

            popupScale *= getScale();

            Vector2 pos = Tracking.Position;
            pos.Y -= 8f;

            pos = level.WorldToScreen(pos);

            float radius = BG.Width * 0.5f * 0.75f * popupScale;

            pos = pos.Clamp(
                0f + radius, 0f + radius,
                1920f - radius, 1080f - radius
            );

            BG.DrawCentered(
                pos,
                Color.White * alpha * alpha * alpha,
                Vector2.One * popupScale
            );

            Indicator.DrawCentered(
                pos,
                Color.White * alpha * alpha * alpha,
                Vector2.One * popupScale,
                Angle
            );

            float selectedScale = 1.2f + 0.4f * Calc.Clamp(Ease.CubeOut(selectedTime / 0.1f), 0f, 1f) + (float) Math.Sin(time * 1.8f) * 0.1f;

            for (int i = 0; i < list.Count; i++) {
                Line.DrawCentered(
                    pos,
                    Color.White * alpha * alpha * alpha,
                    Vector2.One * popupScale,
                    ((i + 0.5f) / list.Count) * 2f * (float) Math.PI
                );

                Item item = list[i];
                if (shopMode && item?.cost == 0)
                    continue;
                if (!shopMode && item?.count == 0)
                    continue;

                float a = (i / (float) list.Count) * 2f * (float) Math.PI;
                Vector2 itemPos = pos + new Vector2(
                    (float) Math.Cos(a),
                    (float) Math.Sin(a)
                ) * radius;

                MTexture icon = item.texture;
                if (icon == null)
                    continue;

                Vector2 iconSize = new(icon.Width, icon.Height);
                float iconScale = (180 / Math.Max(iconSize.X, iconSize.Y)) * 0.24f * popupScale;

                icon.DrawCentered(
                    itemPos,
                    Color.White * (Selected == i ? (Calc.BetweenInterval(selectedTime, 0.1f) ? 0.9f : 1f) : 0.7f) * alpha,
                    Vector2.One * (Selected == i ? selectedScale : 1f) * iconScale
                );

                string itemText;
                if (shopMode) {
                    itemText = item.cost.ToString() + " - " + item.count.ToString();
                } else {
                    itemText = item.count.ToString();
                }

                Vector2 textSize = CelesteNetClientFont.Measure(itemText);
                float textScale = (180 / Math.Max(textSize.X, textSize.Y)) * 0.24f * popupScale;

                CelesteNetClientFont.DrawOutline(
                    itemText,
                    itemPos + new Vector2(0, 10f),
                    new(0.5f, 0.5f),
                    Vector2.One * (Selected == i ? selectedScale : 1f) * textScale,
                    (Selected == i ? (Calc.BetweenInterval(selectedTime, 0.1f) ? TextSelectColorA : TextSelectColorB) : Color.LightSlateGray) * alpha,
                    2f,
                    Color.Black * alpha * alpha * alpha
                );
            }
        }

		public override void Update() {
            base.Update();

            if (Tracking == null || Tracking?.Scene != Scene)
                RemoveSelf();
		}
    }
}
