: s s" input-test-8.txt" slurp-file ;

: before-char >r 2dup r> scan nip - ;
: after-char scan dup 0<> if 1 /string then ;
: first-line 10 before-char ;
: rest-lines 10 after-char ;
: /char ( a n c -- str-after-char str-before-char )
  >r 2dup r@ after-char 2swap r> before-char ;

: get-num-boxes 0 s begin
    rest-lines rot 1+ -rot
    dup 0= until 2drop ;
get-num-boxes constant num-boxes

: boxes cells 3 * ;
create boxes-arr num-boxes boxes allot

: line-at ( n )
  s rot 0 ?do rest-lines loop first-line ;

: s>n  ( addr u -- n )
  0 0 2swap >number 2drop drop ;
: parse-box-line ( s )
  ',' /char s>n -rot ',' /char s>n -rot s>n ;
: 3reverse swap rot ;
: init-boxes-arr
  num-boxes 0 do
    i line-at parse-box-line 3reverse
    boxes-arr i boxes + >r
    r@ !
    r@ 1 cells + !
    r> 2 cells + !
  loop ;
init-boxes-arr

\ box is ( n n n )
: box-at ( n -- box )
  boxes boxes-arr + >r
  r@ @
  r@ 1 cells + @
  r> 2 cells + @ ;
 
: sq dup * ;
: calculate-dist ( n n -- ) ( F: -- r )
  >r box-at r> box-at
  3 pick - abs sq >r
  3 pick - abs sq >r
  3 pick - abs sq >r
  drop drop drop 
  r> r> r> + + s>f fsqrt ;

create dist-arr \ each element is dist box-num box-num
  num-boxes num-boxes * dup
  floats swap 2 * cells + allot
: dist-arr-elems dup 2 * cells swap floats + ;

: init-dist
  num-boxes 0 do
    num-boxes 0 do
      j num-boxes * i + dist-arr-elems dist-arr + { arr }
      i j calculate-dist arr f!
      \ i j . . arr f@ f.
      i arr 1 floats + !
      j arr 1 floats + 1 cells + !
    loop
  loop ;
init-dist

: dist ( n n )
  num-boxes * + dist-arr-elems dist-arr + f@ ;
: dist-arr-at ( n )
  dist-arr-elems dist-arr + ;

: dist-insert-idx ( n )
  dup 1- dist-arr-at f@
  dup 0 do 
    ( n ) ( F: -- dist )
    \ fdup f.
    fdup i dist-arr-at f@ ( fdup f. ) f< if
      drop fdrop i leave
    then
    dup i 1+ = if
      drop fdrop i leave
    then
  loop ;

: print-dist-arr ( n )
  0 do i dist-arr-at f@ f. loop ;

create buf 1 dist-arr-elems allot
: sort-last-dist { n }
  n 1- dist-arr-at buf 1 dist-arr-elems move
  n dist-insert-idx dup { insert-idx } dist-arr-at 
  \ insert-idx . newline type
  dup 1 dist-arr-elems +
  n insert-idx - 1- ( dup . newline type ) dist-arr-elems
  move
  buf insert-idx dist-arr-at 1 dist-arr-elems move
  \ n print-dist-arr newline type
  ;

  
: sort-dist-arr
  num-boxes num-boxes * 2 do i sort-last-dist loop ;
sort-dist-arr

: nth-smallest-pair ( n )
  \ 0 is smallest
  2 * num-boxes 1- ( skip 0's ) +
  dist-arr-at dup 1 cells + @ swap 2 cells + @ swap ;
