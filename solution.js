const fs = require('fs');

// BigFraction handles arbitrary precision rational arithmetic using BigInt
class BigFraction {
    constructor(num, den = 1n) {
        if (den < 0n) {
            num = -num;
            den = -den;
        }
        const g = BigFraction.gcd(num < 0n ? -num : num, den);
        this.num = num / g;
        this.den = den / g;
    }

    static gcd(a, b) {
        while (b !== 0n) {
            let t = b;
            b = a % b;
            a = t;
        }
        return a;
    }

    add(other) {
        return new BigFraction(this.num * other.den + other.num * this.den, this.den * other.den);
    }

    multiply(other) {
        return new BigFraction(this.num * other.num, this.den * other.den);
    }
}

// Base parser supporting radix up to 36 with BigInt
function parseBigIntBase(str, base) {
    const chars = '0123456789abcdefghijklmnopqrstuvwxyz';
    str = str.trim().toLowerCase();
    const b = BigInt(base);
    let result = 0n;
    for (const char of str) {
        const val = BigInt(chars.indexOf(char));
        if (val < 0n || val >= b) continue;
        result = result * b + val;
    }
    return result;
}

// Lagrange interpolation evaluated at targetX
function evaluateLagrange(points, targetX) {
    const k = points.length;
    let total = new BigFraction(0n, 1n);

    for (let i = 0; i < k; i++) {
        let num = 1n;
        let den = 1n;
        for (let j = 0; j < k; j++) {
            if (i === j) continue;
            num *= (targetX - points[j].x);
            den *= (points[i].x - points[j].x);
        }
        const basis = new BigFraction(num, den);
        const term = new BigFraction(points[i].y, 1n).multiply(basis);
        total = total.add(term);
    }
    return total;
}

function getCombinations(arr, k) {
    const result = [];
    function backtrack(start, current) {
        if (current.length === k) {
            result.push([...current]);
            return;
        }
        for (let i = start; i < arr.length; i++) {
            current.push(arr[i]);
            backtrack(i + 1, current);
            current.pop();
        }
    }
    backtrack(0, []);
    return result;
}

function solveTestCase(filePath) {
    const raw = fs.readFileSync(filePath, 'utf-8');
    const data = JSON.parse(raw);

    const n = data.keys.n;
    const k = data.keys.k;

    const points = [];
    for (const key of Object.keys(data)) {
        if (key === 'keys') continue;
        const x = BigInt(key);
        const base = parseInt(data[key].base, 10);
        const y = parseBigIntBase(data[key].value, base);
        points.push({ x, y });
    }

    points.sort((a, b) => (a.x < b.x ? -1 : a.x > b.x ? 1 : 0));

    console.log("========================================");
    console.log(`File: ${filePath}`);
    console.log(`Total roots (n): ${n}, Required (k): ${k}`);

    const combinations = getCombinations(points, k);
    const secretCounts = new Map();
    const secretCombos = new Map();

    for (const combo of combinations) {
        const c = evaluateLagrange(combo, 0n);
        if (c.den === 1n) {
            const secretStr = c.num.toString();
            secretCounts.set(secretStr, (secretCounts.get(secretStr) || 0) + 1);
            if (!secretCombos.has(secretStr)) {
                secretCombos.set(secretStr, combo);
            }
        }
    }

    let bestSecret = '';
    let maxCount = -1;
    for (const [secret, count] of secretCounts.entries()) {
        if (count > maxCount) {
            maxCount = count;
            bestSecret = secret;
        }
    }

    console.log(`Secret (constant term c): ${bestSecret}`);

    if (bestSecret && secretCombos.has(bestSecret)) {
        const bestCombo = secretCombos.get(bestSecret);
        const wrongPoints = [];
        for (const pt of points) {
            const evalY = evaluateLagrange(bestCombo, pt.x);
            if (evalY.den !== 1n || evalY.num !== pt.y) {
                wrongPoints.push(pt.x.toString());
            }
        }
        if (wrongPoints.length > 0) {
            console.log(`Wrong / Outlier point(s) at x: ${wrongPoints.join(', ')}`);
        } else {
            console.log("All provided points lie on the polynomial.");
        }
    }
}

function main() {
    const files = process.argv.slice(2);
    if (files.length === 0) {
        solveTestCase('testcase1.json');
        solveTestCase('testcase2.json');
    } else {
        files.forEach(solveTestCase);
    }
}

main();
