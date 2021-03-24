% Author: Loizos Michael
% Date: 2017-05-20

elaborate(KB,X,Elab) :-
   format("\n============ ELABORATE ============\n\n"),
   derivation(KB,X,[],DN), DN=(IRules,Prefs),
   format("Derivations:\n"),
   format("Rules: ~p\nPrefs: ~p\n", [IRules,Prefs]),
   switch([steps], format("\nMarked to bootstrap inference : ~p", [X])),
   findall(why(T,[seen(T)]),
      member(T,X), MarkedE),
   MarkedR=[], MarkedT=X,
   Marked=(MarkedR,MarkedT,MarkedE),
   switch([steps], format("\nRecording simple explanations : ~p", [MarkedE])),
   acceptable(DN,X,Marked,Elab),
   Elab=(ElabR,ElabT,ElabE),
   format("\n\nElaboration:\n"),
   format("Rules: ~p\nTerms: ~p\nExpls: ~p\n", [ElabR,ElabT,ElabE]),
   format("\nActions:"),
   findall(!P, ( member(!P,ElabT),
      format("~p...", [P]),
      if(clause(P,_),(call(P) ; true))
   ), _), nl.


acceptable((IRules0,Prefs0),X,Marked0,Marked3) :-
   switch([steps], format("\n\nrepeat")),
   Marked0=(MarkedIR0,MarkedT0,MarkedE0),

   % Retain consistency
   switch([steps], format("\nRemoved to retain consistency : ")),
   findall(IR, ( member(IR,IRules0),
      \+disabled(IR,MarkedT0)
   ), IRules1),
   fixprefs(Prefs0,IRules1,Prefs1),
   subtract(IRules0,IRules1,RemovedIR1),
   switch([steps], format("~p", [RemovedIR1])),

   % Force groundedness
   switch([steps], format("\nRemoved to force groundedness : ")),
   %findall(IR, ( member(IR,IRules1),
   %   crowned(IR,IRules1,X)
   %), IRules2),
   allcrowned2(IRules1,X,IRules2),
   fixprefs(Prefs1,IRules2,Prefs2),
   subtract(IRules1,IRules2,RemovedIR2),
   switch([steps], format("~p", [RemovedIR2])),

   % Respect preferences
   switch([steps], format("\nMarked to respect preferences : ")),
   findall(IR1, ( member(IR1,IRules2),
      effected(IR1,MarkedT0),
      \+((member(IR2,IRules2), attacks(IR2,IR1,Prefs2)))
   ), AddMarkedIR),
   add2set(MarkedIR0,AddMarkedIR,MarkedIR2),
   subtract(MarkedIR2,MarkedIR0,NewlyMarkedIR),
   switch([steps], format("~p", [NewlyMarkedIR])),

   % Propagate inference
   switch([steps], format("\nMarked to propagate inference : ")),
   findall(T, ( member(IR,NewlyMarkedIR),
      IR=rule(_N1,_B,T)
   ), AddMarkedT),
   add2set(MarkedT0,AddMarkedT,MarkedT2),
   subtract(MarkedT2,MarkedT0,NewlyMarkedT),
   switch([steps], format("~p", [NewlyMarkedT])),

   % Recording explanations
   switch([steps], format("\nRecording simple explanations : ")),
   findall(why(T,TE), ( member(T,NewlyMarkedT),
      IR=rule(_N2,B,T), member(IR,NewlyMarkedIR),
      findall(BTE, ( member(BT,B),
         BTE=[infer(BT)] % single-step explanation
         %member(why(BT,BTE),MarkedE0) % recursive explanation
      ), BE),
      append([[IR]|BE],TE)
   ), NewlyMarkedE),
   add2set(MarkedE0,NewlyMarkedE,MarkedE2),
   Marked2=(MarkedIR2,MarkedT2,MarkedE2),
   switch([steps], format("~p", [NewlyMarkedE])),

   % Loop until convergence
   Changes=(RemovedIR1,RemovedIR2,NewlyMarkedIR,NewlyMarkedT),
   if(Changes==([],[],[],[]), Marked3=Marked0,
      acceptable((IRules2,Prefs2),X,Marked2,Marked3) ).