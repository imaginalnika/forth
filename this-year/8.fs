: s s" input-test-8.txt" slurp-file ;

: until-char >r 2dup r> scan nip - ;
: count-char { c } 
  0 -rot begin c scan dup 0<>
    if 1 /string rot 1+ -rot
    else 2drop exit then again ;
: s>n 0 0 2swap >number 2drop drop ;

: /oneline ( addr u -- addr u addr u )
  2dup 10 scan tuck 2>r -
  2r> 1 /string 2swap ;

: /csv-num ( addr u -- addr u n )
  0 0 2swap >number
  dup 0<> if 1 /string then
  rot drop rot ;

: s1 s" 12,34,567" ;


: csv>box ( "1,2,3" -- addr )
  here >r
  /csv-num ,
  /csv-num ,
  /csv-num ,
  2drop r> ;

: -x@ @ ;
: -y@ 1 cells + @ ;
: -z@ 2 cells + @ ;

s 10 count-char constant num-boxes
create boxes num-boxes cells allot

: nth-box cells boxes + @ ;

: init-boxes
  s num-boxes 0 do
    /oneline csv>box i cells boxes + !
  loop 2drop ;
init-boxes

: boxes>junct ( addr addr -- addr )
  swap here >r , , r> ;

num-boxes 1- dup 1+ * 2 / constant num-juncts
create juncts num-juncts cells allot

: init-juncts
  juncts num-boxes 0 do
    i 0 ?do
      dup i nth-box j nth-box boxes>junct swap !
      1 cells +
    loop 
  loop drop ;
init-juncts

: dist ( junct -- ) ( F: -- n )
  0 swap dup 1 cells + @ swap @
  3 0 do
    2dup @ swap @ - dup *
    3 roll + -rot
    1 cells + swap 1 cells +
  loop 2drop s>f fsqrt ;

: box= ( addr addr )
  2dup @ swap @ = -rot
  1 cells + swap 1 cells +
  2dup @ swap @ = -rot
  1 cells + swap 1 cells +
  @ swap @ = and and ;

\ : sort-juncts 
\ ;
