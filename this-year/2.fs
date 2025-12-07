: odd? 2 mod 1 = ;
: split-halves 2/ 2dup ( a n/2 ) tuck + swap ( a+n/2 n/2 ) ;
: same-halves?
  dup odd? if 2drop 0
  else split-halves compare 0= then ;

: id>str s>d <# #s #> ;
: sum-invalids ( end start -- acc )
  0 -rot ( acc end start )
  do i dup id>str same-halves?
    if + else drop then loop ;

: str>id 0 0 2swap >number 2drop drop ;
: split ( "s" delim )
  >r 2dup r> scan nip dup >r - 
  2dup + 1+ r> 1- ; ( probably much easier solution with /string )
: read-range-str ( "a-b" -- b+1 a )
  '-' split str>id 1+ -rot str>id ;

: acc-range-str ( acc "a-b" -- acc' )
read-range-str sum-invalids + ;

: sum-csv ( "s" -- acc )
  0 -rot
  begin dup 0>
  while ',' split 2>r read-range-str sum-invalids + 2r>
  repeat 2drop ;

: 4dup 2over 2over ;
: prefix? ( prefix str ) drop over compare 0= ; 
: consume-prefixes ( prefix string -- leftover-string )
  rot dup >r -rot begin 4dup prefix? while r@ /string repeat 2swap 2drop r> . ;
: n-repeat? ( "s" n )
  2dup mod 0= if
    >r 2dup drop r> 2swap ( n-prefix "s" ) consume-prefixes nip 0=
  else
    2drop drop 0
  then ;

: repeat? ( addr len -- flag )
  dup 1 = if 2drop false
  else 
    dup 2/ 1+ 1 do
      2dup i n-repeat?
      if 2drop true unloop exit then
    loop 2drop false
  then ;

: sum-new-invalids ( end start -- acc )
  0 -rot ( acc end start )
  do i dup id>str repeat?
    if + else drop then loop ;

: sum-new-csv ( "s" -- acc )
  0 -rot
  begin dup 0>
  while ',' split 2>r read-range-str sum-new-invalids + 2r>
  repeat 2drop ;

