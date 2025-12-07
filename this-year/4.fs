: s s" input-4.txt" slurp-file ;
\ : s s" input-test-4.txt" slurp-file ;

135 value width
\ 10 value width
width width * value max-len

create arr width width * cells allot
: load-grid { arr addr len }
  addr 0
  width 0 do
    width 0 do
      ( addr idx )
      2dup j + ( correct for \n ) + c@ 
      64 ( '@' ) = 
      ( addr idx '@'? )
      over arr swap cells + !
      1+
      loop 
    loop 2drop ;

: idx@  ( arr idx -- n )
  dup dup 0 < swap
  max-len >= or if 
    2drop 0
  else cells + @ then ;

: valid? ( row/col -- b )
  dup 0 >= swap
  width < and ;

: 2d@ ( arr row col -- n )
  2dup valid? swap valid? and
  if swap width * + idx@ 
  else 2drop drop 0 then  ;

: 3dup 2 pick 2 pick 2 pick ;

: idx>2d ( idx -- row col )
  dup width / swap width mod ;

: 4th+ -rot >r >r swap >r + r> r> r> ;

: left  ( row col ) 1- ;
: right ( row col ) 1+ ;
: up    ( row col ) swap 1- swap ;
: down  ( row col ) swap 1+ swap ;

: sum-8-adjacents ( arr idx -- n )
  0 -rot idx>2d
  left 3dup 2d@ 4th+
  up 3dup 2d@ 4th+
  right 3dup 2d@ 4th+
  right 3dup 2d@ 4th+

  down 3dup 2d@ 4th+
  down 3dup 2d@ 4th+
  left 3dup 2d@ 4th+
  left 2d@ + negate ;

: accessible ( arr idx -- b )
  sum-8-adjacents 4 < ;
: is-roll ( arr idx -- b ) idx@ ;

: 2nd+ rot + swap ;

: part-one ( -- n )
  arr s load-grid
  0 arr max-len 0 do
    ( acc arr )
    dup i is-roll if
      dup i accessible if 1 2nd+ then
    then 
  loop drop ;

: remove-some-rolls ( -- b )
  0 arr max-len 0 do
    ( num-removed arr )
    dup i is-roll if
      dup i accessible if 
        ( num-removed arr )
        swap 1+ swap
        0 arr i cells + ! 
      then
    then
  loop drop ;

: part-two ( -- ) 
  arr s load-grid
  0 begin remove-some-rolls dup >r + r> 0= until ;

part-one
part-two
