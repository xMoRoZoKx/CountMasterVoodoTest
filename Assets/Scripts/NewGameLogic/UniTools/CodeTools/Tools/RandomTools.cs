using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
namespace UniTools
{

    public static class RandomTools
    {
        public static void InvokWithChance(Action action, int chance)
        {
            if (chance == 0) return;
            if (GetChance(chance)) action?.Invoke();
        }
        public static bool GetChance(float chance) => UnityEngine.Random.Range(0, 100) <= chance;
        public static bool GetDeterministicChance(float chance, string key) => RangeDeterministic(key, 0, 100) <= chance;
        public static Vector3 GetRandomPointInRange(Vector3 centre, float range, bool useX = true, bool useY = true, bool useZ = true)
        {
            return new Vector3(centre.x + (useX ? GetRandom() : 0), centre.y + (useY ? GetRandom() : 0), centre.z + (useZ ? GetRandom() : 0));

            float GetRandom() => UnityEngine.Random.Range(-range, range);
        }
        public static float Range01Deterministic(string key)
        {
            var rnd = new System.Random(GetDeterministicHashCode(key));
            return (float)rnd.NextDouble();
        }
        public static float RangeDeterministic(string key, float min, float max)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] keyBytes = Encoding.UTF8.GetBytes(key);
                byte[] hash = sha.ComputeHash(keyBytes);

                uint hashInt = BitConverter.ToUInt32(hash, 0);

                float t = hashInt / (float)uint.MaxValue;

                return min + t * (max - min);
            }
        }
        private static int GetDeterministicHashCode(string str)
        {
            using (var md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
                return BitConverter.ToInt32(hash, 0);
            }
        }
    }
}