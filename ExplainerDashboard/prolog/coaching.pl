% Author: Loizos Michael
% Date: 2017-05-20

coach(KB0,X0,KB2) :-
   perceive(X0,KB0,X1),
   elaborate(KB0,X1,Elab),
   coaching(Elab,X1,Act),
   integrate(Act,Elab,KB0,KB1),
   coach(KB1,X1,KB2).
coach(KB,_X,KB).

coaching(Elab,X,Act) :-
   format("\n============ COACHING ============\n\n"),
   Elab=(Rules,Terms,Expls),
   format("elaboration : ~p\n", [Terms]),
   format("of context  : ~p\n", [X]),
   format("using rules : ~p\n", [Rules]),
   format("by arguing  : ~p\n", [Expls]),
   repeat,
   format("\n"),
   format("How do you want to respond?\n\n"),
   format("0. I accept the elaboration of the context!\n"),
   format("1. I do not recognize a rule you have used.\n"),
   format("2. You seem to have missed one of my rules.\n"),
   format("3. There is an indefensible counter-attack.\n"),
   format("\nResponse...? "), read(Ans),
   caseDo(Ans, [
   (0, (
      Act=null
      )),
   (1, (
      repeat,
      format("Name the rule: "), read(N),
      R=rule(N;_;_), member(R,Rules), Act=(-rule(R))
      )),
   (2, (
      repeat,
      format("Type the rule: "), read(RE),
      ext2int(RE,RI,rule), Act=(+rule(RI))
      )),
   (3, (
      repeat,
      format("Type the derivation: "), read(RsE),
      findall(RI, (member(RE,RsE), ext2int(RE,RI,rule)), RsI),
      length(RsE,Len), length(RsI,Len), Act=(+derv(RsI))
      ))
   ]), !.
