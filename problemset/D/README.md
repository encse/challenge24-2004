# Problem D: Wizards
## Introduction
30th February is the Annual Magician’s Day, since it is the birthday of the Great Archmage Luxiputrius. 
The National Association of Wizards and Sorcerers has decided to organize several shows on this day 
throughout the country. There are several cities where these events can be held. However, in a given 
city only one wizard can perform: it would be too dangerous to allow two wizards to cast spells in the 
same city at the same time. The wizards live in ivory towers, usually far from the cities. They do not 
like travelling: a wizard refuses to go a distance of more than 50 km from their home (most probably 
because the popular Teleport Amulets only have a range of 50 km). The National Association of Wizards 
and Sorcerers would like to organize as many shows as possible on the Annual Magician’s Day. Your job 
is to determine how many shows can be held.

## Input
Each input file begins with two numbers *n (1 ≤ n ≤ 1000)*, the number of cities, and *m (1 ≤ m ≤ 1000)*,
the number of wizards. This is followed by *n + m* lines, each containing two coordinates (in km). The first
*n* of these lines describe the position of the cities, the next m lines describe the position of the towers 
of the wizards. The coordinates are not necessarily integer.

## Output
Output is a single number, the maximum number of shows that can be held on one night. At most one show 
can be organized in every city, and a wizard can perform no more than one show a day. The number in the 
output should be terminated by a new line character 10.

## Sample Input
```
4 4
10 0
70 0
150 0
220 0 
60 0 
110 0 
190 0 
0 0
```
## Sample Output
```
4
```
