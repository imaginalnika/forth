256 buffer: linebuf

: each-line ( xt filename-addr filename-len -- )
  r/o open-file throw >r
  begin
    linebuf 256 r@ read-line throw
  while
    linebuf swap rot dup >r execute r>
  repeat
  drop drop r> close-file throw ;

: parse-r over c@ >r 1 /string s>number? drop d>s r> 76 ( L ) = if negate else then ;

: count-step ( counts dial r-str -- counts' dial' )
  parse-r + 100 mod
  ( dial' )
  dup 0= if swap 1+ swap then ;

: day1a 0 50 ' count-step s" input-1.txt" each-line .s ;

: clicks ( dial r -- clicks ) 
over + ( dial dial+r )
dup 0= if
  drop ( dial ) 0> if 1 else 0 then
else
  dup 0> if
    100 / nip
  else
    abs 100 / swap ( dial ) 0> if 1+ then
  then
then ;

: rotate-dial ( dial r -- clicks dial' ) 2dup + 100 mod -rot clicks swap ;

: dial-step ( clicks dial r-str -- clicks' dial' ) 
  parse-r rotate-dial -rot + swap ;

: day1b 0 50 ' dial-step s" input-1.txt" each-line .s ;
