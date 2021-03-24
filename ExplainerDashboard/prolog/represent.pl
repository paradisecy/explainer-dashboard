% Author: Loizos Michael
% Date: 2017-05-21

:- op(450, xfx, (@)),
   op(500, fx, [(-),(+),(/),(*),(=)]),
   op(500, fx, [(-),(?),(!)]),
   op(1025, xfx, (::)), (multifile (::)/2),
   discontiguous((::)/2), (dynamic (::)/2),
   op(1025, xfx, (>>)), (multifile (>>)/2),
   discontiguous((>>)/2), (dynamic (>>)/2),
   op(1025, xfx, (~~)), (multifile (~~)/2),
   discontiguous((~~)/2), (dynamic (~~)/2).
keywords(Pos, Keywords) :- op(1020, Pos, Keywords).
:- keywords(xfx, [implies, (~>)]).

% ==============================================================

universally(L,Rule) :-
   if(\+ground(L),
   warning(1,['Ungrounded rule head: ',Rule])).

ext2int(E,I,rule) :-
   E=(N :: BP implies H),
   I=rule(N,BL,H),
   if(\+var(BP),toList((,),BP,BL)),
   if(\+var(BL),toPair((,),BL,BP)).
ext2int(E,I,pref) :-
   E=(N1 >> N2),
   I=pref(N1,N2).
ext2int(E,I,conf) :-
   E=(N ~~ W),
   I=conf(N,W).

atomize(-A,A,-A) :- A\=(-_), !.
atomize(A,A,-A) :- A\=(-_), !.
%atomize(L,_,_) :- error(1,[L]).

negate(L,NL) :-
   atomize(L,L,NL) ;
   atomize(L,NL,L).
   
% ==============================================================

printKB((Rules,Prefs,Confs)) :-
   format("Rules: ~p\nPrefs: ~p\nConfs: ~p\n", [Rules,Prefs,Confs]).

% ==============================================================

readFile(File,(Rules,Prefs,Confs),VarNames) :-
   if(\+access_file(File,read), (
      open(File,write,Stream1),
      close(Stream1) )),
   open(File,read,Stream),
   readList(Stream,(Rules,Prefs,Confs),VarNames),
   close(Stream).

readList(Stream,(Rules1,Prefs1,Confs1),VarNames1):-
   read_term(Stream,E,[variable_names(VNs)]),
   E\=end_of_file, !,
   ext2int(E,I,Type),
   caseDo(Type, [
   (rule, (
      Rules1=[I|Rules0],
      Prefs1=Prefs0,
      Confs1=Confs0
      )),
   (pref, (
      Rules1=Rules0,
      Prefs1=[I|Prefs0],
      Confs1=Confs0
      )),
   (conf, (
      Rules1=Rules0,
      Prefs1=Prefs0,
      Confs1=[I|Confs0]
      ))
   ]),
   append(VNs,VarNames0,VarNames1),
   readList(Stream,(Rules0,Prefs0,Confs0),VarNames0).
readList(_Stream,([],[],[]),[]).


saveFile(File,(Rules,Prefs,Confs),VarNames) :-
   open(File,write,Stream),
   saveList(Stream,Rules,VarNames),
   saveList(Stream,Prefs,VarNames),
   saveList(Stream,Confs,VarNames),
   close(Stream).

saveList(_Stream,[],_VarNames).
saveList(Stream,[I|List],VarNames):-
   ext2int(E,I,Type),
   E=..[EB,EA,EC],
   write_term(Stream,EA,[variable_names(VarNames),quoted(true)]),
   write_term(Stream,' ',[]),
   write_term(Stream,EB,[variable_names(VarNames),quoted(true)]),
   write_term(Stream,' ',[]),
   caseDo(Type, [
   (rule, (
       EC=..[EC2,EC1,EC3],
       write_term(Stream,EC1,[variable_names(VarNames),quoted(true)]),
       write_term(Stream,' ',[]),
       write_term(Stream,EC2,[variable_names(VarNames),quoted(true)]),
       write_term(Stream,' ',[]),
       write_term(Stream,EC3,[variable_names(VarNames),quoted(true)])
      )),
   (pref, (
       write_term(Stream,EC,[variable_names(VarNames),quoted(true)])
      )),
   (conf, (
       write_term(Stream,EC,[variable_names(VarNames),quoted(true)])
      ))
   ]),
   write_term(Stream,'.',[nl(true)]),
   saveList(Stream,List,VarNames).
