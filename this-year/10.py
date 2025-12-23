import re
import sys
from pulp import LpMinimize, LpProblem, LpVariable, PULP_CBC_CMD, lpSum

def solve_min_l1(M, b):
    n, m = len(M), len(M[0])

    prob = LpProblem("MinL1", LpMinimize)
    x = [LpVariable(f"x{i}", lowBound=0, cat="Integer") for i in range(m)]

    prob += lpSum(x)

    for i in range(n):
        prob += lpSum(M[i][j] * x[j] for j in range(m)) == b[i]

    prob.solve(PULP_CBC_CMD(msg=0))

    if prob.status == 1:
        return [int(v.varValue) for v in x]
    return None


def parse_line(line):
    bracket = re.search(r"\[(.*?)\]", line)
    if not bracket:
        raise ValueError(f"Missing [..] section: {line}")
    n = len(bracket.group(1))

    parens = re.findall(r"\((.*?)\)", line)
    target_match = re.search(r"\{(.*?)\}", line)
    if not target_match:
        raise ValueError(f"Missing {{..}} section: {line}")
    target = [int(x) for x in target_match.group(1).split(",") if x != ""]
    if len(target) != n:
        raise ValueError(f"Target length {len(target)} != digits {n}: {line}")

    cols = []
    for p in parens:
        if p.strip() == "":
            idx = []
        else:
            idx = [int(x) for x in p.split(",") if x != ""]
        col = [0] * n
        for i in idx:
            col[i] = 1
        cols.append(col)

    if not cols:
        raise ValueError(f"No button columns found: {line}")

    M = [[cols[j][i] for j in range(len(cols))] for i in range(n)]
    return M, target


def solve_file(path):
    total = 0
    with open(path, "r", encoding="ascii") as f:
        for raw in f:
            line = raw.strip()
            if not line:
                continue
            M, b = parse_line(line)
            x = solve_min_l1(M, b)
            if x is None:
                raise ValueError(f"Infeasible line: {line}")
            total += sum(x)
    return total


def main():
    path = sys.argv[1] if len(sys.argv) > 1 else "input-10.txt"
    total = solve_file(path)
    print(total)


if __name__ == "__main__":
    main()
