: until-char >r 2dup r> scan nip - ;
: line 10 until-char ;
: rest 10 scan 1 /string ;
: s s" input-7.txt" slurp-file ;

s line 1+ constant width
create beams width allot

: log1 beams width 0 do dup i + c@ if -1 . else 0 . then loop drop ;
: init-beams s line 0 do dup c@ 'S' = if true beams i + c! then 1+ loop 2drop ;
init-beams
: split { i }
  beams i + 1- dup 1+ dup 1+
  -1 swap c! 0 swap c! -1 swap c! ;

: update-beams ( addr len -- n )
  0 -rot 0 do ( acc addr )
    dup i + c@ '^' =  
    beams i + c@ and if
      swap 1+ swap
      i split
    else then loop drop ;

: num-chars { addr len c }
  0 addr 
  len 0 do 
    dup i + c@ c = negate
    rot + swap loop drop ;
s 10 num-chars 2 / 1- constant num-split-rows 

: part-one 0 s rest rest
  num-split-rows 0 do
    ( acc addr len )
    2dup line update-beams ( n )
    3 roll + -rot
    rest rest
    loop 2drop ;

create timelines width cells allot
: log newline type timelines width 0 do dup i cells + @ . loop drop ;
: init-timelines s line 
   0 do dup i + c@ 'S' = 
       if 1 else 0 then 
       timelines i cells + ! loop drop ;
init-timelines

create timelines-buf width cells allot
: reset-buf timelines-buf width cells erase ;
reset-buf

: logb timelines-buf width 0 do dup i cells + @ . loop drop ;

: split-timeline ( idx )
  dup cells timelines + @ swap
  ( n idx )
  2dup 1- cells timelines-buf + +!
  1+ cells timelines-buf + +! ;

: retain-timeline ( idx )
  dup cells timelines + @ 
  swap cells timelines-buf + +! ;

: paste-buf timelines timelines-buf
  width 0 do
    2dup @ swap !
    1 cells + swap 1 cells + swap
    loop 2drop reset-buf ;

: update-timelines ( addr len -- )
  0 do dup i + c@ '^' =
      if i split-timeline
      else i retain-timeline
      then loop paste-buf drop ;

: sum ( arr len )
  0 -rot 0 do
    ( acc arr )
    dup i cells + @
    rot + swap loop drop ;
  
: part-two s rest rest
  num-split-rows 0 do
    ( addr len )
    \ i . newline type
    2dup line update-timelines
    \ log
    rest rest
    loop 2drop timelines width sum ;

part-one part-two
