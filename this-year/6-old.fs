\ : s s" input-6.txt" slurp-file ;
: s s" input-test-6.txt" slurp-file ;

32 constant spc
10 constant \n
: ws? dup spc = >r dup \n = >r 0 = >r r> r> or r> or ;
: trim ( a n ) begin over c@ ws? if 1 /string false else true then until ;
: /space ( a n -- a n a n ) trim 2dup spc scan 2swap 2over nip - ;
: s>n  ( addr u -- n )
  0 0 2swap >number 2drop drop ;
: last-line ( s ) + 1- 1
  begin -1 /string over c@ \n = until 1+ ;
create syms-addr s last-line nip allot
: place! ( offset syms c )
  -rot + ! ;

: copy-c! ( src dst ) 
  swap c@ swap c! ;
: fill-syms ( addr len -- n )
  syms-addr -rot 0 
  do 
    ( syms addr )
    dup c@ ws? invert if 
      2dup swap copy-c! swap 1+ swap
      then 1+ loop drop syms-addr - ;
s last-line fill-syms constant syms-len
: syms syms-addr syms-len ;

create answers syms-len cells allot
: cell++ 1 cells + ;
: init-answers ( -- ) 
  answers syms-len 0 do 
    syms-addr i + c@ '*' = if 1 else 0 then 
    over ! cell++ loop drop ;

: f
  init-answers
  ( finish later )
  syms-len 0 do  loop ;

