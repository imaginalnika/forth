: max-digit-index { addr total-len -- idx c }
  0 addr total-len 48 ( '0' ) total-len 0
  do
    ( idx addr len c )
    >r over c@ r>
    ( idx addr len c c )
    2dup > if drop >r rot drop dup -rot r>
    else nip then
    ( idx addr len c )
    >r 1 /string r>
    loop -rot drop drop
    ( idx-reversed c )
    swap total-len swap - swap ;

: s1 s" 987654321111111" ;
: s2 s" 811111111111119" ;
: s3 s" 234234234234278" ;
: s4 s" 818181911112111" ;

: c>n 48 - ;

: max-joltage ( addr len -- int )
  2dup 1- max-digit-index
  ( addr len idx tens-c )
  >r 1+ /string max-digit-index
  ( idx ones-c : tens-c )
  nip r>
  ( ones-c tens-c )
  c>n 10 * swap c>n + ;

\ : s s" test-input-3.txt" slurp-file ;
\ 15 value line-length
\ 4 value num-banks

: s s" input-3.txt" slurp-file ;
100 value line-length ( including newline )
200 value num-banks

: first-line line-length min ;
: rest-lines dup line-length 1+ min /string ;

: +3rd ( a b c d -- a+d b c )
  >r rot r> + -rot ;

: part-one
  0 s num-banks 0 do 2dup first-line max-joltage ( dup . ) +3rd rest-lines loop 2drop ;

: **  ( base exp -- result )
  1 swap 0 ?do over * loop nip ;

: max-large-joltage ( addr len -- int )
  0 -rot 12 0 do
    ( acc addr len )
    2dup 11 i - - max-digit-index
    ( acc addr len idx c )
    >r 1+ /string r>
    ( acc addr' len' c )
    c>n 10 11 i - ** * +3rd
  loop 2drop ;

: part-two
  0 s num-banks 0 do 2dup first-line max-large-joltage ( dup . ) +3rd rest-lines loop 2drop ;
