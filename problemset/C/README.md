# Problem C: TV programming
## Introduction
BanduTV is the most popular TV channel in Bandulu. The channel has a fixed schedule: every day the same 
types of programs are shown during the same time periods. For example, it is possible that every day there 
is news from 18:00 to 19:00, a soap opera from 19:00 to 20:00, a movie from 20:00 to 22:00, music from 22:00 
to 02:00, etc. Every year at the start of the season, the program director of the channel decides the schedule 
of the programs. The different departments of the channel propose several possible programs. Each proposal 
contains a time interval for the program, and the profit expected from it. The director chooses the programs 
in such a way that maximizes the profit. For example, if the following programs are proposed:
```
1. 16:00–20:00 Sport              1200
2. 18:00–19:00 News               2550
3. 19:00–20:00 Series             1800
4. 20:00–22:00 Movie              2000
5. 22:00–08:00 Music              800
6. 22:00–01:00 Politics           1300
7. 07:00–16:00 Children’s Program 4000 
```
     
then the best thing to do is to is to select 2, 3, 4, 6 and 7 for a total profit of 11650. Notice that a 
program can extend past midnight, and it is possible that no program is scheduled for certain time intervals. 
Of course, we cannot select two programs that overlap.

## Input
Each input file begins with a number *n (1 ≤ n ≤ 1000)*, the number of program proposals. The first line of the 
input is followed by *n* lines, one line for each proposal. Each line contains the start and end time of the 
program, and the expected profit. The times are given in 24-hour 4-digit form such as `’08:35’`, and are separated by spaces.

## Output
Output is a single number, the maximum profit that can be achieved. The number in the output should be terminated 
by a new line character 10.

## Sample Input
```
7
16:00 20:00 1200
18:00 19:00 2550
19:00 20:00 1800
20:00 22:00 2000
22:00 08:00 800
22:00 01:00 1300
07:00 16:00 4000
```
## Sample Output
```
11650
```
