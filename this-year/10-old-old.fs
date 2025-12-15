: s s" input-test-10.txt" slurp-file ;

: /oneline ( addr u -- addr u addr u )
  2dup 10 scan tuck 2>r -
  2r> 1 /string 2swap ;

: blob ( addr u -- blob ) 2dup 32 scan nip - ;
: delimiter ( blob -- delimiter ) drop c@ ;
: num-elems ( blob -- count )
  2dup delimiter case
    '[' of nip 2 - endof
    '(' of nip 1- 2 / endof
    '{' of nip 1- 2 / endof
    throw
  endcase ;

: max-num-digits ( addr u -- n )
  0 -rot
  begin
    dup 0= if 2drop exit then
    /oneline blob num-elems \ [] blob
    3 roll max -rot
  again ;

: 2** ( n -- 2^n ) 1 swap lshift ;
: bin. ( n -- ) base @ swap 2 base ! . base ! ;


variable num-digits
s blob num-elems num-digits !


: s>n 0 0 2swap >number 2drop drop ;
: strip 1 /string 1- ;

: csv ( addr u -- n ) 2dup ',' scan nip - s>n ;
: read-csvs ( addr u -- ... n )
  dup 0= if 2drop then
  0
  begin
    >r
    2dup csv -rot ',' scan dup 0= if true
    else 1 /string false then
    r> 1+ swap
  until -rot 2drop ;
: until-space 2dup 32 scan nip - ;

: /buttons ( addr u -- addr u ... n )
  over c@ '{' = if drop 0 0 exit then \ no more
  '(' scan 2dup ')' scan 2 /string 2swap 
  until-space strip read-csvs ;

: build-bin ( ... n -- bin )
  0 \ acc
  swap 0 do swap num-digits @ swap - 1- 2** + loop ;

: f s /oneline 4 0 do /buttons build-bin bin. loop ;

create bin-steps s max-num-digits 2** cells allot
bin-steps s max-num-digits 2** cells erase

: bin>steps ;

create bit-buf 1 allot
: move-bit 1 move ;
: swap-addr ( addr addr -- )
  dup bit-buf move-bit 2dup move-bit drop bit-buf swap move-bit ;
: reverse ( addr n -- addr n )
  tuck 2 / 0 do 
    ( n addr ) 
    2dup + 1- i - over i + swap-addr
    loop swap ;
\ create arr 2 c, 3 c,

\ 6 value light
\ : push ( arr n -- )
\   arr>bin light xor to light ;

\ create light-scores num-digits 2** allot
\ light-scores num-digits 2** erase
\ : init-light-scores
  
\   ;
