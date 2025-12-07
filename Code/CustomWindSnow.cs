using Celeste;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vitmod
{
    public class CustomWindSnow : Backdrop
    {
        public CustomWindSnow(string colors, string alphas, int amount, float speedX, float speedY, bool ignoreWind) : base()
        {
            this.colors = new List<Color>();
            string[] colorStrings = colors.Split(',');
            foreach(string color in colorStrings)
            {
                this.colors.Add(Calc.HexToColor(color));
            }
            this.alphas = new List<float>();
            string[] alphaStrings = alphas.Split(',');
            foreach(string alpha in alphaStrings)
            {
                this.alphas.Add(float.Parse(alpha));
            }
            this.amount = amount;
            addSpeed = new Vector2(speedX * 100f, speedY * 100f);
            this.ignoreWind = ignoreWind;

            Color = Color.White;
            CameraOffset = Vector2.Zero;
            scale = Vector2.One;
            rotation = 0f;
            visibleFade = 1f;
            positions = new Vector2[amount];
            particleColors = new int[amount];
            Random rng = new Random();
            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = Calc.Random.Range(new Vector2(0f, 0f), new Vector2(640f, 360f));
                particleColors[i] = rng.Next(0, this.colors.Count);
            }
            sines = new SineWave[amount / 15];
            for (int i = 0; i < sines.Length; i++)
            {
                sines[i] = new SineWave(Calc.Random.Range(0.8f, 1.2f), 0f);
                sines[i].Randomize();
            }
        }

        public override void Update(Scene scene)
        {
            base.Update(scene);
            Level level = scene as Level;
            visibleFade = Calc.Approach(visibleFade, IsVisible(level) ? 1f : 0f, Engine.DeltaTime * 2f); 
            foreach(SineWave sine in sines)
            {
                sine.Update();
            }
            windVector = ignoreWind ? Vector2.Zero : level.Wind;
            windVector += addSpeed;
            scale.X = Math.Max(1f, Math.Abs(windVector.Length()) / 100f);
            float vertical = (float)Math.Abs(Math.Sin(Math.Atan2(windVector.Y, windVector.X)));
            scale.X /=  Math.Max(1f - vertical, 0.4f);
            scale.Y = 1f / Math.Max(1f, scale.X * 0.25f);
            rotation %= (float)Math.PI * 2;
            float target_rot = (float)Math.Atan2(windVector.Y, windVector.X);
            if (Math.Abs(rotation - target_rot) == Math.PI)
            {
                rotation = target_rot;
            }
            else
            {
                if (rotation - target_rot > Math.PI)
                {
                    target_rot += (float)Math.PI * 2;
                }
                else if (target_rot - rotation > Math.PI)
                {
                    target_rot -= (float)Math.PI * 2;
                }
            }
            rotation = Calc.Approach(rotation, target_rot, Engine.DeltaTime * 8f);
            if (windVector.X == 0f)
            {
                windVector.Y *= 3f;
            }
            for (int i = 0; i < positions.Length; i++)
            {
                float value = sines[i % sines.Length].Value;
                Vector2 zero = new Vector2(windVector.X + value * 10f, windVector.Y + value * 10f);
                if (windVector.Length() == 0f)
                {
                    zero.Y += 20f;
                }
                positions[i] += zero * Engine.DeltaTime;
            }
        }

        public override void Render(Scene scene)
        {
            base.Render(scene);
            int index = 0;
            float limit = (float)Math.Abs(Math.Sin(Math.Atan2(windVector.Y, windVector.X)));
            if (windVector == Vector2.Zero)
            {
                limit = 0f;
            }
            limit = 0.6f + (1 - limit) * 0.4f;
            foreach(Vector2 init in positions)
            {
                Color color = colors[particleColors[index]];
                if (alphas.Count == colors.Count)
                {
                    color *= visibleFade * alphas[particleColors[index]];
                }
                else
                {
                    color *= visibleFade * alphas[0];
                }
                Vector2 position = init;
                position.Y -= (scene as Level).Camera.Y + CameraOffset.Y;
                position.Y %= 360f;
                if (position.Y < 0f)
                {
                    position.Y += 360f;
                }
                position.X -= (scene as Level).Camera.X + CameraOffset.X;
                position.X %= 640f;
                if (position.X < 0f)
                {
                    position.X += 640f;
                }
                if (index < amount * limit)
                {
                    for (int draw_y = 0; draw_y < (scene as Level).Camera.Viewport.Height; draw_y += 360)
                    {
                        for (int draw_x = 0; draw_x < (scene as Level).Camera.Viewport.Width; draw_x += 640)
                        {
                            GFX.Game["particles/snow"].DrawCentered(position + new Vector2(draw_x, draw_y), color, scale, rotation);
                        }
                    }
                }
                index++;
            }
        }

        private List<Color> colors;

        private List<float> alphas;

        private int amount;

        private Vector2 addSpeed;

        private Vector2 windVector;

        private bool ignoreWind;

        public Vector2 CameraOffset;

        private Vector2[] positions;

        private int[] particleColors;

        private SineWave[] sines;

        private Vector2 scale;

        private float rotation;

        private float visibleFade;
    }
}
