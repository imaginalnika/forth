: s s" input-5.txt" slurp-file ;

: /scan ( a n c -- a n a n )
  ( ugly as hell )
  >r 2dup 2dup r@ scan nip -
  r> -rot 2>r scan 
  1 /string 2r> ;

10 constant \n
: /newline ( a n -- a n a n )
  \n /scan ;
: range-s s" 10-14" ;
: split-range ( a n -- n N )
  ( s" 10-14" -- 10 14 )
  '-' /scan evaluate -rot evaluate ;

: num-chars { a n c -- n }
  a n 0 -rot begin c scan 1 /string rot 1+ -rot dup -1 = until 2drop 1- ;

: two! { a b arr }
  a arr !
  b arr 1 cells + ! ;

: idx@ ( addr idx )
  cells + @ ;

: get-num-ranges ( a n -- n ) '-' num-chars ;
s get-num-ranges constant num-ranges
create ranges-arr num-ranges 2 * cells allot
: build-ranges ( arr a n -- )
  num-ranges 0 do
    ( arr a n )
    /newline split-range 
    ( arr a n low high )
    4 roll dup >r two!
    ( a n : arr )
    r> 2 cells + -rot
    ( arr+2 a n )
    loop 2drop drop ;
ranges-arr s build-ranges

: ids-section ( a n -- a n )
  s\" \n\n" search drop 2 /string ;

variable fresh-count

: range@ ( arr -- n N )
  dup @ swap 1 cells + @ ;

: in-range ( id n N -- b )
  1+ within ;

: fresh? ( id ranges-arr num-ranges )
  false swap 0 do 
    ( id ranges-arr b )
    drop 
    2dup range@ 
    ( id ranges-arr id n N )
    in-range if true leave then
    ( id ranges-arr )
    2 cells +
    false
  loop -rot 2drop ; 

: ++! 1 swap +! ;

: part-one ( -- n )
  s ids-section 0 -rot
  2dup \n num-chars 0 do 
    ( acc a n )
    /newline evaluate
    ( acc a n id )
    ranges-arr num-ranges fresh?
    ( acc a n b )
    3 roll + -rot
    loop 2drop negate ;

( sort ranges by start and end, then think of all cases )
( low  high )
( low' high' )

( low high )
( low'  high )

( low     high )
(   low high )

( low     high )
(   low   high )

( low     high )
(   low     high )

( roll-based insertion sort should work )

: range-section ( a n -- a n )
  2dup s\" \n\n" search drop nip
  ( a n len-ids-section )
  - ;

: /range ( a n )
  \n /scan split-range ;
: acc-init 0 -rot ;
: acc-max 3 roll max -rot ;


: ranges cells 2 * ;
: range-cmp { a b c d -- bool }
  a c = if b d < else a c < then ;
: range++ 1 ranges + ;

: insert-range-idx { arr num-ranges low high -- idx }
  num-ranges 0= if 0 else
    arr 0 num-ranges 1- do
      dup i ranges + range@ low high 
      range-cmp if i 1+ leave then
      i 0= if 0 leave then
      -1 +loop nip 
  then ;

: move-back-a-range ( addr num-ranges )
  ranges >r
  dup range++ r> move ;

: print-array ( arr len )
  0 do dup i cells + @ . loop drop ;

: sort-last-range { arr num-ranges -- }
  ( assumes first n-1 ranges sorted )
  arr num-ranges 1- 
  2dup ranges + range@ 2dup 2>r
  insert-range-idx 
  ( idx : low high ) 

  \ arr num-ranges 2 * print-array newline type
  dup dup ranges arr + swap num-ranges swap - 1-
  move-back-a-range
  
  ranges arr +
  dup 1 cells + r>  swap !
  r> swap ! ;

: sort-ranges ( arr num-ranges )
  newline type
  1+ 2 do dup i sort-last-range loop drop ;

: consume-range { acc high low' high' -- acc' high* }
  high low' <= if
    high' low' - acc +  high low' <> if 1+ then
    high'
  else
    high high' < if
      high' high - acc + high'
    else
      acc high
    then
  then ;

: part-two
  ranges-arr num-ranges sort-ranges
  0 0 num-ranges 0 do
    ranges-arr i ranges + range@ consume-range
  loop drop ;
