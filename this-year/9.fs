\ : s s" input-9.txt" slurp-file ;
: s s" input-test-9.txt" slurp-file ;
\ 100000 constant width \ should generate this programmatically but...
12 constant width \ should generate this programmatically but...

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


: point here >r swap , , r> ;

: xs ( rect -- x1 x2 )
  dup @ @ swap ++ @ @ ;
: ys ( rect -- y1 y2 )
  dup @ ++ @ swap ++ @ ++ @ ;
: between ( a b c -- b )
  2dup <= >r
  drop <= r> and ;

: contains-red? { x1 y1 x2 y2 -- b }
  false num-reds 0 do
    x1 x2 min i nth-red @     x1 x2 max between
    y1 y2 min i nth-red ++ @  y1 y2 max between 
    and if drop true then
  loop ;

: validation-rects { pt1 pt2 -- x1 y1 x2 y2 x3 y3 x4 y4 }
  pt1 @ pt2 @ <
  pt1 ++ @ pt2 ++ @ < xor if
    ( y=x shaped )
    pt1 @ pt2 @ min
    pt1 ++ @ pt2 ++ @ min
    0 0

    pt1 @ pt2 @ max
    pt1 ++ @ pt2 ++ @ max
    width width
  else
    ( y=-x shaped )
    pt1 @ pt2 @ min
    pt1 ++ @ pt2 ++ @ max
    0 width

    pt1 @ pt2 @ max
    pt1 ++ @ pt2 ++ @ min
    width 0
  then ;

: print-rect ( rect )
  dup @ dup @ . ++ @ .
  ++ @ dup @ . ++ @ . ;

: rectable? ( pt1 pt2 -- b )
  validation-rects contains-red? >r contains-red? r> and ;

: parttwo
  0 num-reds 0 do
    i 0 ?do
      i nth-red j nth-red 
      2dup rectable? if
        rect size 2dup < if dup . max else drop then
        \ dup 50 = if i . j . then
      else
        2drop
      then
    loop
  loop ;
( need to do this with grid and col and row checking to alternatively fill with greens. )
