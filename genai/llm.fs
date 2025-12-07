1024 buffer: cmd
variable cmd-len
20000 buffer: response
variable resp-len

: api-key  ( -- addr u )
  s" ~/.env" slurp-file
  s" ANTHROPIC_API_KEY=" search if
    18 /string
    108 min
  then ;

: cmd!  ( addr u -- )  dup cmd-len !  cmd swap move ;
: cmd+  ( addr u -- )  cmd cmd-len @ + swap dup cmd-len +! move ;

: claude-cmd ( prompt-addr u -- addr u )
  s\" curl -s https://api.anthropic.com/v1/messages -H 'x-api-key: " cmd!
  api-key cmd+
  s\" ' -H 'anthropic-version: 2023-06-01' -H 'content-type: application/json' -d '{\"model\":\"claude-sonnet-4-5\",\"max_tokens\":1024,\"messages\":[{\"role\":\"user\",\"content\":\"" cmd+
  cmd+
  s\" \"}]}'" cmd+
  cmd cmd-len @ ;

: claude ( prompt-addr u -- addr u )
  claude-cmd r/o open-pipe throw { fid }
  response 20000 fid read-file throw resp-len !
  fid close-pipe drop
  response resp-len @ rot drop
  ( addr u )
  s\" text\":" search drop 7 /string
  2dup s\" \"}]" search drop nip - ;

: llm claude ;

4096 buffer: msgs
variable msgs-len

: msgs!  ( addr u -- )  dup msgs-len !  msgs swap move ;
: msgs+  ( addr u -- )  msgs msgs-len @ + swap dup msgs-len +! move ;
: idx-user? ( idx ) 4 mod 0= ;
: build-msgs { n -- addr u }
  n s\" [" msgs!
  dup 1- 2* swap 0 do
    n>r
    ( addr u )
    s\" {\"role\":\"" msgs+
    i idx-user? if s" user" else s" assistant" then msgs+
    newline type
    s\" \", \"content\":\"" msgs+
    msgs+
    s\" \"}" msgs+
    i 0<> if s" ," msgs+ then
    nr> 2 -
    loop drop 
  s" ]" msgs+
  msgs msgs-len @ ;
  

: build-msgs-cmd ( ... n -- addr u )
  build-msgs
  s\" curl -s https://api.anthropic.com/v1/messages -H 'x-api-key: " cmd!
  api-key cmd+
  s\" ' -H 'anthropic-version: 2023-06-01' -H 'content-type: application/json' -d '{\"model\":\"claude-sonnet-4-5\",\"max_tokens\":1024,\"messages\":" cmd+
  cmd+
  s\" }'" cmd+ 
  cmd cmd-len @ ;

: chat ( ... n -- addr u )
  build-msgs-cmd r/o open-pipe throw { fid }
  response 20000 fid read-file throw resp-len !
  fid close-pipe drop
  response resp-len @ rot drop
  ( addr u )
  s\" text\":" search drop 7 /string
  2dup s\" \"}]" search drop nip - ;

: ndup { n -- }
  n 0 do n 1- pick loop ;

: chat+ ( ... n -- ... addr u )
  dup >r 2 * ndup
  r> chat ;

\ add string concatenation
