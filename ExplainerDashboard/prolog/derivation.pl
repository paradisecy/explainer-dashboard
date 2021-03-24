% Author: Loizos Michael
% Date: 2017-08-11

:- use_module(library(tabling)).

%:- table derivation/4.
derivation(KB,Terms,Params,DN) :-
   derivation(KB,Terms,Params,0,DN).

derivation(KB,Terms1,Params,Depth1,DN) :-
   KB=(Rules,Prefs,Confs),
   lookup(Params,exclude,[],Excl),
   subtract(Terms1,Excl,InTerms),
   findall((IR,T), ( member(R,Rules),
      triggered(R,Confs,InTerms,IR), IR=rule(_N,_B,T)
   ), Reachable),
   unzip((,),Reachable,IRules1,AddTerms),
   lookup(Params,search,-,Srch),
   if(subset(Srch,IRules1), DN=found, (
      Depth2 is Depth1+1,
      lookup(Params,bound,Depth2,Bound),
      add2set(InTerms,AddTerms,Terms2),
      if((Depth1>=Bound ; subset(Terms2,Terms1)), (
         IRules2=IRules1,
         fixprefs(Prefs,IRules2,Prefs2),
         DN=(IRules2,Prefs2)
      ), derivation(KB,Terms2,Params,Depth2,DN) )
   ) ).


triggered(R,Confs,Terms,IR) :-
   R=rule(N,B,H),
   highconf(N,Confs),
   satisfied(B,Terms,IB),
   IR=rule(N,IB,H),
   universally(H,IR).

satisfied([],_Terms,[]).
satisfied([T|B],Terms,IB) :-
   T=(?P), call(P),
   satisfied(B,Terms,IB).
satisfied([T|B],Terms,[T|IB]) :-
   T\=(?_P), member(T,Terms),
   satisfied(B,Terms,IB).

effected(IR,Terms) :-
   IR=rule(_N,B,_H),
   subset(B,Terms).

disabled(IR,Terms) :-
   IR=rule(_N,B,H), member(T,[H|B]),
   negate(T,NT), member(NT,Terms).

attacks(IR1,IR2,Prefs) :-
   IR1=rule(N1,_B1,H1), IR2=rule(N2,_B2,H2),
   negate(H1,H2), \+member(pref(N2,N1),Prefs).

:- table crowned/3.
crowned(IR,IRules,X) :-
   %write('<'), flush_output,
   IR=rule(_N,_B,H), KB=(IRules,[],[]),
   derivation(KB,X,[(exclude,[H]),(search,[IR])],found).

:- table allcrowned1/3.
allcrowned1(IRules,X,Crowned) :-   
   derivation((IRules,[],[]),X,[],(Reach,_)),
   findall(IR, ( member(IR,Reach),
      crowned(IR,Reach,X)
   ), Crowned).

:- table allcrowned2/3.   
allcrowned2(IRules,X,Crowned) :-
   derivation((IRules,[],[]),X,[],(Reach,_)),
   findall(H, member(rule(_N,_B,H),Reach), Hs),
   sort(Hs,SortedHs),
   findall((H,HRules), ( member(H,SortedHs),
      findall(rule(N,B,H),
             member(rule(N,B,H),Reach), 
      HRules)
        ), Groups),
   findall(HCrowned, ( member((H,HRules),Groups),
   %write('<'), flush_output,
      derivation((Reach,[],[]),X,[(exclude,[H])],(ReachH,_)),
          intersection(HRules,ReachH,HCrowned)
   ), CrownedList),
   append(CrownedList,Crowned).
         