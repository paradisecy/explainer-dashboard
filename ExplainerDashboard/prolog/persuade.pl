% Author:
% Date: 2017-08-30


% check what is persuading, and then use it to persuade

persuador([],_Mode,-KB,_X,_Primed).
persuador([H|Terms],Mode,KB,X,Primed) :-
   if(persuasive(H,Mode,KB,X,Primed),
      format("Accept ~p in context ~p and primed on ~p\n", [H,X,Primed]),
      format("Reject ~p in context ~p and primed on ~p\n", [H,X,Primed]),
   ),
   priming(Primed1),
   persuador(Terms,Mode,KB,X,Primed)
   

persuasive(H,Mode,KB,X,Primed) :-
   add2set(X,Primed,XandPrimed),
   derivation(KB,XandPrimed,,(IRulesAll,PrefsAll)),
   derivation((IRules,Prefs,[]),XandPrimed,[bound,0],(IRulesOne,PrefsOne)),
   findall(IR, ( member(IR,IRules),
      \+( member(IR3,IRulesOne),
          attacks(IR3,IR,PrefsOne) )
   ), IRules1),
   caseDo(Mode, [
   (skeptical, (
      IR1=rule(_,_,H),
      crowned(IR1,IRules1,X),
      \+( crowned(IR2,IRules1,X),
          attacks(IR2,IR1,Prefs1) )
      )),
   (credulous, (
      negate(H,NH),
      IR1=rule(_,_,NH),
      \+crowned(IR1,IRules1,X)
      ))
   ]).
   
   