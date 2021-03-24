:- set_prolog_flag(print_write_options, [
      portray(true), numbervars(true), quoted(true) ]).

if(Cond,Then) :-
   if(Cond,Then,true).
if(Cond,Then,Else) :-
   once(Cond -> Then ; Else).

lookup(Data,Key,Default,Value) :-
   if(member((Key,Value),Data),
      true, Value=Default), !.

caseDo(Case,List) :-
   lookup(List,Case,fail,Do),
   once(call(Do)), !.

add2set(Set,List,UnionSet) :-
   union(Set,List,Union),
   sort(Union,UnionSet).

sample(List,Many,Out) :-
   random_permutation(List,List1), !,
   length(List1,Length),
   Available is min(Many,Length),
   prefix(List1,Available,Out).

prefix(In,Length,Out) :-
   length(In,InLength),
   (InLength =< Length
   -> Out=In ; (
   length(Out,Length),
   append(Out,_,In) )).

suffix(In,Length,Out) :-
   length(In,InLength),
   (InLength =< Length
   -> Out=In ; (
   length(Out,Length),
   append(_,Out,In) )).

unzip(_Op,[],[],[]).
unzip(Op,[Pair|P],[A|L],[B|R]) :-
   Pair=..[Op,A,B], unzip(Op,P,L,R).

toList(Op,Pair,[A|BL]) :-
   Pair=..[Op,A,B], !,
   toList(Op,B,BL).
toList(_Op,true,[]) :- !.
toList(_Op,A,[A]).

toPair(_Op,[],true).
toPair(_Op,[A],A).
toPair(Op,[A|BL],Pair) :-
   toPair(Op,BL,B),
   Pair=..[Op,A,B].

switch(Cond,Goal) :-
   switches(Switches),
   subset(Cond,Switches),
   once(call(Goal)), !.
switch(_Cond,_Goal).

enlist(List) :-
   switch([list(Max)], (
      number(Max),
      write_term(List,N),
      prefix(List,Max,Prex),
      listaux(Prex),
      format("...<out of ~p>", [N])
      )),
   switch([list(full)], (
      listaux(List) )),
   format("]").
listaux([H]) :-
   format("~p", [H]).
listaux([H|B]) :-
   format("~p,", [H]),
   switch([list(full)], (
      flush_output )),
   listaux(B).

track(Message) :-
   switch([track], (
   format("TRACK: ~p\n", [Message]) )).

warning(Code,Message) :-
   switch([warning], (
   format("~~~ Warning Code ~p: ~p\n", [Code,Message]) )).

error(Code,Message) :-
   switch([error], (
   format("~~~ Error Code ~p: ~p\n", [Code,Message]), abort )).

say([]) :- !.
say([H|B]) :-
   H=[_|_], !,
   list(H),
   say(B).
say([H|B]) :-
   write(H),
   say(B).

list(List) :-
   write('['),
   switch([list(Max)], (
      number(Max),
      length(List,N),
      say(['<',N, '>...']),
      prefix(List,Max,Pref),
      show(Pref) )),
   switch([list(full)], (
      show(List) )),
   write(']').

show([H]) :-
   write(H).
show([H|B]) :-
   write(H),
   write(','),
   switch([list(full)], (
      flush_output )),
   show(B).
% ==============================================================
