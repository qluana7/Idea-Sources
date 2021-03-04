using System;
using System.IO;

namespace EasyEncryptionAlgorithm
{
    // Xor (베타적 논리합)을 이용한 간단한 암호화
    public class XorEncryption
    {
        // 랜덤 패스워드 생성
        public XorEncryption()
        {
            Random rnd = new Random();
            byte[] b = new byte[1];
            rnd.NextBytes(b);
            Password = b[0];
        }

        // 지정 패스워드 생성
        public XorEncryption(byte pass)
            => Password = pass;

        // 패스워드
        public byte Password { get; }

        // 암호화
        public Stream Encrypt(ref Stream stream)
        {
            // ref 스트림 포지션 기억
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

                // 패스월드와 데이터를 베타적 논리합 수행
                var e = b ^ Password;

                // 출력 스트림에 암호화 된 데이터 저장
                s.WriteByte((byte)e);
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
 * 베타적 논리 합은 같은 두번 수행하면 원래 값으로 돌아오게 되는데
 * 예를 들어 1001 과 0101 을 베타적 논리합을 수행하게 되면
 * 1100 이 나오게 되는데 이 1100과 0101 을 다시 한번 베타적
 * 논리합을 수행하게 되면 1001 로 데이터가 복구된다
 * 이를 이용해서 쉽게 암호화 복호화가 가능
 */
