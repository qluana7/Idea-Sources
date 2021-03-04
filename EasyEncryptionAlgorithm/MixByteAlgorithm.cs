using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EasyEncryptionAlgorithm
{
    public class MixByteAlgorithm
    {
        // 랜덤 패스워드 생성
        public MixByteAlgorithm()
        {
            Random rnd = new Random();
            byte[] b = new byte[1];
            rnd.NextBytes(b);
            Password = b[0];
        }

        // 지정 패스워드 생성
        public MixByteAlgorithm(byte spin)
            => Password = spin;

        // 패스워드
        public byte Password { get; }

        // 배열 분리 메소드
        private void Split<T>(T[] array, int index, out T[] first, out T[] second)
        {
            // 앞 배열 분해
            first = array.Take(index).ToArray();

            // 뒷 배열 분해
            second = array.Skip(index).ToArray();
        }

        // 문자열(2진수) => 바이트
        private byte ToByte(char[] str)
        {
            // 출력 값 선언
            int b = 0;

            // 비트를 읽고 변환
            for (int i = 0; i < str.Length - 1; i++)
            {
                // b에 값을 더하고
                b += int.Parse(str[i].ToString());

                // 비트 이동으로 옆으로 밀어준다
                b <<= 1;
            }
            
            // 마지막은 밀면 안되므로 따로 값을 더해준다
            b += int.Parse(str[7].ToString());
            
            // 바이트로써 반환
            return (byte)b;
        }

        // 암호화
        public Stream Encrypt(ref Stream stream)
        {// ref 스트림 포지션 기억
            var pos = stream.Position;

            // 스트림을 읽기 위해 처음으로 이동
            stream.Seek(0, SeekOrigin.Begin);

            // 내보낼 스트림 생성
            var s = new MemoryStream();

            while (true)
            {
                // 스트림에서 값을 읽어옴
                var b = stream.ReadByte();

                // 읽어온 값이 EOF(-1)에 도달하면 종료
                if (b == -1)
                    break;

                // 바이트를 문자열로 변환 = 비트를 이용하기 위함
                var bs = Convert.ToString(b, 2).PadLeft(8, '0').ToArray();

                // 반복
                for (int i = 0; i < bs.Length; i++)
                {
                    // 비트가 1인 경우 분해를 진행
                    if (bs[i] == '1')
                    {
                        // 비트가 1인 위치를 기준으로 분해
                        Split(bs, i, out char[] sf, out char[] ss);

                        // 분해된 비트를 반대로 결합
                        bs = ss.Concat(sf).ToArray();
                    }
                }

                // 문자열을 바이트로 변환
                var e = ToByte(bs);

                // 출력 스트림에 암호화 된 데이터 저장
                s.WriteByte(e);
            }

            // 스트림을 원래 위치로 복귀
            stream.Position = pos;

            // 스트림 반환
            return s;
        }

        // 암호화 과정을 번복하면 복호화가 되기에
        // 해당 메서드는 암호화 메서드와 병합해도 됨
        // 자세한 내용은 아래에 원리 주석 참고
        public Stream Decrypt(ref Stream stream)
            => Encrypt(ref stream);
    }
}

/* 암호화 원리
 * 
 * 비트에서 1인 비트에 위치를 기준으로 배열을 잘라
 * 반대로 붙이는 작업을 진행한다 예를 들어
 * 패스워드가 1101 이고 데이터가 1001 인 경우
 * 첫번째가 1이므로 1/001 로 데이터를 나누고
 * 001/1 이렇게 데이터를 다시 병합한다
 * 이를 반복한다. 00/11 => 11/00
 * 1100/ => /1100. 최종적 = 1001 => 1100
 * 이를 한번 더 반복하게 되면 다시 원래대로 돌아온다
 */