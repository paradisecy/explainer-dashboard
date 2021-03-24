% Author:
% Date: 2017-05-25

perceive(X0,KB,X1) :-
   repeat,
   format("\n============ PERCEIVE ============\n\n"),
   format("Current knowledge base :\n"),
   printKB(KB),
   format("Current context in use : ~p\n\n", [X0]),
   format("How do you want to proceed?\n\n"),
   format("0. Save the current knowledge base and exit!\n"),
   format("1. I want to (slightly) modify this context.\n"),
   format("2. I want to choose an entirely new context.\n"),
   format("3. You go ahead and choose the next context.\n"),
   format("4. Create a new context uniformly at random.\n"),
   format("\nResponse...? "), read(Ans),
   caseDo(Ans, [
   (0, (
      X1=X0
      )),
   (1, (
      repeat,
      format("Enter an update: "), read(U),
      forall(member(M,U),member(M,[+cont(_),-cont(_)])),
      modifyContext(U,X0,X1)
      )),
   (2, (
      repeat,
      format("Type the context: "), read(X1),
      is_list(X1), ground(X1)
      )),
   (3, (
      repeat,
      format("Max context size: "), read(Max),
      chooseContext(KB,Max,X1)
      )),
   (4, (
      repeat,
      format("Max context size: "), read(Max),
      randomContext(KB,Max,X1)
      ))
   ]), !.


modifyContext([],X0,X0).
modifyContext([+cont(L)|U],X0,X2) :-
   negate(L,NL), subtract(X0,[NL],X1),
   modifyContext(U,[L|X1],X2).
modifyContext([-cont(L)|U],X0,X2) :-
   subtract(X0,[L],X1),
   modifyContext(U,X1,X2).

chooseContext(KB,Max,X) :-
   randomContext(KB,Max,X).

randomContext(KB,Max,X) :-
   KB=(Rules,_Prefs),
   findall(PL, ( member(R,Rules),
      R=rule(_N;B;H), member(L,[H|B]),
      atomize(L,PL,_NL)
   ), PLs1),
   sort(PLs1,PLs2),
   Size is random(Max+1),
   sample(PLs2,Size,PX),
   track([Size,Max,PX]),
   findall(RL, ( member(L,PX),
      atomize(L,PL,NL),
      sample([PL,NL],1,[RL])
   ), X).

