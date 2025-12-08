create buf 160 cells allot
variable len
variable cursor

: char@ ( n -- c )  cells buf + @ ;
: char! ( c n -- )  cells buf + ! ;

: bol         13 emit ;
: clear  27 emit ." [K" ;

: redraw
  bol clear
  len @ 0 ?do i char@ emit loop
  bol
  cursor @ 0 ?do i char@ emit loop ;

: raw    s" stty raw -echo" system ;
: cooked s" stty sane" system ;

: insert-character ( c -- )
  cursor @ char!
  1 cursor +!
  1 len +! ;

: log len @ 0 do i char@ emit loop ;

create str-buf 160 allot

: find-word-end ( -- n )
  cursor @
  begin
    dup len @ < if
      dup char@ 32 <>    \ not space
    else
      false
    then
  while
    1+
  repeat ;

: buf>string ( -- addr len )
  find-word-end
  dup 0 ?do
    i char@ str-buf i + c!
  loop
  str-buf swap ;

: exec  buf>string evaluate ;

: clear-stack  begin depth while drop repeat ;

: safe-eval
  buf>string
  ['] evaluate catch
  if 2drop then ;   \ silently ignore errors (incomplete input)

: live-eval
  clear-stack
  safe-eval ;

: home  27 emit ." [H" ;
: clear-screen  27 emit ." [2J" home ;

: delete-char
  cursor @ 0> if
    -1 cursor +!
    -1 len +!
  then ;

: left-arrow   cursor @ 0> if -1 cursor +! then ;
: right-arrow  cursor @ len @ < if 1 cursor +! then ;

: handle-esc
  key drop   \ eat '['
  key        \ get letter
  case
    'D' of left-arrow endof
    'C' of right-arrow endof
  endcase ;

: repl
  raw
  clear-screen
  0 len !  0 cursor !
  begin
    key
    dup 'q' = if drop cooked exit then
    dup 13 = if 
      drop
      0 len !  0 cursor !
    else
      dup 127 = if drop delete-char else
      dup 27 = if drop handle-esc else
        insert-character
      then then
    then
    live-eval
    home clear .s
    cr clear redraw
  again ;

cooked repl
