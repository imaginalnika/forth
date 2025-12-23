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

: move-and-zero-out ( addr addr u -- )
  >r 2dup r> move over - erase ;

: push-move-cells ( addr len numzeros -- )
  tuck - >r cells over + r> cells move-and-zero-out ;

: num-trailing-zeros ( addr len -- n )
  0 -rot tuck cells + swap 0 do
    cell - dup @ 0= if swap 1+ swap else leave then
  loop drop ;

: lexico-push ( addr n -- )
  2dup num-trailing-zeros push-move-cells ;

: f here 3 , 0 , 0 , 3 2dup lexico-push print ;

: last-non-zero ( addr n -- addr )
  2dup num-trailing-zeros 1+ - cells + ;
: /last-non-zero ( addr n -- addr n )
  2dup num-trailing-zeros 1+ -rot last-non-zero swap ;

: --! ( addr -- ) -1 swap +! ;
: ++! ( addr -- ) 1 swap +! ;
: steal ( addr -- ) dup cell - ++! --! ;
: lexico-next ( addr n -- )
  /last-non-zero over steal lexico-push ;

: lexico-end? ( addr n -- b )
  tuck num-trailing-zeros = ;

: lexico-start ( n -- addr n )
  here swap dup 1- 0 do
    0 ,
  loop dup , ;

( now need to know what pressing all the buttons according to lexico will do )


variable seed
here seed !
: rnd ( -- n )
  seed @ 6364136223846793005 * 1+ dup seed ! 33 rshift ; \ musl implementation
: randbool ( n -- b )
  rnd 2 mod 0= ;
: rand-button ( num-digits -- button )
  0 swap 0 do randbool if i 2^ + then loop ;

: light? ( button n )
  2^ and 0<> ;
: nth-joltage+! ( addr n amount -- ) -rot cells + +! ;

: button+! ( joltage-addr n button amount -- )
  { amount } swap 0 do dup i light? if over i amount nth-joltage+! then loop 2drop ;

: buttons+! ( joltage-addr n buttons-addr n -- )
  0 do dup @ >r -rot 2dup r> button+! cell + loop ;

: wires? ( button n -- b )
  rshift 1 and 0<> ;
: joltage++ ( joltage-addr n -- )
  cells + 1 swap +! ;
: joltage-press-button ( joltage-addr button -- )
  num-digits 0 do dup i wires? if over i joltage++ then loop 2drop ;
\ : joltage-press-buttons ( joltage-addr buttons-addr n -- )
\   0 do 2dup @ joltage-press-button cell + loop 2drop ;
: duo-cell+ cell + swap cell + swap ;
: duo-@ @ swap @ swap ;
: joltage<= ( addr addr )
  num-digits 0 do 2dup duo-@ > if 2drop false unloop exit then duo-cell+ loop 2drop true ;

: last ( addr n -- addr )
  1- cells + ;

