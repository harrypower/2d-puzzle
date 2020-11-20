\ 2dpuzzle.fs
\    Copyright (C) 2020 Philip K. Smith
\    This program is free software: you can redistribute it and/or modify
\    it under the terms of the GNU General Public License as published by
\    the Free Software Foundation, either version 3 of the License, or
\    (at your option) any later version.

\    This program is distributed in the hope that it will be useful,
\    but WITHOUT ANY WARRANTY; without even the implied warranty of
\    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
\    GNU General Public License for more details.

\    You should have received a copy of the GNU General Public License
\    along with this program.  If not, see <http://www.gnu.org/licenses/>.
\
\ top level trying to solve 2d puzzle

require ./Gforth-Objects/objects.fs
require ./Gforth-Objects/mdca-obj.fs
require ./Gforth-Objects/double-linked-list.fs
require ./piecelevel.fs

16 2 2 multi-cell-array heap-new constant solutionarray  \ ( apiecelevel , solutionindex )
double-linked-list heap-new constant solutionslist

: startsolutionarray ( uindex -- ) \ empty solutionarray from uindex to end of array
  16 swap do
    0 i 0 solutionarray cell-array!    \ store no board object
    true i 1 solutionarray cell-array!  \ store no index
  loop ;
0 startsolutionarray
aboard heap-new apiecelevel heap-new 0 0 solutionarray cell-array! \ place beginning piecelevel at index 0
0 0 1 solutionarray cell-array! \ start at first piece

0 value solutionedge \ this is the current location where solution is at
: getNboard ( nsolutionindex -- uaboard ) \ return uaboard for a given usolutionindex level
  0 solutionarray cell-array@ theboard@ ;
: gettotallvlsolutions ( nsolutionindex -- usoluionstotal ) \ return usoluionstotal on a given nsolutionindex level
  0 solutionarray cell-array@ piecesfound? ;
: getcurrentlvlsolution# ( nsolutionindex -- ucurrentsolution# ) \ return the current solution # working on for nsolutionindex
  1 solutionarray cell-array@ ;
: addtocurrentlvlsolution# ( nsolutionindex -- ) \ add 1 to nsolutionindex solutionindex value so getNpieceindex will return next lvl solution
  dup getcurrentlvlsolution# 1 +
  swap 1 solutionarray cell-array! ;
: getNpieceindex ( nsolutionindex -- npiece nindex ) \ return niece nindex at nsolutionindex solutionlevel note this will only get next npiece nindex if there are more solutions at this lvl only
  dup getcurrentlvlsolution# \ ( nsolutionindex nlvlsolutionfornsolutionindex )
  swap 0 solutionarray cell-array@ thepieces@ swap ;

: solutionboard@ ( -- uaboard nflag ) \ return new aboard such that it contains current solution move
\ nflag is true if aboard contains current solution
\ nflag is false if the current solution is not valid
  solutionedge getNpieceindex ( -- npiece nindex )
  drop true <> if
    aboard heap-new \ start with empty board then add the current solution pieces
    solutionedge 0 ?do
      dup i getNpieceindex rot boardput drop \ add pieces up to solutionedge
    loop
    dup solutionedge getNpieceindex rot boardput \ add the solutionedge piece ( -- aboard nflag )  note nflag should be true always
  else
    false false
  then ;

: solutionarray! ( napiecelevel -- ) \ store apiecelevel to solutionarray and start at piece 0 update solutionedge index value
  solutionedge 1 + dup to solutionedge
  0 solutionarray cell-array! \ store napiecelevel object
  0 solutionedge 1 solutionarray cell-array! ; \ start with piece 0 from napicelevel object

: addnextlvl ( -- nflag ) \ create and store next piecelevel ... nflag is true for next lvl added nflag is false if no lvl found and non added
  solutionboard@ if
    apiecelevel heap-new
    dup piecesfound? 0 > if solutionarray! true \ a piece found for next level so stort it in solutionarray
     else dup [bind] apiecelevel destruct free throw false then \ no next level found so destruct apiecelevel that was constructed earlier
  else
      dup false <> if [bind] aboard destruct free throw then \ if aboard returned by solutionboard@ is present destruct it here
      false
  then ;

: incsolution ( -- nflag ) \ increment solutionedge solutions ... nflag is true if increment works nflag is false if no more solutions on lvl
  solutionedge gettotallvlsolutions
  solutionedge getcurrentlvlsolution# 1 + <> if \ current solution on this solutionedge lvl is not at end yet so stay at lvl and advance solution
    solutionedge addtocurrentlvlsolution# true
  else false then ;

: decsolutionindex ( -- nflag ) \ decriment solutionedge but clean up memory before .. if at 0 for solutionedge then return false else return true
  solutionedge 0 <> if \ if on any lvl other then 0 drop that level and clean up and return true
    solutionedge 0 solutionarray cell-array@ [bind] apiecelevel destruct
    solutionedge startsolutionarray \ ensuring solutionarray past edge is empty ... note a memory leak might happen here need to look into
    solutionedge 1 - to solutionedge
    true
  else \ if on lvl 0 then there is no soluiton to puzzle return false
    false
  then ;

: backuplvl ( -- nflag ) \ backup on level from current solution ... nflag is true if backup to another solution works false if no more solutions on this lvl
  recursive
  incsolution false = if
    decsolutionindex false = if \ there is no solution return false
      false
    else \ lvl was decrimented now test if this lvl has anymore solutions
      backuplvl
    then
  else true then ;

0 value solutionmax
50 value displayskip
0 value displaystep
0 value solutions
: displayit ( -- )
displaystep 1 + to displaystep
displaystep displayskip > solutionedge 15 >= or if
  0 to displaystep
  0 0 page solutionedge getNboard displayboard
  0 5 at-xy solutionedge . ." pieces for current solution" cr
  solutionedge solutionmax max to solutionmax
  solutionmax . ." current max solutions found" cr
  0 getcurrentlvlsolution# . ." depth into first lvl" cr
  solutions . ." current solutions"
then ;

: solveit ( -- )
  begin
    begin
      solutionedge 15 >= if
        true
      else
        addnextlvl invert
      then
    until
    displayit
    solutionedge 15 >= if
      true
    else
      backuplvl invert
    then
  until ;

: addsolution ( -- ) \ get current solution and add it to solutionslist
  16 2 2 multi-cell-array heap-new { ustorage } \ ( npiece , nindex ) < this will be how the solution data is stored in array 
  ustorage solutionslist ll-cell! \ store the array that will contain the pieces for solution
  16 0 do true i 0 ustorage cell-array! true i 1 ustorage cell-array! loop \ empty solution list first
  solutionedge 1 + 0 do
    i getNpieceindex i 1 ustorage cell-array! i 0 ustorage cell-array!
  loop
  solutions 1 + to solutions ;

: findanswers ( -- ) \ find all solutions
  begin
    solveit
    solutionedge 15 >= if \ found a solution add it
      addsolution then
    backuplvl invert
  until ;
