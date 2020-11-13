\ piecelevel.fs
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
\ makes a board that also handles pieces

require ./Gforth-Objects/objects.fs
require ./Gforth-Objects/mdca-obj.fs
require ./Gforth-Objects/double-linked-list.fs
require ./boardpieces.fs

[ifundef] destruction
  interface
     selector destruct ( object-name -- ) \ to free allocated memory in objects that use this
  end-interface destruction
[endif]

object class \
  destruction implementation
  protected
  cell% inst-var mainlist
  cell% inst-var piecelistarray
  cell% inst-var piecesfound

  protected
  m: ( ndata apiecelevel -- ) \ store ndata in mainlist linked list
    mainlist @ [bind] double-linked-list ll-cell!
  ;m method mainlist!
  m: ( apiecelevel -- ndata nflag ) \ retrieve ndata from mainlist ... nflag is true
  ;m method mainlist@
  m: ( nboardindex npiece# apiecelevel -- ) \ create and store piece list data
    2 1 multi-cell-array heap-new piecelistarray !
    piecelistarray @ this mainlist!
    1 piecelistarray @ [bind] multi-cell-array cell-array! \ store npiece# in piecelistarray that is furthor stored in the mainlist
    0 piecelistarray @ [bind] multi-cell-array cell-array! \ store nboardindex in piecelistarray
    piecesfound @ 1 + piecesfound ! \ add to pieces found
  ;m method piece+
  m: ( npiecelistarray apiecelevel -- nboardindex npiece# ) \ return board piece level data from npiecelistarray
    piecelistarray !
    piecelistarray @
  ;m method piece@

  public
  m: ( naboard apiecelevel -- )
    double-linked-list heap-new mainlist !
    this mainlist! \ store the board that this piecelevel is based on
    0 piecesfound !
  ;m overrides construct

  m: ( apiecelevel -- )
    0 mainlist @ [bind] double-linked-list nll-cell@ [bind] aboard destruct \ free up the board
    piecesfound 0 do i 1 + mainlist @ [bind] double-linked-list nll-cell@ [bind] multi-cell-array destruct loop \ free piece data here
    mainlist @ [bind] double-linked-list destruct \ free up list 
  ;m overrides destruct

  m: ( apiecelevel -- ) \ with given board find all the pieces that can be placed

  ;m method findpieces
end-class apiecelevel
