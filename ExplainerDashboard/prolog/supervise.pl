% Author:
% Date: 2017-08-18

supervisor(KB0,X0,KB2) :-
   perceive(X0,KB0,X1),
   elaborate(KB0,X1,Elab),
   supervising(Elab,X1,Act),
   integrate(Act,Elab,KB0,KB1),
   supervisor(KB1,X1,KB2).
supervisor(KB,_X,KB).

supervising(Elab,X,Act) :-
   format("\n============ SUPERVISE ============\n\n"),
   Elab=(Rules,Terms,Expls),
   format("elaboration : ~p\n", [Terms]),
   format("of context  : ~p\n", [X]),
   format("using rules : ~p\n", [Rules]),
   format("by arguing  : ~p\n", [Expls]),
   repeat,
   format("\n"),
   format("How do you want to respond?\n\n"),
   format("0. I accept the elaboration of the context!\n"),
   format("1. I do not agree with an inference/action.\n"),
   format("\nResponse...? "), read(Ans),
   caseDo(Ans, [
   (0, (
      Act=null
      )),
   (1, (
      repeat,
      format("Type the term: "), read(T),
      member(T,Terms), \+member(T,X), Act=(-term(T))
      ))
   ]), !.
