: until-char >r 2dup r> scan nip - ;
: trim begin 
    dup 0= if exit then
    over c@ 32 <> if exit then 
    1 /string again ;

: last-line ( addr len ) + 1- 1
  begin -1 /string over c@ 10 = until 1+ 1 /string ;
: num-chars { addr len c }
  0 addr 
  len 0 do 
    dup i + c@ c = negate
    rot + swap loop drop ;
: sf s" input-6.txt" slurp-file ;

sf last-line 2dup '*' num-chars -rot '+' num-chars + constant num-cols
create syms num-cols allot
: fill-syms
  sf last-line num-cols 0 do trim over c@ syms i + ! 1 /string loop 2drop ;
fill-syms
create results num-cols cells allot

: fill-results
  num-cols 0 do syms i + c@ 
    case
      '*' of 1 endof
      '+' of 0 endof
      throw
    endcase
    results i cells + !
  loop ;
fill-results

: log num-cols 0 do results i cells + @ . loop ;

: s>n  ( addr u -- n )
  0 0 2swap >number 2drop drop ;
: /number ( addr len -- addr len num )
  2dup 32 scan trim 2swap 32 until-char s>n ;

: compute ( n n sym  )
  case
    '*' of * endof
    '+' of + endof
    throw
  endcase ;

: compute-row ( addr len )
  trim num-cols 0 do
    /number 
    results i cells + @
    syms i + c@
    compute results i cells + !
    loop 2drop ;

sf 10 num-chars 1- constant num-rows

: /row ( addr len )
  2dup 10 scan 1 /string 2swap 10 until-char ;

: compute-grid ( addr len )
  num-rows 0 do /row compute-row loop 2drop ;

: sum ( addr len )
  0 -rot 0 do dup i cells + @ rot + swap loop drop ;

: part-one
  sf compute-grid
  results num-cols sum ;

create vertical-buf num-rows allot
: vertical-num { offset }
  sf vertical-buf num-rows 0 do 
    ( addr len row-buf )
    2 pick offset + c@ dup 32 <> if
      over c! 1+
    else drop
    then
    >r 10 scan 1 /string r> loop 
    vertical-buf - >r 2drop vertical-buf r> s>n ; 

: next-sym-offset ( offset -- offset )
  dup sf last-line drop + 1+ 
  begin ( offset addr-at-offset ) 
    dup sf + 1+ = if drop 1+ exit then
    dup c@ 32 <> 
    if drop 1+ exit then swap 1+ swap 1+ again ;

: vertical-* ( offset )
  dup next-sym-offset 1- swap 1 -rot do i vertical-num * loop ;
: vertical-+ ( offset )
  dup next-sym-offset 1- swap 0 -rot do i vertical-num + loop ;

: vertical-compute ( offset c )
  case
    '*' of vertical-* endof
    '+' of vertical-+ endof
    throw
  endcase ;

: part-two
  ( the input must be changed a bit )
  0 0 num-cols 0 do
    ( result offset )
    dup syms i + c@ vertical-compute
    rot + swap
    next-sym-offset
    loop drop ;
part-one part-two
