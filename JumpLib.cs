using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JumpLib
{
    public class Jumping
    {
        public Jumping(TimeSpan speed, double distance, int height)
        {
            Speed = speed;
            Distance = distance;
            Height = height;

            var xii = Math.Sqrt(4 * (1 / distance) * height) / (2 * (1 / distance));
            xi = Math.Round(xii, 2);
        }

        public TimeSpan Speed { get; set; }
        public double Distance { get; set; }
        public int Height { get; set; }

        private readonly double xi;
        private double x;

        public IEnumerable<double> Jump()
        {
            x = -xi;

            while (true)
            {
                if (x > xi)
                    break;

                yield return -(1 / Distance) * x * x + Height;

                x += 0.01;

                Task.Delay(Speed);
            }
        }

        public void Crash()
            => x = -x;
    }
}
