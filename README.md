# Placements Assignment - Shamir's Secret Sharing

## Problem Overview
Given points of a polynomial $P(x)$ in JSON format where $y$-coordinates are encoded in various numerical bases, find the constant term $c$ (the secret, which is $P(0)$) of the polynomial $P(x) = a_m x^m + \dots + a_1 x + c$, where $k = m + 1$.

---

## Final Output / Answers

### **Test Case 1**
- **$n$ (total roots provided):** 4
- **$k$ (minimum roots required):** 3
- **Degree of polynomial ($m = k - 1$):** 2
- **Decoded Points:** $(1, 4), (2, 7), (3, 12), (6, 39)$
- **Polynomial:** $P(x) = x^2 + 3$
- **Secret (Constant Term $c = P(0)$):** **`3`**
- **Wrong / Outlier Points:** None (all points lie on the curve)

---

### **Test Case 2**
- **$n$ (total roots provided):** 10
- **$k$ (minimum roots required):** 7
- **Degree of polynomial ($m = k - 1$):** 6
- **Secret (Constant Term $c = P(0)$):** **`79836264049851`**
- **Valid Points on Curve:** $x \in \{1, 3, 4, 5, 6, 7, 9, 10\}$
- **Wrong / Outlier Points:** **$x = 2$** and **$x = 8$**

---

## How to Run

### Option 1: C# (.NET / Windows Native)
Compile and execute directly on Windows:
```bash
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /r:System.Numerics.dll /out:Solution.exe Solution.cs
.\Solution.exe testcase1.json testcase2.json
```

### Option 2: Node.js (JavaScript)
```bash
node solution.js testcase1.json testcase2.json
```

### Option 3: Java
```bash
javac Solution.java
java Solution testcase1.json testcase2.json
```
