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
  cell% inst-var theboard

  protected
  m: ( ndata apiecelevel -- ) \ store ndata in mainlist linked list
    mainlist @ [bind] double-linked-list ll-cell!
  ;m method mainlist!
  m: ( uindex apiecelevel -- ndata ) \ retrieve ndata from mainlist at nindex ...
    mainlist @ [bind] double-linked-list nll-cell@
  ;m method mainlist@
  m: ( uboardindex npiece# apiecelevel -- ) \ create and store piece list data
    2 1 multi-cell-array heap-new piecelistarray !
    piecelistarray @ this mainlist!
    1 piecelistarray @ [bind] multi-cell-array cell-array! \ store npiece# in piecelistarray that is furthor stored in the mainlist
    0 piecelistarray @ [bind] multi-cell-array cell-array! \ store nboardindex in piecelistarray
    piecesfound @ 1 + piecesfound ! \ add to pieces found
  ;m method piece+
  m: ( npiecelistarray apiecelevel -- nboardindex npiece# ) \ return board piece level data from npiecelistarray
  \ note this can return true true ... meaning no piece or boardindex to return
    piecesfound @ 0 > if
      this mainlist@
      piecelistarray !
      0 piecelistarray @ [bind] multi-cell-array cell-array@ \ retrieve nboardindex
      1 piecelistarray @ [bind] multi-cell-array cell-array@ \ retrieve npiece#
    else
      true true \ true is returned because there are no pieces or board indexs to return
    then
  ;m method piece@
  m: ( apiecelevel -- ) \ with given board find all the pieces that can be placed
    16 0 do
      5 0 do
        i j \ npiece nindex
        theboard @  \ get the stored board to work with
        [bind] aboard boardtest? if j i this piece+ then \ store uboardindex upiece#
      loop
    loop
  ;m method findpieces

  public
  m: ( naboard apiecelevel -- )
    theboard ! \ store the board handle
    double-linked-list heap-new mainlist !
    0 piecesfound !
    this findpieces
  ;m overrides construct

  m: ( apiecelevel -- )
    theboard @ [bind] aboard destruct \ free up the board
    piecesfound @ 0 ?do i mainlist @ [bind] double-linked-list nll-cell@ [bind] multi-cell-array destruct loop \ free piece data here
    mainlist @ [bind] double-linked-list destruct \ free up list
    0 mainlist !
    0 piecesfound !
    0 theboard !
  ;m overrides destruct

  m: ( apiecelevel -- npiecesfound ) \ return total pieces found
    piecesfound @
  ;m method piecesfound?

  m: ( uindex apiecelevel -- nboardindex npiece# ) \ for given uindex return nboardindex npiece# that can be placed on theboard
    this piece@
  ;m method thepieces@

  m: ( apiecelevel -- naboard ) \ returns the board object used in this piece level object
    theboard @
  ;m method theboard@
end-class apiecelevel
