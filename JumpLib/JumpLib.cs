using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JumpLib
{
    public class Jumping
    {
        public Jumping(TimeSpan speed, double distance, int height)
        {
            Speed = speed;       // 쿼리 타임
            Distance = distance; // 1/a
            Height = height;     // c
            
            // 근의 공식 사용 : b = 0 이므로, 루트(-4 x a x c) / 2 x a
            var xii = Math.Sqrt(4 * (1 / distance) * height) / (2 * (1 / distance));
            // 반올림 하여 소숫점 2째 자리까지 제한
            xi = Math.Round(xii, 2);
        }

        public TimeSpan Speed { get; set; }
        public double Distance { get; set; }
        public int Height { get; set; }

        // 한계값 설정
        private readonly double xi;
        // 가상의 X값 선언
        private double x;

        // LET'S JUMP!
        public IEnumerable<double> Jump()
        {
            // X 방정식에 0이 되는 낮은 값으로 설정
            x = -xi;

            while (true)
            {
                // X가 0에 도달, 즉 해에 도달하는 경우 멈춤
                if (x > xi)
                    break;

                // X가 해에 도달할 때까지 방정식을 이용해 Y값을 도출
                // - (1 / a) × x^2 + c => 2차 함수 사용
                yield return -(1 / Distance) * x * x + Height;

                // X값 증가
                x += 0.01;

                // 쿼리 딜레이
                Task.Delay(Speed);
            }
        }

        // 위에 블록과 충돌시 좌표 반전 => 그 자리에서 바로 내려감
        public void Crash()
            => x = -x;
    }
}
