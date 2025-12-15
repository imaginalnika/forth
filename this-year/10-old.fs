: s s" input-test-10.txt" slurp-file ;
10 constant max-num-digits \ must set this manually
max-num-digits value num-digits
: 2^ ( n -- 2^n ) 1 swap lshift ;
: num-bools num-digits 2^ ;
create bools num-bools allot
bools num-bools erase
true bools c! \ initial state
create bools-buf num-bools allot

: press-state ( button state -- )
  xor bools-buf + true swap c! ;
: press ( button -- )
  bools num-bools 0 do
    dup c@ if over i press-state then 1+
  loop 2drop ;
: bin. ( n -- ) base @ swap 2 base ! . base ! ;
: nl newline type ;
: s. ( -- )
  nl num-bools 0 do
    bools i + c@ if i bin. then
  loop ;


: press-all ( addr n -- )
  bools bools-buf num-bools move
  0 do dup @ press cell + loop drop
  bools-buf bools num-bools move ;

: init-bools ( -- ) bools num-bools erase true bools c! ;
: constructable? ( goal -- b ) bools + c@ ;
: min-presses ( goal buttons-addr n )
  init-bools
  0 -rot
  ( goal count buttons-addr n )
  begin
    3 pick constructable? if 2drop nip exit then
    rot 1+ -rot
    2dup press-all
  again ;


: /scan ( addr n c )
  >r 2dup r> scan
  tuck 2>r - 2r> 1 /string 0 max 2swap ;

: s>n 0 0 2swap >number 2drop drop ;
: s>b 2 base ! evaluate decimal ;
: >goal ( addr n -- goal )
  2dup 0 do dup c@ case
    '.' of '0' over c! endof
    '#' of '1' over c! endof
    endcase 1+ loop drop s>b ;

: make-arr ( ... n -- addr n ) 
  here >r >r r@ 0 do , loop 2r> ;
: c>bits ( c -- bits ) 48 - num-digits 1- swap - 2^ ;

: >button ( addr n -- button )
  0 -rot ( bits )
  1+ 2 / 0 do 
    dup c@ c>bits 
    ( bits addr bits )
    rot + swap 
    2 + loop drop ;
: line-min-presses ( addr n -- n )
  1 /string
  ']' /scan dup to num-digits >goal >r
  2 /string
  0 -rot
  begin
    ( ... num-buttons addr n )
    ')' /scan >button
    ( ... num-buttons addr n button )
    -rot 2>r swap 1+ 2r>
    ( ... button num-buttons addr n )
    2 /string
    over 1- c@ '{' =
  until
  2>r make-arr 2r> 2swap 
  ( addr n buttons-arr n : goal )
  r> -rot min-presses -rot 2drop ;

: /oneline ( addr u -- addr u addr u )
  2dup 10 scan tuck 2>r -
  2r> 1 /string 2swap ;

: partone
  0 s begin /oneline line-min-presses
    3 roll + -rot
    dup 0=
  until 2drop ;
