# Problem A: Error Correction
## Introduction
Captain Jean-Pierre Spock received an extremely important transmission via a sub-ether quantum hyper- link. 
Unfortunately, the sub-ether quantum hyperlink communication software is still under beta testing and some
of the characters in the message were received incorrectly. Your task is to decode the garbled message. We 
know the language of the message, and we have a dictionary of all the possible words that could appear in the 
original text. You have to decode the message by examining each word in the text, and determining which word 
it could have been originally.

## Input
Each test case consists of two input files: the garbled message and the dictionary (the text file containing
the message has extension ’.in’, while the extension of the dictionary file is ’.dict’). In the message some 
of the characters are replaced by the character ’*’, meaning that the character was received in- correctly 
(the character ’*’ does not appear in the original message). For some weird reason, the errors appeared only 
when the letters ’a’-’z’, ’A’-’Z’ were transmitted, the other characters were always received correctly.

The dictionary contains all the words that appear in the original message (it can contain words that do not 
appear in the message). A word is defined as a sequence of characters ’a’-’z’ or ’A’-’Z’ such that the character 
before and after the word is not a letter. Only the lower case version of the words appear in the dictionary 
(using only the letters ’a’-’z’), but in the message any (or all) character of word can be in upper case.
For example, if the word ’holodeck’ is present in the dictionary, then the message may contain ’holodeck’, 
’Holodeck’, ’HoloDeck’, or ’HOLODECK’. The words in the dictionary are separated by a new line character 10. 
(The last word is also followed by a new line character.)

## Output
For each test case, you have to output the original version of the message. For each word in the input that 
contains the character ’*’, you have to guess what the word in the original message was, and replace the missing
characters with the correct letters. Always use lower case letters when replacing a ’*’ (but leave the other 
characters of the word as they appear in the message). It can be assumed that the problem is unambiguous: 
for each word in the message, there is only one correct possibility in the dictionary. Other than replacing
the characters ’*’ with the correct letters, do not modify the text in any way. In particular, the end of line 
characters should appear in the output exactly as in the input.

## Sample Input
Message:
```
one,O*E,f**r (th*ee t*o) *ix
```
Dictionary:
```
one
two
three
four
five
six
```
## Sample Output
Original message:
```
one,OnE,four (three two) six
```
