# Problem B: Knights of the Round Table
## Introduction
King Arthur has summoned the valiant knights of his kingdom to Camelot. In the Castle of Camelot, 
the knights sit around a large round table and discuss how to defend the country against its enemies.
However, some of the knights do not trust each other, so the order of the knights around the table has
to be determined in such a way that ensures no fighting will start during the council. Moreover, some
of the knights (for example, the brave Sir Robin) are so afraid of the other knights that they will only
sit at the table if a trusted ally sits beside them. Your task is to list all the possible seating 
arrangements of the knights that satisfy the requirements.

## Input
Each input file begins with two numbers: n (2 ≤ n ≤ 26), the number of knights, and m (1 ≤ m ≤ 100), 
the number of requirements that has to be satisfied. The knights are codenamed ’a’, ’b’, . . . , ’z’.
The first line of the input is followed by m lines, one line for each requirement. The requirements can 
be of two types. The requirement
```
b hates a and z and t
```
means that knight ’b’ cannot sit beside either ’a’, ’z’, or ’t’, otherwise they will kill each other. The
line
```
r needs q or t or m
```
means that kinght ’r’ sits to the table only if at least one of ’q’, ’t’, or ’m’ sits besides him.

## Output
For each input file, you have to output all the possible seating arrangements for the knights. Each line 
of the output should be n characters long (terminated by a new line character 10). These n characters 
describe the positions of the knights around the table. The first character is the name of the knight 
sitting at Seat 1, the second character is the name of the knight sitting at Seat 2, etc. Note that 
seating assignments that differ only by a rotation or by reversing the order count as different 
solutions: abdce, bdcea and ecdba all appear in the output below. The lines in the output should 
be sorted alphabetically. It can be assumed that there is always at least one possible solution 
to the problem. Do not forget that the first knight is the neighbour of the last knight!

### Sample Input
```
5 2
a hates c 
b needs d
```
### Sample Output
```
abdce
adbce
aecbd
aecdb
baecd
bcead
bdaec
bdcea
cbdae
cdbae
ceabd
ceadb
daecb
dbaec
dbcea
dceab
eabdc
eadbc
ecbda
ecdba
```
