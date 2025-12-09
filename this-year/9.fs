: s s" input-9.txt" slurp-file ;
\ : s s" input-test-9.txt" slurp-file ;
100000 constant width \ should generate this programmatically but...
\ 12 constant width \ should generate this programmatically but...

: /oneline ( addr u -- addr u addr u )
  2dup 10 scan tuck 2>r -
  2r> 1 /string 2swap ;

: count-char { c } 
  0 -rot begin c scan dup 0<>
    if 1 /string rot 1+ -rot
    else 2drop exit then again ;

s 10 count-char constant num-reds

: s>n 0 0 2swap >number 2drop drop ;
: csv>red ( "1,2" -- addr )
  here >r
  2dup ',' scan tuck 2>r
  - s>n , 2r> 1 /string s>n , r> ;

create reds num-reds cells allot
: init-reds
  s num-reds 0 do
    /oneline csv>red reds i cells + !
  loop 2drop ;
init-reds

: nth-red cells reds + @ ;
: ++ 1 cells + ;

: rect ( pt pt -- rect )
  here >r swap , , r> ;

: size ( rect -- n )
  dup @ swap ++ @ ( pt pt )
  2dup @ swap @ - abs 1+ >r
  ++ @ swap ++ @ - abs 1+ r> * ;

: partone
  0 num-reds 0 do
    i 0 ?do
      i nth-red j nth-red rect size
      max
    loop
  loop ;

: pt1 @ ;
: pt2 ++ @ ;

: pt here >r swap , , r> ;
: x @ ;
: y ++ @ ;
: xs ( r )
  dup pt1 x swap pt2 x ;
: ys ( r )
  dup pt1 y swap pt2 y ;
: << { a b c -- b }
  a b < b c < and ;

: overlaps? { start1 end1 start2 end2 }
  ( assumes startn endn <= )
  start1 start2 <=
  end1 start2 <= and
  end2 start1 <=
  end2 end1 <= and or invert ;

: goes-across? { p1 p2 r }
  ( assumes p1 p2 <> )
  p1 y p2 y = if
    r ys min  p1 y  r ys max <<
    p1 x p2 x 2dup min -rot max r xs 2dup min -rot max overlaps? and
  else
    r xs min  p1 x  r xs max <<
    p1 y p2 y 2dup min -rot max r ys 2dup min -rot max overlaps? and
  then ;
: print-reds num-reds 0 do i nth-red dup x . y . space loop ;

: valid? { r }
  true num-reds 1- 0 do
    \ i . i 1+ . newline type
    i nth-red i 1+ nth-red r goes-across? if
      drop false leave
    then
  loop 
  num-reds 1- nth-red 0 nth-red r goes-across? if
    drop false
  then ;

create rect-buf 2 cells allot
: rect-buf! { p1 p2 }
  p1 rect-buf ! p2 rect-buf ++ ! ;

: parttwo
  0 num-reds 0 do
    i 0 ?do
      i nth-red j nth-red rect-buf!
      rect-buf valid? if
        rect-buf size max
      then
    loop
  loop ;

partone parttwo
