: s s" input-8.txt" slurp-file ;

: until-char >r 2dup r> scan nip - ;
: count-char { c } 
  0 -rot begin c scan dup 0<>
    if 1 /string rot 1+ -rot
    else 2drop exit then again ;
: s>n 0 0 2swap >number 2drop drop ;


: /csv-num ( addr u -- addr u n )
  0 0 2swap >number
  dup 0<> if 1 /string then
  rot drop rot ;

: /oneline ( addr u -- addr u addr u )
  2dup 10 scan tuck 2>r -
  2r> 1 /string 2swap ;

: csv>box ( "1,2,3" -- addr )
  here >r
  /csv-num ,
  /csv-num ,
  /csv-num ,
  2drop r> ;

: ++ 1 cells + ;
: x @ ;
: y ++ @ ;
: z ++ ++ @ ;

s 10 count-char constant num-boxes
create boxes num-boxes cells allot

: nth-box ( i ) cells boxes + @ ;

: init-boxes
  s num-boxes 0 do
    /oneline csv>box i cells boxes + !
  loop 2drop ;
init-boxes

num-boxes 1- dup 1+ * 2 / constant num-juncts
3 cells constant junct-size
2 cells constant dist-offset
create juncts num-juncts junct-size * allot \ each junct has p q
juncts num-juncts junct-size * erase

: nth-junct ( n -- p q ) junct-size * juncts + ;

: calculate-dist ( i j ) ( F: -- n )
  nth-box swap nth-box 
  0 -rot \ init-acc
  3 0 do
    over @ over @ - dup * \ sq
    3 roll + -rot \ acc
    ++ swap ++
  loop 2drop s>f fsqrt ;

: init-juncts
  juncts num-boxes 0 do
    i 0 ?do
      j i 2 pick 2!
      j i calculate-dist dup dist-offset + f!
      junct-size +
    loop 
  loop drop ;
init-juncts

: nth-dist ( n -- F: d ) nth-junct dist-offset + f@ ;
: .j
  newline type
  num-juncts ( 10 ) 0 do i . ( i nth-junct 2@ . . ) i nth-dist f. newline type loop ;

: swap-elems-raw ( addr1 addr2 elem-size -- )
  0 do 
    2dup over c@ over c@
    ( addr1 addr2 addr1 addr2 addr1@ addr2@ )
    >r swap c! r> swap c!
    1+ swap 1+ swap
  loop 2drop ;
: swap-elems { i j arr elem-size -- }
  i elem-size * arr +
  j elem-size * arr +
  elem-size swap-elems-raw ; 

\ create arr 3 , 1 , 4 , 1 , 5 , 9 , 2 , 6 ,
\ create arr 5 , 9 , 4 , 6 ,
\ : .as { n arr } n 0 ?do arr i cells + @ . loop ;
\ : .a 8 arr .as ;

: qscani { xt-cmp elem-size arr n idx }
  n idx <= if exit then
  n n idx 1+ ?do
    arr arr i elem-size * + xt-cmp execute 0 >= if
      drop i leave
    then
  loop ;

: qscanj { xt-cmp elem-size arr n idx }
  0 idx >= if exit then
  0 0 idx 1- ?do
    arr arr i elem-size * + xt-cmp execute 0 <= if
      drop i leave
    then
    -1
  +loop ;

: qscan { xt-cmp elem-size arr n -- j }
  \ arr n 1- elem-size * + xt-cmp execute { pivot }
  n 0 begin
    ( j i )
    >r xt-cmp elem-size arr n r> qscani
    swap
    ( i' j )
    >r xt-cmp elem-size arr n r> qscanj
    swap
    ( j' i' )
    2dup > if 
      2dup arr elem-size swap-elems 
    else
      drop dup 0 arr elem-size swap-elems exit
    then
  again ;

: dist-cmp ( addr addr ) 
  dist-offset + f@ dist-offset + f@
  fover fover f= if fdrop fdrop 0 else
    f> if 1 else -1 then
  then ;

: qsort { xt-cmp elem-size arr n -- }
  n 1 <= if exit then
  xt-cmp elem-size arr n qscan { j-idx }
  xt-cmp elem-size arr j-idx recurse
  xt-cmp elem-size arr j-idx 1+ elem-size * +  n j-idx - 1- recurse 
  ;

: get-dist dist-offset + @ ;
: sort-juncts ( -- )
  ['] dist-cmp junct-size juncts num-juncts qsort ;
sort-juncts

\ two boxes have the same tag iff they are part of the same circuit
create box-tags num-boxes cells allot
box-tags num-boxes cells -1 fill \ -1 for box with no tag

num-boxes constant num-tags
create used-tags num-tags cells allot
used-tags num-boxes cells erase \ tags start unused

: nth-box-tag ( n -- tag )
  cells box-tags + @ ;

: .t
  newline type
  num-tags 0 do 
    i cells used-tags + @ if
      i . ." : { "
      i num-boxes 0 do
        dup i nth-box-tag ( .s newline type ) = if i . then
      loop drop
      ." }" newline type
    then
  loop ;

: next-tag ( -- n )
  num-tags 0 do
    used-tags i cells + dup @ invert if true swap ! i leave then drop
  loop ;

: erase-tag ( n -- )
  0 swap cells used-tags + ! ;

: set-box-tag ( tag n -- )
  cells box-tags + ! ;

: tagged? -1 <> ;
: retag { src dst }
  num-boxes 0 do i nth-box-tag src = if dst i set-box-tag then loop 
  src erase-tag ;

: junct>tags ( n ) nth-junct
  2@ { p q }
  p nth-box-tag q nth-box-tag { p-tag q-tag }
  p-tag tagged? q-tag tagged? and if
    p-tag q-tag = if exit then \ in the same circuit
    p-tag q-tag max p-tag q-tag min retag
    exit
  then
  p-tag tagged? q-tag tagged? invert and if
    p-tag q set-box-tag
    exit
  then
  p-tag tagged? invert q-tag tagged? and if
    q-tag p set-box-tag
    exit
  then
  next-tag
  dup p set-box-tag
  q set-box-tag ; 

: finished? true num-boxes 0 do i nth-box-tag 0 <> if drop false leave then loop ;
: parttwo 
  num-juncts 0 do 
    i junct>tags 
    finished? if 
      i nth-junct 2@ nth-box @ swap nth-box @ * leave 
    then 
  loop ;

parttwo
