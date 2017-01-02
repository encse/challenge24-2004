# Problem E: Numbers
## Introduction
Two players A and B play the following game. They start with eight integers *x<sub>1</sub>, x<sub>2</sub>, ..., x<sub>8</sub>*, 
and they have eight magic numbers *c<sub>1</sub>, c<sub>2</sub>, ..., c<sub>8</sub>*. The following formula is used to 
calculate the numbers *x<sub>i</sub>* for *i > 8*:

*x<sub>i</sub> =x<sub>i−1</sub> ·c<sub>1</sub> +x<sub>i-2</sub> ·c<sub>2</sub> +x<sub>i-3</sub> ·c<sub>3</sub> +x<sub>i-4</sub> ·c<sub>4</sub> + x<sub>i-4</sub> ·c<sub>4</sub> +x<sub>i-5</sub> ·c<sub>5</sub> +x<sub>i-6</sub> ·c<sub>6</sub> +x<sub>i-7</sub> ·c<sub>7</sub> + x<sub>i-8</sub> ·c<sub>8</sub>*

All the calculations are done modulo 1000, so *x<sub>i</sub>* is always a number between 0 and 999. Each player has a goal, 
which is a sequence of eight numbers. If these numbers appear in the sequence *x<sub>i</sub>, then the player wins. 
For example, if the goal of Player A is 500 12 0 8 67 289 901 415 and

*x<sub>89</sub> =11, x<sub>90</sub> =12, x<sub>91</sub> =500, x<sub>92</sub> =12, x<sub>93</sub> =0, x<sub>94</sub> =8, x<sub>95</sub> =67, x<sub>96</sub> =289, x<sub>97</sub> =901, x<sub>98</sub> =415, ...*

then Player A wins at *x<sub>98</sub>*. To win the game the numbers have to appear exactly in the given order, and
there cannot be additional numbers between the eight numbers in the sequence.
Because life is short, and calculating this sequence is a very boring thing to do, your have to write 
a program that determines who will be the winner.
## Input
Each input file consists of four lines, with eight non-negative integers in each line. The first line 
contains the eight numbers *x<sub>1</sub>, x<sub>2</sub>, ..., x<sub>8</sub>*. The second line contains the magic numbers *c<sub>1</sub>, c<sub>2</sub>, ..., c<sub>8</sub>*. 
The third line contains the goal of A, while the fourth line contains the goal of B.
## Output
You have to determine who will win the game and when. If player A wins at *x<sub>98</sub>* (that is, his sequence appears on *x<sub>91</sub>*, *x<sub>92</sub>*, *x<sub>93</sub>*, *x<sub>94</sub>*, *x<sub>95</sub>*, *x<sub>96</sub>*, *x<sub>97</sub>*, *x<sub>98</sub>*) then the output should be the line

```A wins at 98.```

The line should be terminated by a new line character 10. It can be assumed that the game will be finished in at most 10000000 steps.
## Sample Input
```
1 2 3 4 5 6 7 8
2 0 1 500 101 222 333 444
1 1 1 1 1 1 1 1
438 393 722 198 794 331 878 976
```
## Sample Output
```
B wins at 2829.
```
