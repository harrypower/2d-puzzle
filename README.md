# 2d-puzzle
*This is a 4X4 2d puzzle i was trying to solve so i made a little program*
* 2dpuzzle.fs is the main program file that you load with Gforth as follows
```
gforth 2dpuzzle.fs
```
  * To run code execute the word `main` at the gforth console
  * This main word will simply solve all possible combinations of puzzle via brute force method.
  * During the solving you will see some form of a 4 X 4 board with numbers representing the puzzle pieces along with other info
  * At the end of all combinations it will show you the solutions and the step by step order for that solution along with other info

* boardpieces.fs
  * This file contains an object as in objects.fs ( called aboard) for board placement and displaying and piece placeing on board.  
  * This object is used by the other files.

* piecelevel.fs
  * This file contains an object called apiecelevel.
  * This object simply stores aboard object and all possible pieces that can be placed on that board.
  * This object is primarily used to solve the puzzle in 2dpuzzle.fs
