# Placements Assignment - Shamir's Secret Sharing Solver

[![Live Demo](https://img.shields.io/badge/Live%20Demo-GitHub%20Pages-indigo)](https://koppuravurisrinivas-crypto.github.io/clg-placement-Drive/)
[![Test Status](https://img.shields.io/badge/CI-Automated%20Tests-success)](https://github.com/KoppuravuriSrinivas-crypto/clg-placement-Drive/actions)

## 🌐 Live Web Application
Try the live interactive polynomial root solver here:
👉 **[https://koppuravurisrinivas-crypto.github.io/clg-placement-Drive/](https://koppuravurisrinivas-crypto.github.io/clg-placement-Drive/)**

---

## 📌 Problem Overview
Given roots (points) of a polynomial $P(x)$ in JSON format where the $y$-coordinates are encoded in various numerical bases:
1. Decode each $y$-coordinate from its specified base (e.g. base 2, 3, 4, 6, 8, 10, 12, 15, 16) into standard decimal arbitrary-precision integers.
2. Degree of the polynomial is $m = k - 1$, where $k$ is the minimum number of roots required.
3. Compute the constant term $c$ (the secret, which is $P(0)$) using exact Lagrange interpolation.
4. Detect and filter out any outlier/imposter points that do not satisfy the consensus polynomial.

---

## 🎯 Final Verified Results

### **Test Case 1**
*   **$n$ (Total roots provided):** 4
*   **$k$ (Minimum roots required):** 3
*   **Degree of polynomial ($m = k - 1$):** 2
*   **Decoded Points $(x, y)$:** $(1, 4), (2, 7), (3, 12), (6, 39)$
*   **Polynomial:** $P(x) = x^2 + 3$
*   **Secret (Constant Term $c = P(0)$):** **`3`**
*   **Wrong / Outlier Points:** None (all points lie on the curve)

---

### **Test Case 2**
*   **$n$ (Total roots provided):** 10
*   **$k$ (Minimum roots required):** 7
*   **Degree of polynomial ($m = k - 1$):** 6
*   **Valid Points on Curve:** $x \in \{1, 3, 4, 5, 6, 7, 9, 10\}$
*   **Wrong / Outlier Points:** **$x = 2$** and **$x = 8$**
*   **Consensus Polynomial:**
    $$P(x) = 205802168748539 x^6 + 129715447661077 x^5 + 105860038268942 x^4 + 147160079768248 x^3 + 234176747398429 x^2 + 92534348706405 x + 79836264049851$$
*   **Secret (Constant Term $c = P(0)$):** **`79836264049851`**

---

## 💻 Console Execution Output
```text
========================================
File: testcase1.json
Total points (n): 4, Required points (k): 3
Parsed points count: 4
Secret (constant term c): 3
All provided points lie on the polynomial.
========================================
File: testcase2.json
Total points (n): 10, Required points (k): 7
Parsed points count: 10
Secret (constant term c): 79836264049851
Wrong / Outlier point(s) at x: 2, 8
```

---

## 🚀 How to Run Locally

### Node.js (JavaScript)
```bash
node solution.js testcase1.json testcase2.json
```

### Java
```bash
javac Solution.java
java Solution testcase1.json testcase2.json
```

### C# (.NET)
```bash
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /r:System.Numerics.dll /out:Solution.exe Solution.cs
.\Solution.exe testcase1.json testcase2.json
```
