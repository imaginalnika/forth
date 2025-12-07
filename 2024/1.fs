: at ( arr idx -- val ) cells + @ ;
: last ( arr n -- val ) 1- at ;

: insert-idx ( arr n val -- idx )
  >r 1+
  begin
    1-
    ( arr n )
    dup 0= if
      drop 0 true 
      ( arr 0 true )
    else
      2dup last r@ <
      ( arr n last<val )
    then
  until ( arr idx )
  nip rdrop ;

: /array ( arr len n -- array' len' ) tuck - -rot cells + swap ;
: -rot-array ( arr n -- )
  2dup last >r
  >r dup dup r> ( arr arr arr n ) 1 /array cells move
  r> ( arr last ) swap ! ;

: insert-last-into-sorted ( arr n -- )
  ( assumes first n-1 elems is sorted )
  2dup 2dup last insert-idx ( arr n idx )
  /array -rot-array ;
  ( there's probably an off-by-one error here )

: sort ( arr n -- )
  dup 1 <= if 2drop
  else
    1+ 2 do dup i insert-last-into-sorted loop
    drop
  then ;

create arr1 1000 cells allot
create arr2 1000 cells allot

: arr++ 1 cells + ;
: 2arr++ arr++ swap arr++ swap ;
: first-line 14 min ;
: rest-lines dup 14 min /string ;
: s s" 39472   15292 41795   28867 " ;
: 2interleave >r swap r> ;

: load-lines ( arr1 arr2 "s" -- )
  begin
    2>r 2dup 2r@ first-line evaluate 2interleave
    ( arr1 arr2 arr1 v1 arr2 v2 )
    swap ! swap ! 2arr++
    ( arr1+1 arr2+1 )
    2r> rest-lines dup 0=
  until 2drop 2drop ; 

: input-text s" input-1.txt" slurp-file ;

arr1 arr2 input-text load-lines
arr1 1000 sort
arr2 1000 sort

: distance ( arr1 arr2 -- n ) @ swap @ - abs ;
: total-distance ( arr1 arr2 len -- n )
  0 swap 0 
  ( arr1 arr2 0 len 0 )
  do 
    >r 2dup distance r> + 
    -rot 2arr++ rot loop
  -rot 2drop ;

arr1 arr2 1000 total-distance ( Part One )

\ create test-arr2 4 , 3 , 5 , 3 , 9 , 3 ,
\ create test-arr1 3 , 4 , 2 , 1 , 3 , 3 ,

: 1-if-val-in-arr ( val arr -- b )
  @ = negate ;

: +3rd ( a b c d -- a+d b c )
  >r rot r> + -rot ;

: similarity-score ( val arr len -- n )
  >r 0 -rot r> 0
  ( 0 val arr len 0 )
  do
    ( acc val arr )
    2dup 1-if-val-in-arr
    +3rd arr++ loop drop * ;

: total-similarity-score { arr1 arr2 len -- }
  arr1 0 arr1 @ arr2 len len 0
  do
    ( arr1 acc val arr2 len )
    2dup 2>r similarity-score + 
    ( arr1 acc )
    swap arr++ swap over @
    ( arr1' acc' val )
    2r> loop drop drop drop swap drop ;

arr1 arr2 1000 total-similarity-score ( Part Two )
