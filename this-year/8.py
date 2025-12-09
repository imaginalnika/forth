with open('input.txt') as f:
  boxes = [line.strip().split(',') for line in f]
  boxes = [(int(a), int(b), int(c)) for a, b, c in boxes]

import math
def dist(a, b):
  return math.sqrt(abs(a[0] - b[0])**2 + abs(a[1] - b[1])**2 + abs(a[2] - b[2])**2)

from itertools import combinations
junctions = list(combinations(range(len(boxes)), 2))
junctions = [(dist(boxes[n],boxes[m]), n, m) for n, m in junctions ]

junctions = sorted(junctions, key=lambda j: j[0])
junctions = [(n, m) for f, n, m in junctions]

# circuits = [set(j) for j in junctions[:1000]]

# while True:
#   updated = False
#   for i in range(len(circuits)):
#     for j in range(i):
#       circuit1 = circuits[i]
#       circuit2 = circuits[j]
#       if bool(circuit1 & circuit2):
#         circuits = [x for k, x in enumerate(circuits) if k != i and k != j]
#         circuits.append(circuit1 | circuit2)
#         updated = True
#         break
#     if updated:
#         break
#   if not updated:
#       break

# circuits = sorted(circuits, key=lambda c: len(c), reverse=True)
# partone = len(circuits[0]) * len(circuits[1]) * len(circuits[2])

circuits = []

junction = None

while True:
  junction = set(junctions[0])
  junctions = junctions[1:]
  circuit1_idx = None
  leftover = junction
  for i, circuit in enumerate(circuits):
    if circuit & junction:
      leftover = junction - circuit
      circuit1_idx = i
      break
  # print(junction)
  if len(leftover) == 2:
    circuits.append(junction)
  elif len(leftover) == 1:
    updated = False
    for i, circuit in enumerate(circuits):
      if circuit & leftover:
        new_circuits = [x for k, x in enumerate(circuits) if k != circuit1_idx and k != i]
        new_circuits.append(circuits[circuit1_idx] | circuits[i])
        circuits = new_circuits
        updated = True
    if not updated:
      circuits[circuit1_idx] = circuits[circuit1_idx] | junction
  else:
    circuits[circuit1_idx] = circuits[circuit1_idx] | junction

  print(len(circuits))
  if len(circuits) == 1 and len(circuits[0]) == len(boxes) :
    break

a, b = junction
parttwo = boxes[a][0] * boxes[b][0]
