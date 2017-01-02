# Challenge24 (2003) EC 

This project contains the solutions to the first electronic round of Challenge24, a yearly programming contest held in Budapest.

## Setup
Check the `problemset` folder for problem descriptions and inputs. The `ch24` folder contains our solutions written in C#. The repo is self contained, the binary dependencies are also included under `packages`.

### For Visual Studio 2015 and Visual Studio for Mac users
Open the solution file `Contest.sln`. Under the project options for the `ch24` project set the run directory to the directory containing the solution file. You should also set `Run on external console`. 

### If you prefer the command line and have Mono installed
In the repository root:
```
> xbuild
> mono bin/Ch24.exe
```
