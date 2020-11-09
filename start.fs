\ start.fs
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
\ Start of the 2d puzzle.  Need to start somewhere so here it is.

require ./Gforth-Objects/objects.fs
require ./Gforth-Objects/mdca-obj.fs
require ./Gforth-Objects/double-linked-list.fs

16 1 multi-cell-array heap-new constant board-test-array \ make the board test array
16 0 do double-linked-list heap-new i board-test-array bind multi-cell-array cell-array! loop \ make all the linked list and put them in array
: bta! { nboard-test-array-index nlistvalue -- } \ store nlistvalue into board-test-array index list
  nlistvalue
  nboard-test-array-index board-test-array cell-array@
  [bind] double-linked-list ll-cell! ;
: nbta! ( nbtl ... nxbtl nbtlqnt nboard-test-array-index -- ) \ store the list nbtl to nxbtl of nbtlqnt quantity inside board test array at index
\ nboard-test-array-index
  { nboard-test-array-index }
  0 ?do nboard-test-array-index swap bta! loop ;
: bta@ ( nboard-test-array-index -- nboard-test-linked-list ) \ return the linked list for board test from board test array
  [bind] multi-cell-array cell-array@ ;

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


\\\
[ifundef] destruction
  interface
     selector destruct ( object-name -- ) \ to free allocated memory in objects that use this
  end-interface destruction
[endif]

object class \
  destruction implementation
  protected

end-class
