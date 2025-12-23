: input-test s" input-test-10.txt" slurp-file ;
: input s" input-10.txt" slurp-file ;

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
: buttons-end? ( addr n -- b )
  drop c@ '(' <> ;

: read-and-parse-buttons ( addr n -- buttons-addr n addr n )
  here -rot 0 -rot
  begin
    read-button parse-button ,
    rot 1+ -rot
    \ 2dup buttons-end?
    2dup buttons-end?
  until ;

: read-machine \n /char ; 
: parse-machine ( addr n -- goal buttons-addr n )
  read-goal parse-goal -rot read-and-parse-buttons 2drop ;

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
  0 do dup @ . cell + loop drop ;
: cprint ( addr n )
  0 do dup c@ . cell + loop drop ;
  
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

: read-joltages ( addr n -- addr n addr n )
  \n /char 1 /string ',' '}' replace ;

: parse-joltages ( addr n -- addr n )
  here -rot 0 -rot 
  begin ',' /char s>n , rot 1+ -rot dup 0= until 2drop ;

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

: last-non-zero ( addr n -- addr )
  2dup num-trailing-zeros 1+ - cells + ;
: /last-non-zero ( addr n -- addr n )
  2dup num-trailing-zeros 1+ -rot last-non-zero swap ;

: --! ( addr -- ) -1 swap +! ;
: ++! ( addr -- ) 1 swap +! ;
: steal ( addr -- ) dup cell - ++! --! ;
: lexico-next ( addr n -- )
  2dup /last-non-zero over steal lexico-push ;

: lexico-end? ( addr n -- b )
  tuck num-trailing-zeros 1+ = ;

create lexico-buf 10 cells allot
: last ( addr n ) 1- cells + ;
: lexico-start ( num-buttons n -- addr n )
  lexico-buf -rot >r 2dup last r> swap !
  ( lexico-buf num-buttons )
  2dup 1- cells erase ;

: parse-button-into-arr ( addr n -- n ) 
  \ key differenec is we don't use bit representations anymore, so no need to "flip" with num-digits
  0 -rot begin ',' /char s>n 2^ 3 roll + -rot dup 0= until 2drop ;

: parse-machine-joltages ( addr n -- joltage-goal-addr buttons-addr n )
  read-goal parse-goal drop
  here -rot 0 -rot
  begin
    read-button parse-button-into-arr , \ change this
    rot 1+ -rot
    \ 2dup buttons-end?
    2dup buttons-end?
  until
  read-joltages parse-joltages 2nip 2swap rot drop ;

\ lexico and buttons have num-buttons
\ joltage-goal has num-digits

create candidate-joltage-buf 10 cells allot
: light? ( button n )
  2^ and 0<> ;
: press-n-times! ( button n -- )
  num-digits 0 do over i light? if dup ( n ) candidate-joltage-buf i cells + +! then loop 2drop ;

: d@ @ swap @ swap ;
: dcell+ cell+ swap cell+ swap ;
: joltage-candidate ( buttons-addr lexico-addr n -- candidate-joltage-addr )
  candidate-joltage-buf num-digits cells erase
  0 do dup @ 0<> if 2dup d@ ( button n ) press-n-times! then dcell+ loop 2drop candidate-joltage-buf ;

: 3dup 2 pick 2 pick 2 pick ;
: arr= ( addr addr n ) >r true -rot r>
  0 do 2dup d@ <> if rot drop false -rot leave then dcell+ loop 2drop ;

: n-reachable? ( buttons-addr n joltage-goal n -- b )
  swap { joltage-goal }
  lexico-start
  begin 
  \ 2dup print space 
  3dup joltage-candidate 
  \ dup num-digits print newline type 
  joltage-goal 
  \ dup num-digits print newline type
  num-digits arr= if 2drop drop true exit then 2dup lexico-end? >r lexico-next r> until 2drop drop false ;
  ( buttons-addr lexico-addr n )
variable parttwo-count

: acc-count ( joltage-goal-addr buttons-addr n )
  rot 1000 1 do i . 3dup i n-reachable? if i parttwo-count +! leave then loop 2drop drop ;
: parttwo begin read-machine parse-machine-joltages acc-count dup 0= until 2drop parttwo-count @ ;

( fuck... linear algebra? are you serious? lol )

: h candidate-joltage-buf num-digits print ;
: m lexico-buf num-digits print ;
