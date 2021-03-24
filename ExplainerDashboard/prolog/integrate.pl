% Author: Loizos Michael
% Date: 2017-05-20

integrate(print,Change,Elab,KB0,KB1) :-
   format("\n============ INTEGRATE ============\n"),
   integrate(Change,Elab,KB0,KB1),
   printKB(KB1).

   
integrate(null,_Elab,KB,KB) :- !.

integrate(+pref(P),Elab,KB0,KB2) :-
   P=pref(N1,N2), NP=pref(N2,N1),
   integrate(-pref(NP),Elab,KB0,KB1),
   KB1=(Rules1,Prefs1,Confs1),
   KB2=(Rules1,[P|Prefs1],Confs1).

integrate(-pref(P),_Elab,KB0,KB1) :-
   KB0=(Rules0,Prefs0,Confs0),
   subtract(Prefs0,[P],Prefs1),
   KB1=(Rules0,Prefs1,Confs0).


integrate(+rule(R),Elab,KB0,KB2) :-
   KB0=(Rules0,_Prefs0,_Confs0),
   newprefs([R],Rules0,AddPrefs),
   addprefs(AddPrefs,Elab,KB0,KB1),
   KB1=(Rules1,Prefs1,Confs1),
   KB2=([R|Rules1],Prefs1,Confs1).
integrate(-rule(R),_Elab,KB0,KB1) :-
   KB0=(Rules0,Prefs0,Confs0),
   subtract(Rules0,[R],Rules1),
   fixprefs(Prefs0,Rules1,Prefs1),
   KB1=(Rules1,Prefs1,Confs0).

integrate(+derv(D),Elab,KB0,KB2) :-
   Elab=(Rules,_Terms,_Expls),
   newprefs(D,Rules,AddPrefs),
   addprefs(AddPrefs,Elab,KB0,KB1),
   KB1=(Rules1,Prefs1,Confs1),
   union(Rules1,D,Rules2),
   KB2=(Rules2,Prefs1,Confs1).


integrate(-term(T),Elab,KB0,KB1) :-
   Elab=(_Rules,_Terms,Expls),
   member(why(T,ExpT),Expls),
   R=rule(_N,_B,T), member(R,ExpT),
   integrate(/rule(R),Elab,KB0,KB1).

integrate(/rule(R),_Elab,KB0,KB2) :-
   KB0=(Rules0,Prefs0,Confs0),
   R=rule(N,_B,_H),
   fixconf(N,Confs0,0.5,_C,Confs2),
   KB2=(Rules0,Prefs0,Confs2).


newprefs(Strong,Weak,AddPrefs) :-
   findall(P, ( member(R,Strong),
      R=rule(N,_B,H), negate(H,NH),
      member(R1,Weak),
      R1=rule(N1,_B1,NH), P=pref(N,N1)
   ), AddPrefs).

addprefs([],_Elab,KB0,KB0).
addprefs([P|Prefs],Elab,KB0,KB2) :-
   integrate(+pref(P),Elab,KB0,KB1),
   addprefs(Prefs,Elab,KB1,KB2).

fixprefs(Prefs0,Rules,Prefs1) :-
   findall(P, (
      member(P,Prefs0), P=pref(N1,N2),
      once(member(rule(N1,_B1,_H1),Rules)),
      once(member(rule(N2,_B2,_H2),Rules))
   ), Prefs1).


fixconf(N,Confs0,Mul,C2,Confs2) :-
   if(member(conf(N,C0),Confs0),
      subtract(Confs0,[conf(N,C0)],Confs1),
      (Confs1=Confs0, C0=4) ),
   C2 is C0*Mul, Confs2=[conf(N,C2)|Confs1].

highconf(N,Confs0) :-
   if(member(conf(N,C0),Confs0), C0>=1).
