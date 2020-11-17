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

16 2 2 multi-cell-array heap-new constant solutionarray  ( apiecelevel , solutionindex )

: startsolutionarray ( -- ) \ start solution array empty
  16 0 do
    0 i 0 solutionarray cell-array!    \ store no board object
    true i 0 solutionarray cell-array!  \ store no index
  loop ;
startsolutionarray
aboard heap-new apiecelevel heap-new 0 0 solutionarray cell-array! \ place beginning piecelevel at index 0
0 0 1 solutionarray cell-array! \ start at first piece

0 value solutionedge \ this is the current location where solution is at
: getNboard ( nsolutionindex -- uaboard )
  0 solutionarray cell-array@ theboard@ ;
: getNpieceindex ( nsolutionindex -- npiece nindex )
  dup 1 solutionarray cell-array@
  swap 0 solutionarray cell-array@ thepieces@ swap ;

: solutionboard@ ( -- uaboard nflag ) \ return aboard such that it contains current solution move
\ nflag is true if aboard contains current solution
\ nflag is false if the current solution is not valid
  solutionedge getNpieceindex ( -- npiece nindex )
  drop true <> if
    aboard heap-new \ start with empty board then add the current solution pieces
    solutionedge 0 ?do
      dup i getNpieceindex rot boardput drop \ add pieces up to solutionedge
    loop
    dup solutionedge getNpieceindex rot boardput \ add the solutionedge piece ( -- aboard true )
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
    dup piecesfound? 0 > if solutionarray! true
     else dup [bind] apiecelevel destruct free throw true then
  else
    dup [bind] aboard destruct free throw
    true
  then ;
