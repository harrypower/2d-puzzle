\ boardpieces.fs
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

16 1 multi-cell-array heap-new constant board-test-array \ make the board test array
: maketest 16 0 do double-linked-list heap-new i board-test-array [bind] multi-cell-array cell-array! loop ;
maketest
\ make all the linked lists and put them in array

: bta! { nboard-test-array-index nlistvalue -- } \ store nlistvalue into board-test-array index list
  nlistvalue
  nboard-test-array-index board-test-array cell-array@
  [bind] double-linked-list ll-cell! ;
: nbta! ( nbtl ... nxbtl nbtlqnt nboard-test-array-index -- ) \ store the list nbtl to nxbtl of nbtlqnt quantity inside board test array at index
\ nboard-test-array-index
  { nboard-test-array-index }
  0 ?do nboard-test-array-index swap bta! loop ;
: bta@ ( nboard-test-array-index -- nboard-test-linked-list ) \ return the linked list for board test from board test array
  board-test-array [bind] multi-cell-array cell-array@ ;

1 4 5 3 0 nbta!
0 2 4 5 6 5 1 nbta!
1 3 5 6 7 5 2 nbta!
2 6 7 3 3 nbta!
0 1 5 8 9 5 4 nbta!
0 1 2 4 6 8 9 10 8 5 nbta!
1 2 3 5 7 9 10 11 8 6 nbta!
2 3 6 10 11 5 7 nbta!
4 5 9 12 13 5 8 nbta!
4 5 6 8 10 12 13 14 8 9 nbta!
5 6 7 9 11 13 14 15 8 10 nbta!
6 7 10 14 15 5 11 nbta!
8 9 13 3 12 nbta!
8 9 10 12 14 5 13 nbta!
9 10 11 13 15 5 14 nbta!
10 11 14 3 15 nbta!
\ this should be the complete board test data


[ifundef] destruction
  interface
     selector destruct ( object-name -- ) \ to free allocated memory in objects that use this
  end-interface destruction
[endif]

object class \
  destruction implementation
  protected
  cell% inst-var board-array
  cell% inst-var board-start
  cell% inst-var piece-count-array
  \ public
  m: ( npiece nindex aboard -- ) \ store npiece in board at nindex
    board-array @ [bind] multi-cell-array cell-array!
  ;m method board!
  m: ( nindex aboard -- npiece ) \ retreive npiece from nindex of board
    board-array @ [bind] multi-cell-array cell-array@
  ;m method board@
  m: ( npiece nindex aboard -- ) \ store npiece in start board at nindex
    board-start @ [bind] multi-cell-array cell-array!
  ;m method start!
  m: ( nindex aboard -- npiece ) \ rerieve npiece from nindex of start board
    board-start @ [bind] multi-cell-array cell-array@
  ;m method start@
  m: ( npiece aboard -- ) \ retreive piece count
    piece-count-array @ [bind] multi-cell-array cell-array@
  ;m method piece@
  m: ( npiece aboard -- nflag ) \ test if npiece is at its limit for use
    \ true returned if npiece can be added to
    \ false returned if npiece can not be added to
    { npiece }
    npiece 4 = if npiece this piece@ 4 = then
    npiece 4 < if npiece this piece@ 3 = then
    invert
  ;m method piece?
  m: ( nqnt npiece aboard -- ) \ store nqnt into the piece array
    piece-count-array @ [bind] multi-cell-array cell-array!
  ;m method piece!
  m: ( npiece aboard -- ) \ add a count to piece
    dup this piece@ 1 + swap this piece!
  ;m method piece+
  m: ( npiece nindex aboard -- nflag ) \ test if npiece can be placed at nindex on board
  \ nflag is true if it can be placed.. false if cannot be placed
    { npiece nindex }
    nindex this start@ npiece = if false \ test if npiece is same as base piece then npiced cannot be placed
    else
      nindex bta@ [bind] double-linked-list ll-set-start
      begin
        nindex bta@ [bind] double-linked-list ll-cell@ dup this board@ true = if this start@ else this board@ then
        npiece = if false true else nindex bta@ [bind] double-linked-list ll> false = if false else true true then then
      until
    then \ test if the piece can be placed on board
    npiece this piece?  \ test if the piece can even be used at all
    and
  ;m method btest
  public
  m: ( aboard -- )
    16 1 multi-cell-array heap-new board-array !
    16 1 multi-cell-array heap-new board-start !
    5 1 multi-cell-array heap-new piece-count-array !
    0 0  this start!
    3 1  this start!
    0 2  this start!
    2 3  this start!
    4 4  this start!
    2 5  this start!
    4 6  this start!
    1 7  this start!
    3 8  this start!
    1 9  this start!
    3 10 this start!
    2 11  this start!
    2 12  this start!
    0 13  this start!
    4 14  this start!
    1 15  this start!
    16 0 do true i this board! loop
    5 0 do 0 i this piece! loop
  ;m overrides construct

  m: ( aboard -- )
    board-array @ [bind] multi-cell-array destruct
    0 board-array !
    board-start @ [bind] multi-cell-array destruct
    0 board-start !
    piece-count-array @ [bind] multi-cell-array destruct
    0 piece-count-array !
  ;m overrides destruct

  m: ( npiece nindex aboard -- nflag ) \ test if npiece can be placed on board
  \ nflag is false if npiece cannot be placed on board true if it can be placed
    { npiece nindex }
    nindex this board@ true = if \ test if a piece has been placed then another piece can not be placed
      npiece nindex this btest  \ ( -- nflag ) now test if the piece can be placed on the base board
    else false then
  ;m method boardtest?

  m: ( npiece nindex aboard -- nflag ) \ store npiece on board nflag is false if npiece cannot be placed on board true if it can be placed
    { npiece nindex }
      npiece nindex this boardtest? true = if npiece nindex this board! npiece this piece+ true else false then
  ;m method boardput

  m: ( nindex aboard -- npiece ) \ retrieve npiece from board at nindex
    dup this board@ true = if this start@ else this board@ then
  ;m method boardget

  m: ( nx ny aboard -- ) \ display current board
    0 { nx ny nindex }
    \ page
    4 0 do
      4 0 do
        i 3 * nx +
        j  ny + at-xy
        nindex this boardget .
        nindex 1 + to nindex
      loop
    loop
  ;m method displayboard

  m: ( npiece nindex aboard -- nflag ) \ testing
    this btest
  ;m method btest?
end-class aboard
