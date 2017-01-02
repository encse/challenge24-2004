# Problem F: Movie
## Introduction
Director Stephen Bergspiel would like to shoot a low-budget movie. The film has only two main characters, 
so only two actors have to be hired. Bergspiel wrote a list of all the actors who would be able to play 
the roles in the film. He wants to select the two cheapest actors from this list, but to add interest to 
the film, he wants to select two actors who have never ever appeared together in any film. You have to write 
a program that finds the two cheapest actors satisfying these requirements.

## Input
Each input file begins with a line containing two integers: *n (1 ≤ n ≤ 100)*, the number of suitable actors, 
and *m (1 ≤ m ≤ 10000)*, the number of films where these actors appeared. The next *n* lines describe the cost 
of the *n* actors, there is a single integer in each line. The next *m* lines after that describe the *m* films. 
Each line begins with an integer *k (1 ≤ k ≤ 20)*, the number of actors in the film. This number is followed 
by *k* numbers (separated by spaces). These numbers identify the actors playing in the film, each number is
between *1* and *n*.

## Output
You have to pick two actors such that the sum of their cost is minimal, and they never appeared in the same 
film. You have to output a number, the cost of the best solution. If there is no solution, then output 
`No solution.`. The output should be terminated by a new line character 10.

## Sample Input
```
4 4
10000
20000
30000
40000
2 1 3
1 2
3 1 2 4
2 2 3
```
## Sample Output
```
70000
```
