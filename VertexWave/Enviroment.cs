using System;
using System.Threading;
using Microsoft.Xna.Framework;

namespace VertexWave
{
    public class Enviroment
    {
        public const byte MaxLight = 128;


        static Enviroment()
        {
            Light = 128;
            UpdateEnviroment();
            TimeOfDay = 18000;
        }

        public static byte Light { get; private set; }

        public static int TimeOfDay { get; private set; }

        public static double CelestialAngle { get; private set; }

        public static double Brigthness { get; private set; }

        public static Color SkyColor { get; private set; }

        public static int Minutes { get; private set; }

        public static int Hours { get; private set; }

        private static void UpdateEnviroment()
        {
            var counter = 0;
            new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(5);
                    //listener.UpdatedLight(Light, 0);
                    TimeOfDay++;

                    if (TimeOfDay > 24000) TimeOfDay = 0;

                    CalculateAngle();
                    CalculateBrigtness();
                    CalculateSkyColor();
                    CalculateTime();

                    if (counter > 2) counter = 0;

                    return;
                }
            }).Start();
        }

        private static void CalculateAngle()
        {
            var x = TimeOfDay / 24000.0;
            if (x <= 0) x += 1;
            CelestialAngle = x + (1.0 - (Math.Cos(x * Math.PI) + 1.0) / 2.0 - x) / 3.0;
        }

        private static void CalculateBrigtness()
        {
            Brigthness = Math.Cos(CelestialAngle * 2 * Math.PI - 4) * 1.5 + 0.8;
            if (Brigthness < 0)
                Brigthness = 0;
            else if (Brigthness > 1) Brigthness = 1;

            Light = (byte) (MaxLight * Brigthness);
        }

        private static void CalculateSkyColor()
        {
            float h = 214;
            var s = 0.5f;
            var v = (float) Brigthness;
            SkyColor = HsvtoRgb(h, s, v);

            //SkyColor = new Color((float)Brigthness,(float)Brigthness,(float)Brigthness);
        }

        private static void CalculateTime()
        {
            Hours = TimeOfDay / 1000;
            var min = TimeOfDay % 1000;
            Minutes = (int) (min / 1000f * 60f);
        }

        private static Color HsvtoRgb(float h, float s, float v)
        {
            float r, g, b;

            if (s == 0)
            {
                r = v;
                g = v;
                b = v;
            }
            else
            {
                int i;
                float f, p, q, t;

                if (h >= 360f)
                    h = 0f;
                else
                    h = h / 60f;

                i = (int) Math.Truncate(h);
                f = h - i;

                p = v * (1.0f - s);
                q = v * (1.0f - s * f);
                t = v * (1.0f - s * (1.0f - f));

                switch (i)
                {
                    case 0:
                        r = v;
                        g = t;
                        b = p;
                        break;

                    case 1:
                        r = q;
                        g = v;
                        b = p;
                        break;

                    case 2:
                        r = p;
                        g = v;
                        b = t;
                        break;

                    case 3:
                        r = p;
                        g = q;
                        b = v;
                        break;

                    case 4:
                        r = t;
                        g = p;
                        b = v;
                        break;

                    default:
                        r = v;
                        g = p;
                        b = q;
                        break;
                }
            }

            return new Color(r, g, b);
        }
    }
}