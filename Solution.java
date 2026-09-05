import java.io.*;
import java.math.BigInteger;
import java.util.*;
import java.util.regex.*;

public class Solution {

    static class BigFraction {
        BigInteger num;
        BigInteger den;

        public BigFraction(BigInteger num, BigInteger den) {
            if (den.compareTo(BigInteger.ZERO) < 0) {
                num = num.negate();
                den = den.negate();
            }
            BigInteger gcd = num.abs().gcd(den);
            this.num = num.divide(gcd);
            this.den = den.divide(gcd);
        }

        public BigFraction add(BigFraction other) {
            BigInteger n = this.num.multiply(other.den).add(other.num.multiply(this.den));
            BigInteger d = this.den.multiply(other.den);
            return new BigFraction(n, d);
        }

        public BigFraction multiply(BigFraction other) {
            return new BigFraction(this.num.multiply(other.num), this.den.multiply(other.den));
        }
    }

    static class Point {
        BigInteger x;
        BigInteger y;

        public Point(BigInteger x, BigInteger y) {
            this.x = x;
            this.y = y;
        }
    }

    static BigFraction evaluateLagrange(List<Point> points, BigInteger targetX) {
        int k = points.size();
        BigFraction total = new BigFraction(BigInteger.ZERO, BigInteger.ONE);

        for (int i = 0; i < k; i++) {
            BigInteger num = BigInteger.ONE;
            BigInteger den = BigInteger.ONE;

            for (int j = 0; j < k; j++) {
                if (i == j) continue;
                num = num.multiply(targetX.subtract(points.get(j).x));
                den = den.multiply(points.get(i).x.subtract(points.get(j).x));
            }

            BigFraction basis = new BigFraction(num, den);
            BigFraction term = new BigFraction(points.get(i).y, BigInteger.ONE).multiply(basis);
            total = total.add(term);
        }

        return total;
    }

    static void getCombinations(List<Point> list, int k, int start, List<Point> current, List<List<Point>> result) {
        if (current.size() == k) {
            result.add(new ArrayList<>(current));
            return;
        }
        for (int i = start; i < list.size(); i++) {
            current.add(list.get(i));
            getCombinations(list, k, i + 1, current, result);
            current.remove(current.size() - 1);
        }
    }

    static void processTestCase(String filePath) {
        try {
            String json = new String(java.nio.file.Files.readAllBytes(java.nio.file.Paths.get(filePath)));

            int n = 0, k = 0;
            Matcher kMatcher = Pattern.compile("\"k\"\\s*:\\s*(\\d+)").matcher(json);
            Matcher nMatcher = Pattern.compile("\"n\"\\s*:\\s*(\\d+)").matcher(json);
            if (kMatcher.find()) k = Integer.parseInt(kMatcher.group(1));
            if (nMatcher.find()) n = Integer.parseInt(nMatcher.group(1));

            List<Point> points = new ArrayList<>();
            Matcher pointMatcher = Pattern.compile("\"(\\d+)\"\\s*:\\s*\\{([^}]+)\\}").matcher(json);

            while (pointMatcher.find()) {
                BigInteger x = new BigInteger(pointMatcher.group(1));
                String inner = pointMatcher.group(2);

                Matcher baseM = Pattern.compile("\"base\"\\s*:\\s*\"(\\d+)\"").matcher(inner);
                Matcher valM = Pattern.compile("\"value\"\\s*:\\s*\"([a-zA-Z0-9]+)\"").matcher(inner);

                if (baseM.find() && valM.find()) {
                    int radix = Integer.parseInt(baseM.group(1));
                    String valStr = valM.group(1);
                    BigInteger y = new BigInteger(valStr, radix);
                    points.add(new Point(x, y));
                }
            }

            points.sort(Comparator.comparing(p -> p.x));

            System.out.println("========================================");
            System.out.println("File: " + filePath);
            System.out.println("Total points (n): " + n + ", Required points (k): " + k);

            List<List<Point>> combos = new ArrayList<>();
            getCombinations(points, k, 0, new ArrayList<>(), combos);

            Map<String, Integer> secretCounts = new HashMap<>();
            Map<String, List<Point>> secretCombos = new HashMap<>();

            for (List<Point> combo : combos) {
                try {
                    BigFraction c = evaluateLagrange(combo, BigInteger.ZERO);
                    if (c.den.equals(BigInteger.ONE)) {
                        String s = c.num.toString();
                        secretCounts.put(s, secretCounts.getOrDefault(s, 0) + 1);
                        if (!secretCombos.containsKey(s)) {
                            secretCombos.put(s, combo);
                        }
                    }
                } catch (Exception ignored) {}
            }

            String bestSecret = "";
            int maxCount = -1;
            for (Map.Entry<String, Integer> entry : secretCounts.entrySet()) {
                if (entry.getValue() > maxCount) {
                    maxCount = entry.getValue();
                    bestSecret = entry.getKey();
                }
            }

            System.out.println("Secret (constant term c): " + bestSecret);

            if (!bestSecret.isEmpty() && secretCombos.containsKey(bestSecret)) {
                List<Point> bestCombo = secretCombos.get(bestSecret);
                List<String> wrongPoints = new ArrayList<>();
                for (Point pt : points) {
                    BigFraction evalY = evaluateLagrange(bestCombo, pt.x);
                    if (!evalY.den.equals(BigInteger.ONE) || !evalY.num.equals(pt.y)) {
                        wrongPoints.add(pt.x.toString());
                    }
                }
                if (!wrongPoints.isEmpty()) {
                    System.out.println("Wrong / Outlier point(s) at x: " + String.join(", ", wrongPoints));
                } else {
                    System.out.println("All provided points lie on the polynomial.");
                }
            }

        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public static void main(String[] args) {
        String path1 = args.length > 0 ? args[0] : "testcase1.json";
        String path2 = args.length > 1 ? args[1] : "testcase2.json";

        if (new File(path1).exists()) processTestCase(path1);
        if (new File(path2).exists()) processTestCase(path2);
    }
}
