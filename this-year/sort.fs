\ create arr 9 , 1 , 2 , 3 , 2 , 1 , 7 , 
\ : log arr 7 0 do dup @ . 1++ loop drop ;

: ++ cells + ;
: -- cells - ;
: 1++ 1 ++ ;
: 1-- 1 -- ;

: last ( arr len )
  ++ 1-- @ ;

: insert? ( value addr )
  @ <= ;
: -rotate ( arr len )
  dup 1 <= if 2drop else
    2dup last -rot
    1- cells over dup 1++ rot move
    ! then ;

: insert-last { addr len }
   addr len last 
   addr len 0 do
     ( last-value addr )
     2dup i ++ insert? if
       dup i ++ len i - -rotate
       leave
     then loop 2drop ;

: sort ( addr len )
  dup 1 <= if 2drop else
    dup 1+ 2 do
      2dup insert-last
      loop 2drop 
    then ;
