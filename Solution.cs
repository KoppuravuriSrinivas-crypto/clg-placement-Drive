using System;
using System.IO;
using System.Numerics;
using System.Collections.Generic;
using System.Text.RegularExpressions;

class Solution
{
    public struct BigFraction
    {
        public BigInteger Num;
        public BigInteger Den;

        public BigFraction(BigInteger num, BigInteger den)
        {
            if (den < 0)
            {
                num = -num;
                den = -den;
            }
            BigInteger gcd = BigInteger.GreatestCommonDivisor(BigInteger.Abs(num), den);
            Num = num / gcd;
            Den = den / gcd;
        }

        public static BigFraction Zero = new BigFraction(0, 1);

        public static BigFraction operator +(BigFraction a, BigFraction b)
        {
            return new BigFraction(a.Num * b.Den + b.Num * a.Den, a.Den * b.Den);
        }

        public static BigFraction operator *(BigFraction a, BigFraction b)
        {
            return new BigFraction(a.Num * b.Num, a.Den * b.Den);
        }

        public override string ToString()
        {
            if (Den == 1) return Num.ToString();
            return Num.ToString() + "/" + Den.ToString();
        }
    }

    public class Point
    {
        public BigInteger X;
        public BigInteger Y;
        public Point(BigInteger x, BigInteger y)
        {
            X = x;
            Y = y;
        }
    }

    static BigInteger ParseBase(string val, int radix)
    {
        val = val.Trim().ToLowerInvariant();
        BigInteger result = 0;
        foreach (char c in val)
        {
            int digit;
            if (c >= '0' && c <= '9')
                digit = c - '0';
            else if (c >= 'a' && c <= 'z')
                digit = c - 'a' + 10;
            else
                continue;

            result = result * radix + digit;
        }
        return result;
    }

    // Lagrange Interpolation evaluated at x = targetX
    static BigFraction EvaluateLagrange(List<Point> points, BigInteger targetX)
    {
        int k = points.Count;
        BigFraction total = BigFraction.Zero;

        for (int i = 0; i < k; i++)
        {
            BigInteger num = 1;
            BigInteger den = 1;

            for (int j = 0; j < k; j++)
            {
                if (i == j) continue;
                num *= (targetX - points[j].X);
                den *= (points[i].X - points[j].X);
            }

            BigFraction basis = new BigFraction(num, den);
            BigFraction term = new BigFraction(points[i].Y, 1) * basis;
            total = total + term;
        }

        return total;
    }

    static void GetCombinations(List<Point> list, int k, int start, List<Point> current, List<List<Point>> result)
    {
        if (current.Count == k)
        {
            result.Add(new List<Point>(current));
            return;
        }
        for (int i = start; i < list.Count; i++)
        {
            current.Add(list[i]);
            GetCombinations(list, k, i + 1, current, result);
            current.RemoveAt(current.Count - 1);
        }
    }

    static void ProcessTestCase(string jsonPath)
    {
        string json = File.ReadAllText(jsonPath);

        // Parse n and k from "keys": { "n": ..., "k": ... }
        int n = 0, k = 0;
        Match keysMatch = Regex.Match(json, @"\""keys\""\s*:\s*\{[^}]*\""n\""\s*:\s*(\d+)[^}]*\""k\""\s*:\s*(\d+)", RegexOptions.Singleline);
        if (keysMatch.Success)
        {
            n = int.Parse(keysMatch.Groups[1].Value);
            k = int.Parse(keysMatch.Groups[2].Value);
        }
        else
        {
            // Alternative order: k before n
            Match kMatch = Regex.Match(json, @"\""k\""\s*:\s*(\d+)");
            Match nMatch = Regex.Match(json, @"\""n\""\s*:\s*(\d+)");
            if (kMatch.Success) k = int.Parse(kMatch.Groups[1].Value);
            if (nMatch.Success) n = int.Parse(nMatch.Groups[1].Value);
        }

        // Parse points: "1": { "base": "...", "value": "..." }
        // We match all keys that are integer strings
        var points = new List<Point>();
        MatchCollection pointMatches = Regex.Matches(json, @"\""(\d+)\""\s*:\s*\{([^}]+)\}");
        foreach (Match m in pointMatches)
        {
            BigInteger x = BigInteger.Parse(m.Groups[1].Value);
            string inner = m.Groups[2].Value;

            Match baseM = Regex.Match(inner, @"\""base\""\s*:\s*\""(\d+)\""");
            Match valM = Regex.Match(inner, @"\""value\""\s*:\s*\""([a-zA-Z0-9]+)\""");

            if (baseM.Success && valM.Success)
            {
                int radix = int.Parse(baseM.Groups[1].Value);
                string valStr = valM.Groups[1].Value;
                BigInteger y = ParseBase(valStr, radix);
                points.Add(new Point(x, y));
            }
        }

        // Sort points by X
        points.Sort((a, b) => a.X.CompareTo(b.X));

        Console.WriteLine("========================================");
        Console.WriteLine("File: " + Path.GetFileName(jsonPath));
        Console.WriteLine("Total points (n): " + n + ", Required points (k): " + k);
        Console.WriteLine("Parsed points count: " + points.Count);

        // Find combination of k points that forms a consensus polynomial
        var combos = new List<List<Point>>();
        GetCombinations(points, k, 0, new List<Point>(), combos);

        var secretCounts = new Dictionary<string, int>();
        var secretCombos = new Dictionary<string, List<Point>>();

        foreach (var combo in combos)
        {
            try
            {
                BigFraction c = EvaluateLagrange(combo, 0);
                if (c.Den == 1) // Valid polynomial with integer constant term
                {
                    string key = c.Num.ToString();
                    if (!secretCounts.ContainsKey(key))
                    {
                        secretCounts[key] = 0;
                        secretCombos[key] = combo;
                    }
                    secretCounts[key]++;
                }
            }
            catch { }
        }

        string bestSecret = "";
        int maxCount = -1;
        foreach (var kv in secretCounts)
        {
            if (kv.Value > maxCount)
            {
                maxCount = kv.Value;
                bestSecret = kv.Key;
            }
        }

        Console.WriteLine("Secret (constant term c): " + bestSecret);

        // Identify wrong points if any
        if (!string.IsNullOrEmpty(bestSecret) && secretCombos.ContainsKey(bestSecret))
        {
            var bestCombo = secretCombos[bestSecret];
            var wrongPoints = new List<BigInteger>();
            foreach (var pt in points)
            {
                BigFraction evalY = EvaluateLagrange(bestCombo, pt.X);
                if (evalY.Den != 1 || evalY.Num != pt.Y)
                {
                    wrongPoints.Add(pt.X);
                }
            }

            if (wrongPoints.Count > 0)
            {
                Console.WriteLine("Wrong / Outlier point(s) at x: " + string.Join(", ", wrongPoints.ConvertAll(p => p.ToString()).ToArray()));
            }
            else
            {
                Console.WriteLine("All provided points lie on the polynomial.");
            }
        }
    }

    static void Main(string[] args)
    {
        string path1 = args.Length > 0 ? args[0] : "testcase1.json";
        string path2 = args.Length > 1 ? args[1] : "testcase2.json";

        if (File.Exists(path1)) ProcessTestCase(path1);
        if (File.Exists(path2)) ProcessTestCase(path2);
    }
}
