: /char ( addr n c -- addr n addr n )
  -rot 2dup 2>r rot
  ( addr n c : addr n )
  scan dup 2r> rot -
  ( addr n addr n )
  2>r 1 /string 0 max 2r> ;

: replace ( addr n to from -- addr n )
  2over 0 do
    ( to from addr ) 
    2dup c@ = if rot 2dup swap c! -rot then
    1+
  loop 2drop drop ;

: s>b 2 base ! evaluate decimal ;
: bin. ( n -- ) base @ swap 2 base ! . base ! ;

10 value num-digits

: read-goal ( addr n -- addr n addr n )
  1 /string ']' /char ;
: parse-goal ( addr n -- goal )
  dup to num-digits 
  '0' '.' replace '1' '#' replace s>b ;

: s>n 0 0 2swap >number 2drop drop ;
: 2^ ( n -- 2^n ) 1 swap lshift ;

: read-button ( addr n -- addr n addr n )
  '(' scan 1 /string 32 /char ',' ')' replace ; \ make it easy to parse
: parse-button ( addr n -- n )
  0 -rot begin ',' /char s>n num-digits swap - 1- 2^ 3 roll + -rot dup 0= until 2drop ;

10 constant \n
: until-char ( addr n c -- addr n ) >r 2dup r> scan nip - ;
: machine-end? ( addr n -- b )
  \n until-char '(' scan nip 0= ;

: read-machine \n /char ; 
: parse-machine ( addr n -- goal addr n )
  read-goal parse-goal -rot here -rot 0 -rot
  begin
    read-button parse-button ,
    rot 1+ -rot
    2dup machine-end?
  until 2drop ;

create bools num-digits 2^ allot
create bools-buf num-digits 2^ allot
: num-bools num-digits 2^ ;

: mark-bool ( n -- ) bools-buf + true swap c! ;
: press-button ( n -- ) 
  bools num-bools 0 do dup c@ if over i xor mark-bool then 1+ loop 2drop ;
: press-buttons ( addr n -- )
  0 do dup @ press-button cell + loop drop
  bools-buf bools num-bools move ;

: complete? ( goal -- b )
  bools + c@ ;
: init-bools ( -- )
  num-bools dup bools swap erase true bools c!
  bools bools-buf rot move ;

: print ( addr n )
  0 do dup c@ . 1+ loop drop ;
  
: min-presses ( goal addr n -- n )
  init-bools
  0 -rot begin 
    ( goal count addr n ) 
    3 pick complete? if 2drop nip exit then
    2dup press-buttons
    rot 1+ -rot
    again ;

: partone ( addr n )
  0 -rot
  begin
    read-machine parse-machine min-presses 3 roll + -rot
    dup 0=
  until 2drop ;
