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

16 2 2 multi-cell-array heap-new constant solutionarray
